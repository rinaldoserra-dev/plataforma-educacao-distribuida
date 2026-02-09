
using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoAluno.Domain
{
    public class Aluno : Entity, IAggregateRoot
    {
        public string Nome { get; private set; } = null!;
        public Email Email { get; private set; }

        private readonly List<Matricula> _matriculas;
        public IReadOnlyCollection<Matricula> Matriculas => _matriculas;

        protected Aluno()
        {
            _matriculas = new List<Matricula>();
        }

        public Aluno(Guid alunoId, string nome, string email)
        {
            Id = alunoId;
            Nome = nome;
            Email = new Email(email);
            _matriculas = new List<Matricula>();

            Validar();
        }

        public bool MatriculaExistente(Matricula matricula)
        {
            return _matriculas.Any(p => p.CursoId == matricula.CursoId);
        }

        public void RealizarMatricula(Matricula matricula)
        {
            if (MatriculaExistente(matricula))
            {
                throw new DomainException("Aluno já matriculado no curso.");
            }
            matricula.AssociarAluno(Id);
            _matriculas.Add(matricula);
        }

        public void RealizarPagamentoMatricula(Matricula matricula)
        {
            matricula.IniciarPagamento();
        }

        protected void Validar()
        {
            Validacoes.ValidarSeVazio(Id, "O aluno id é obrigatório.");
            Validacoes.ValidarSeVazio(Nome, "O nome do aluno é obrigatório.");
        }
    }
}
