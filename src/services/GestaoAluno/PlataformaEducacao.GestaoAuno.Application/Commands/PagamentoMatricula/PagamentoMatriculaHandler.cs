using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;
using PlataformaEducacao.MessageBus;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.PagamentoMatricula
{
    public class PagamentoMatriculaHandler : CommandHandler,
       IRequestHandler<PagamentoMatriculaCommand, ValidationResult>
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IMessageBus _bus;

        public PagamentoMatriculaHandler(IAlunoRepository alunoRepository,
                                         IMessageBus bus)
        {
            _alunoRepository = alunoRepository;
            _bus = bus;
        }

        public async Task<ValidationResult> Handle(PagamentoMatriculaCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;

            var aluno = await _alunoRepository.ObterComMatriculasPorId(message.AlunoId, cancellationToken);
            if (aluno is null)
            {
                AdicionarErro("Aluno não encontrado!");
                return ValidationResult;
            }
            var matricula = aluno.Matriculas.FirstOrDefault(m => m.Id == message.MatriculaId);
            if (matricula is null)
            {
                AdicionarErro("Matricula não encontrada!");
                return ValidationResult;
            }
            if (matricula.SituacaoMatricula == SituacaoMatricula.ProcessoPagamento)
            {
                AdicionarErro("A matrícula já está em processo de pagamento. Aguarde a confirmação.");
                return ValidationResult;
            }
            if (matricula.EstaAtiva())
            {
                AdicionarErro("Já foi realizada o pagamento da matrícula!");
                return ValidationResult;
            }
            if (matricula.Valor != message.Total)
            {
                AdicionarErro("Valor do curso inválido!");
                return ValidationResult;
            }

            aluno.RealizarPagamentoMatricula(matricula);

            int pagamentoPorBoleto = 1;

            var pagamentoIniciado = new IniciaPagamentoIntegrationEvent(matricula.Id, matricula.CursoId, 
                matricula.AlunoId, message.Total, pagamentoPorBoleto, message.NomeCartao, message.NumeroCartao, 
                message.ExpiracaoCartao, message.CvvCartao);

            await _bus.PublishAsync(pagamentoIniciado);

            return await PersistirDados(_alunoRepository.UnitOfWork);
        }
    }
}