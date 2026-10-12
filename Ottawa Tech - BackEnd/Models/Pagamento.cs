namespace Ottawa_Tech___BackEnd.Models
{
    public class Pagamento
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public decimal Valor { get; set; } = decimal.Zero;
        public string Metodo { get; set; }
        public bool Status { get; set; }
        public DateTime DataPagamento { get; set; }
        public Reserva Reserva { get; set; }
    }
}