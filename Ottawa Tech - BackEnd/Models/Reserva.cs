using Microsoft.VisualBasic;

namespace Ottawa_Tech___BackEnd.Classes
{
    public class Reserva
    {
        public int id { get; set; }
        public int ClienteID { get; set; }
        public DateOnly Data { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFim { get; set; }
        public decimal Valor { get; set; }
        public bool Status { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}