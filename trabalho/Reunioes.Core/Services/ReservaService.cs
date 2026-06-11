using System;
using System.Collections.Generic;
using System.Linq;
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

        public string AgendarReserva(Reserva reserva)
        {
            if (reserva.Inicio.Hour < 8 || reserva.Fim.Hour > 19 || (reserva.Fim.Hour == 19 && reserva.Fim.Minute > 0))
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

            if (novoInicio.Hour < 8 || novoFim.Hour > 19 || (novoFim.Hour == 19 && novoFim.Minute > 0))
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

        public List<Reserva> ListarTodas() => _context.Reservas.ToList();
    }
}