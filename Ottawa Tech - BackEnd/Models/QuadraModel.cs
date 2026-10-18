using Ottawa_Tech___BackEnd.Enums;

namespace Ottawa_Tech___BackEnd.Models
{
    public class QuadraModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Tipo { get; set; }
        public decimal ValorHora { get; set; }
        public StatusQuadraEnum Status { get; set; }
        public ICollection<ReservaModel> Reservas { get; set; } = [];
    }
}