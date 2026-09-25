namespace Ottawa_Tech___BackEnd.Models
{
    public class Reserva
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int QuadraId { get; set; }
        public DateOnly Data { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFim { get; set; }
        public decimal Valor { get; set; }
        public bool Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public Cliente Cliente { get; set; }
        public Quadra Quadra { get; set; }
        public Pagamento? Pagamento { get; set; }
    }
}