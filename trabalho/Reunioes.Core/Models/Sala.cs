using System.Collections.Generic;

namespace Reunioes.Core.Models
{
    public class Sala
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Andar { get; set; }
        public int QuantidadeAssentos { get; set; }
        public List<Reserva> Reservas { get; set; } = new();
    }
}