using System;

namespace Reunioes.Core.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
        public int SalaId { get; set; }
        public Sala Sala { get; set; } = null!;
    }
}