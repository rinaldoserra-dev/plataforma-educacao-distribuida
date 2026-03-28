using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.PagamentoMatricula
{
    public class PagamentoMatriculaHandler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBus _bus;

        public PagamentoMatriculaHandler(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void SetSubscribers()
        {
            _bus.SubscribeAsync<MatriculaPagamentoRecusadoIntegrationEvent>("PagamentoRecusado", async request => await AtualiarMatricula(request));
            _bus.SubscribeAsync<MatriculaPagamentoRealizadoIntegrationEvent>("PagamentoRealizado", async request => await ConcluirMatricula(request));
        }

        private async Task AtualiarMatricula(MatriculaPagamentoRecusadoIntegrationEvent message)
        {
            var alunoRepository = ObterAlunoRepository();

            // Obtém o aluno com suas matrículas para verificar se a matrícula existe e pode ser concluída.
            var aluno = await alunoRepository.ObterComMatriculasPorId(message.AggregateId, CancellationToken.None);
            if (aluno is null) return;

            // Verifica se a matrícula existe para o aluno. Se não existir, não é possível concluir a matrícula.
            var matricula = aluno.Matriculas.FirstOrDefault(m => m.Id == message.MatriculaId);
            if (matricula is null) return;

            // Se a matrícula já estiver ativa, não pode ser recusada.
            if (matricula.SituacaoMatricula == SituacaoMatricula.Ativa) return;

            // Registra a recusa do pagamento da matrícula, o que deve atualizar o status da matrícula para ativa.
            aluno.RecusarPagamentoMatricula(matricula);

            // Atualiza o aluno no repositório para refletir as mudanças realizadas na matrícula.
            await alunoRepository.UnitOfWork.Commit();
        }

        private async Task ConcluirMatricula(MatriculaPagamentoRealizadoIntegrationEvent message)
        {
            var alunoRepository = ObterAlunoRepository();

            // Obtém o aluno com suas matrículas para verificar se a matrícula existe e pode ser concluída.
            var aluno = await alunoRepository.ObterComMatriculasPorId(message.AggregateId, CancellationToken.None);
            if (aluno is null) return;

            // Verifica se a matrícula existe para o aluno. Se não existir, não é possível concluir a matrícula.
            var matricula = aluno.Matriculas.FirstOrDefault(m => m.Id == message.MatriculaId);
            if (matricula is null) return;

            // Se a matrícula já estiver ativa, não é necessário realizar nenhuma ação.
            if (matricula.SituacaoMatricula == SituacaoMatricula.Ativa) return;

            // Registra o pagamento da matrícula, o que deve atualizar o status da matrícula para ativa.
            aluno.ConcluirPagamentoMatricula(matricula);

            // Atualiza o aluno no repositório para refletir as mudanças realizadas na matrícula.
            await alunoRepository.UnitOfWork.Commit();
        }

        private IAlunoRepository ObterAlunoRepository()
        {
            // Cria um escopo para resolver as dependências necessárias para concluir a matrícula.
            // Isso é importante para garantir que as dependências sejam resolvidas corretamente, especialmente em um ambiente de background service.
            using var scope = _serviceProvider.CreateScope();

            return scope.ServiceProvider.GetRequiredService<IAlunoRepository>();
        }
    }
}