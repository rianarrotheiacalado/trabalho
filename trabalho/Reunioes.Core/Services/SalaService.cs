using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reunioes.Core.Data;
using Reunioes.Core.Models;

namespace Reunioes.Core.Services
{
    public class SalaService
    {
        private readonly ReunioesDbContext _context;

        public SalaService()
        {
            _context = new ReunioesDbContext();
        }

        public void Criar(Sala sala)
        {
            _context.Salas.Add(sala);
            _context.SaveChanges();
        }

        public Sala? ObterPorId(int id) => _context.Salas.Find(id);

        public void Atualizar(Sala sala)
        {
            _context.Salas.Update(sala);
            _context.SaveChanges();
        }

        public void Excluir(int id)
        {
            var sala = _context.Salas.Find(id);
            if (sala != null)
            {
                _context.Salas.Remove(sala);
                _context.SaveChanges();
            }
        }

        public List<Sala> ListarSalas(string? filtroNome, int pagina = 1)
        {
            var query = _context.Salas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtroNome))
            {
                query = query.Where(s => s.Nome.ToLower().Contains(filtroNome.ToLower()));
            }

            return query.OrderBy(s => s.Andar)
                        .Skip((pagina - 1) * 10)
                        .Take(10)
                        .ToList();
        }

        public double CalcularHorasLivresNoDia(int salaId, DateTime data)
        {
            DateTime dataInicioDia = new DateTime(data.Year, data.Month, data.Day, 8, 0, 0, DateTimeKind.Unspecified);
            DateTime dataFimDia = new DateTime(data.Year, data.Month, data.Day, 19, 0, 0, DateTimeKind.Unspecified);

            var reservasDoDia = _context.Reservas
                .Where(r => r.SalaId == salaId && r.Inicio >= dataInicioDia && r.Fim <= dataFimDia)
                .ToList();

            double totalHorasOcupadas = reservasDoDia.Sum(r => (r.Fim - r.Inicio).TotalHours);

            // O limite diário útil são 11 horas (das 08:00 às 19:00)
            double totalHorasComerciais = 11.0;

            double horasLivres = totalHorasComerciais - totalHorasOcupadas;
            return horasLivres < 0 ? 0 : horasLivres;
        }

        public double CalcularHorasLivres(int salaId, DateTime inicioPeriodo, DateTime fimPeriodo)
        {
            DateTime inicioSemFuso = new DateTime(inicioPeriodo.Ticks, DateTimeKind.Unspecified);
            DateTime fimSemFuso = new DateTime(fimPeriodo.Ticks, DateTimeKind.Unspecified);

            var reservas = _context.Reservas
                .Where(r => r.SalaId == salaId && r.Inicio >= inicioSemFuso && r.Fim <= fimSemFuso)
                .ToList();

            double totalHorasOcupadas = reservas.Sum(r => (r.Fim - r.Inicio).TotalHours);
            double totalHorasPeriodo = (fimSemFuso - inicioSemFuso).TotalHours;

            double horasLivres = totalHorasPeriodo - totalHorasOcupadas;
            return horasLivres < 0 ? 0 : horasLivres;
        }

        public int ObterTotalReunioesUltimos7Dias()
        {
            DateTime dataLimite = DateTime.Now.AddDays(-7);
            DateTime dataLimiteSemFuso = new DateTime(dataLimite.Ticks, DateTimeKind.Unspecified);

            return _context.Reservas.Count(r => r.Inicio >= dataLimiteSemFuso);
        }
    }
}