using FluentValidation.Results;
using MediatR;
using PlataformaEducacao.Core.Messages;
using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Repositories;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.FinalizarCurso
{
    public class FinalizarCursoCommandHandler : CommandHandler,
        IRequestHandler<FinalizarCursoCommand, ValidationResult>
    {
        private readonly IAlunoRepository _alunoRepository;

        public FinalizarCursoCommandHandler(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }
        public async Task<ValidationResult> Handle(FinalizarCursoCommand message, CancellationToken cancellationToken)
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
            if (matricula.HistoricoAprendizado.ProgressoGeralCurso < 100)
            {
                AdicionarErro("Existem aulas pendentes de visualização.");
                return ValidationResult;
            }
            if (matricula.HistoricoAprendizado.SituacaoCurso == SituacaoCurso.Concluido)
            {
                AdicionarErro("Curso já finalizado.");
                return ValidationResult;
            }
            matricula.FinalizarCurso();

            return await PersistirDados(_alunoRepository.UnitOfWork);
        }
    }
}
