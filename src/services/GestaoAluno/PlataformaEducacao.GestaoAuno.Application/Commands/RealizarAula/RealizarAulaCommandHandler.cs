using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.RealizarAula
{
    public class RealizarAulaCommandHandler: CommandHandler,
        IRequestHandler<RealizarAulaCommand, ValidationResult>
    {
        private readonly IAlunoRepository _alunoRepository;

        public RealizarAulaCommandHandler(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public async Task<ValidationResult> Handle(RealizarAulaCommand message, CancellationToken cancellationToken)
        {
            if (!message.EhValido()) return message.ValidationResult;
            var matricula = await _alunoRepository.ObterMatriculaComProgressoAulasPorId(message.MatriculaId, cancellationToken);
            if (matricula is null)
            {
                AdicionarErro("Matricula não encontrada!");
                return ValidationResult;
            }

            if (!matricula.EstaAtiva())
            {
                AdicionarErro("Matricula pendente de pagamento!");
                return ValidationResult;
            }
            var progressoAula = new ProgressoAula(message.AulaId);
            if (matricula.AulaRealizada(progressoAula))
            {
                AdicionarErro("Aula já realizada!");
                return ValidationResult;
            }
            matricula.RegistrarAula(progressoAula);

            await _alunoRepository.AtualizarProgressoAula(progressoAula, cancellationToken);
            await _alunoRepository.AtualizarMatricula(matricula, cancellationToken);

            return await PersistirDados(_alunoRepository.UnitOfWork);
        }
    }
}
