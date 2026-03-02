using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoFinanceira.Api.Services
{
    public class PagamentoIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public PagamentoIntegrationHandler(
                            IServiceProvider serviceProvider,
                            IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        private void SetResponder()
        {
            _bus.RespondAsync<IniciaPagamentoIntegrationEvent, ResponseMessage>(async request =>
                await AutorizarPagamento(request));
        }

        private void SetSubscribers()
        {
            _bus.SubscribeAsync<MatriculaPagamentoRecusadoIntegrationEvent>("PedidoCancelado", async request =>
            await CancelarPagamento(request));

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetResponder();
            SetSubscribers();
            return Task.CompletedTask;
        }

        private async Task<ResponseMessage> AutorizarPagamento(IniciaPagamentoIntegrationEvent message)
        {
            using var scope = _serviceProvider.CreateScope();
            var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();
            var pagamento = new Pagamento
            {
                MatriculaId = message.MatriculaId,
                TipoPagamento = (TipoPagamento) message.TipoPagamento,
                Valor = message.Valor,
                DadosCartao = new DadosCartao(
                    message.NomeCartao, message.NumeroCartao, message.ExpiracaoCartao, message.CvvCartao)
            };

            var response = await pagamentoService.AutorizarPagamento(pagamento);

            return response;
        }

        private async Task CancelarPagamento(MatriculaPagamentoRecusadoIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var pagamentoService = scope.ServiceProvider.GetRequiredService<IPagamentoService>();

                var response = await pagamentoService.CancelarPagamento(message.MatriculaId);

                if (!response.ValidationResult.IsValid)
                    throw new DomainException($"Falha ao cancelar pagamento da matricula {message.MatriculaId}");
            }
        }

     
}
}
