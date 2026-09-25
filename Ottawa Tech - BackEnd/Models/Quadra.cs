namespace Ottawa_Tech___BackEnd.Models
{
    public class Quadra
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public decimal ValorHora { get; set; }
        public bool Status { get; set; }
        public ICollection<Reserva> Reservas { get; set; } = [];
    }
}