
using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain
{
    public class Aluno : Entity, IAggregateRoot
    {
        public string Nome { get; private set; } = null!;
        public Email Email { get; private set; } = null!;

        private readonly List<Matricula> _matriculas;
        public IReadOnlyCollection<Matricula> Matriculas => _matriculas;

        protected Aluno()
        {
            _matriculas = [];
        }

        public Aluno(Guid alunoId, string nome, string email)
        {
            Id = alunoId;
            Nome = nome;
            Email = new Email(email);
            _matriculas = [];

            Validar();
        }

        protected void Validar()
        {
            Validacoes.ValidarSeVazio(Id, "O aluno id é obrigatório.");
            Validacoes.ValidarSeVazio(Nome, "O nome do aluno é obrigatório.");
        }

        public void RealizarMatricula(Matricula matricula)
        {
            // Verifica se o aluno já possui uma matrícula para o mesmo curso
            if (MatriculaExistente(matricula))
                throw new DomainException("Aluno já matriculado no curso.");

            // Associa a matrícula ao aluno
            matricula.AssociarAluno(Id);

            // Inicia o processo de pagamento da matrícula
            matricula.IniciarPagamento();

            // Adiciona a matrícula à lista de matrículas do aluno
            _matriculas.Add(matricula);
        }

        public bool MatriculaExistente(Matricula matricula)
        {
            return _matriculas.Any(p => p.CursoId == matricula.CursoId);
        }

        public void RecusarPagamentoMatricula(Matricula matricula)
        {
            if (MatriculaExistente(matricula) is false)
                throw new DomainException("Matricula não encontrada para o aluno.");

            matricula.RecusarPagamento();
        }

        public void ConcluirPagamentoMatricula(Matricula matricula)
        {
            if (MatriculaExistente(matricula) is false)
                throw new DomainException("Matricula não encontrada para o aluno.");

            matricula.ConcluirPagamento();
        }
    }
}