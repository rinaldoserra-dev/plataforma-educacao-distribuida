using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Api.Services
{
    public interface IPagamentoService
    {
        Task<ResponseMessage> AutorizarPagamento(Pagamento pagamento);
        Task<ResponseMessage> CapturarPagamento(Guid pedidoId);
        Task<ResponseMessage> CancelarPagamento(Guid pedidoId);
    }
}
