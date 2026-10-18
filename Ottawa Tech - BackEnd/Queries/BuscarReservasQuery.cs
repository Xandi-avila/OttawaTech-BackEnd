using Ottawa_Tech___BackEnd.Enums;

namespace Ottawa_Tech___BackEnd.Queries
{
    public class BuscarReservasQuery
    {
        // Filtros para buscar
        public string? Nome { get; set; }

        public string? Tipo { get; set; }
        public StatusQuadraEnum? Status { get; set; }
        public decimal? ValorMaximo { get; set; }

        // Paginação
        public int Pagina { get; set; } = 1;

        public int PorPagina { get; set; } = 10;

        // Filtro de datas
        public DateTime? DataInicio { get; set; }

        public DateTime? DataFim { get; set; }
    }
}