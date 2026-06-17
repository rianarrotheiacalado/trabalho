using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reunioes.Core.Data;
using Reunioes.Core.Models;

namespace Reunioes.Core.Services
{
    public class ReservaService
    {
        private readonly ReunioesDbContext _context;

        public ReservaService()
        {
            _context = new ReunioesDbContext();
        }

        public List<Reserva> ConsultarReservas(int? salaId, DateTime? data)
        {
            var query = _context.Reservas.Include(r => r.Sala).AsQueryable();

            if (salaId.HasValue)
            {
                query = query.Where(r => r.SalaId == salaId.Value);
            }

            if (data.HasValue)
            {
                var dataSemFuso = new DateTime(data.Value.Date.Ticks, DateTimeKind.Unspecified);
                var proximoDia = dataSemFuso.AddDays(1);
                query = query.Where(r => r.Inicio >= dataSemFuso && r.Inicio < proximoDia);
            }

            return query.OrderBy(r => r.Inicio).ToList();
        }

        public string AgendarReserva(Reserva reserva)
        {
            if (!reserva.TemHorariosValidos())
            {
                return "Erro: O horário de início deve ser anterior ao horário de fim.";
            }

            if (!reserva.EstaNoHorarioComercial())
            {
                return "Erro: Horário permitido apenas entre 08:00 e 19:00.";
            }

            var reservasExistentes = _context.Reservas
                .Where(r => r.SalaId == reserva.SalaId)
                .ToList();

            bool colide = reservasExistentes.Any(r =>
                (reserva.Inicio >= r.Inicio && reserva.Inicio < r.Fim) ||
                (reserva.Fim > r.Inicio && reserva.Fim <= r.Fim) ||
                (reserva.Inicio <= r.Inicio && reserva.Fim >= r.Fim));

            if (colide)
            {
                return "Erro: Já existe uma reserva que colide com este horário para esta sala.";
            }

            _context.Reservas.Add(reserva);
            _context.SaveChanges();
            return "Sucesso";
        }

        public string ReagendarReserva(int reservaId, DateTime novoInicio, DateTime novoFim)
        {
            var reserva = _context.Reservas.Find(reservaId);
            if (reserva == null) return "Reserva não encontrada.";

            if (reserva.Inicio <= DateTime.Now)
            {
                return "Erro: Só é permitido reagendar reservas futuras.";
            }

            var reservaValidadora = new Reserva { Inicio = novoInicio, Fim = novoFim };

            if (!reservaValidadora.TemHorariosValidos())
            {
                return "Erro: O novo horário de início deve ser anterior ao horário de fim.";
            }

            if (!reservaValidadora.EstaNoHorarioComercial())
            {
                return "Erro: Horário permitido apenas entre 08:00 e 19:00.";
            }

            var reservasExistentes = _context.Reservas
                .Where(r => r.SalaId == reserva.SalaId && r.Id != reservaId)
                .ToList();

            bool colide = reservasExistentes.Any(r =>
                (novoInicio >= r.Inicio && novoInicio < r.Fim) ||
                (novoFim > r.Inicio && novoFim <= r.Fim) ||
                (novoInicio <= r.Inicio && novoFim >= r.Fim));

            if (colide) return "Erro: O novo horário colide com uma reserva existente.";

            reserva.Inicio = new DateTime(novoInicio.Ticks, DateTimeKind.Unspecified);
            reserva.Fim = new DateTime(novoFim.Ticks, DateTimeKind.Unspecified);

            _context.SaveChanges();
            return "Sucesso";
        }

        public void CancelarReserva(int id)
        {
            var reserva = _context.Reservas.Find(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
                _context.SaveChanges();
            }
        }

        public List<Reserva> ListarTodas() => _context.Reservas.Include(r => r.Sala).ToList();
    }
}