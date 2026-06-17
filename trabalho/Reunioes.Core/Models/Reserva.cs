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
        public bool TemHorariosValidos()
        {
            return Inicio < Fim;
        }
        public bool EstaNoHorarioComercial()
        {
            if (Inicio.Hour < 8 || Fim.Hour > 19 || (Fim.Hour == 19 && Fim.Minute > 0))
            {
                return false;
            }
            return true;
        }
    }
}