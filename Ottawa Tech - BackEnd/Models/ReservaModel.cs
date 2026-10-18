using Ottawa_Tech___BackEnd.Enums;

namespace Ottawa_Tech___BackEnd.Models

{
    public class ReservaModel
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int QuadraId { get; set; }
        public DateOnly Data { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFim { get; set; }
        public decimal Valor { get; set; }
        public StatusReservaEnum Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public ClienteModel Cliente { get; set; }
        public QuadraModel Quadra { get; set; }
        public PagamentoModel? Pagamento { get; set; }
    }
}