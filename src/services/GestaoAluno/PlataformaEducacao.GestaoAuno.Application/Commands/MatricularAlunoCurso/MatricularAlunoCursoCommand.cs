using FluentValidation;
using PlataformaEducacao.Core.Messages;

namespace PlataformaEducacao.GestaoAluno.Application.Commands.MatricularAlunoCurso
{
    public class MatricularAlunoCursoCommand : Command
    {
        public MatricularAlunoCursoCommand(Guid cursoId, Guid alunoId, string nomeCurso, int totalAulasCurso, decimal valor)
        {
            CursoId = cursoId;
            AlunoId = alunoId;
            NomeCurso = nomeCurso;
            TotalAulasCurso = totalAulasCurso;
            Valor = valor;
        }

        public Guid CursoId { get; private set; }

        public Guid AlunoId { get; private set; }

        public string NomeCurso { get; private set; }
        public decimal Valor { get; private set; }

        public int TotalAulasCurso { get; private set; }

        public override bool EhValido()
        {
            ValidationResult = new MatricularAlunoCursoCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
    public class MatricularAlunoCursoCommandValidation : AbstractValidator<MatricularAlunoCursoCommand>
    {
        public MatricularAlunoCursoCommandValidation()
        {
            RuleFor(a => a.AlunoId)
                .NotEmpty()
                .WithMessage("Aluno é obrigatório");

            RuleFor(a => a.CursoId)
                .NotEmpty()
                .WithMessage("Curso é obrigatório");

            RuleFor(a => a.NomeCurso)
                .NotEmpty()
                .WithMessage("Nome do curso é obrigatório");

            RuleFor(c => c.Valor)
             .GreaterThan(0)
             .WithMessage("O valor do curso deve ser maior que 0.");

            RuleFor(a => a.TotalAulasCurso)
                .GreaterThan(0)
                .WithMessage("O número de aulas do curso é obrigatório");
        }
    }
}
