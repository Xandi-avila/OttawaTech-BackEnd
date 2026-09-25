using Microsoft.VisualBasic;

namespace Ottawa_Tech___BackEnd.Classes
{
    public class Reserva
    {
        public int id { get; set; }
        public DateTime? DataReserva { get; set; }
        public decimal ValorQuadra { get; set; } = 0;
    }
}