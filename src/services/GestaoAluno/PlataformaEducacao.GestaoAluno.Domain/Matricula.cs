using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.GestaoAluno.Domain.Events;

namespace PlataformaEducacao.GestaoAluno.Domain
{
    public class Matricula : Entity
    {
        public Guid CursoId { get; private set; }
        public string NomeCurso { get; private set; } = null!;
        public decimal Valor { get; private set; }
        public Guid AlunoId { get; private set; }
        public DateTime DataMatricula { get; private set; }
        public SituacaoMatricula SituacaoMatricula { get; private set; }
        public HistoricoAprendizado HistoricoAprendizado { get; private set; } = null!;
        public Aluno Aluno { get; private set; } = null!;
        public Certificado? Certificado { get; private set; }

        private readonly List<ProgressoAula> _progressoAulas;
        public IReadOnlyCollection<ProgressoAula> ProgressoAulas => _progressoAulas;
        protected Matricula()
        {
            _progressoAulas = new List<ProgressoAula>();
        }
        public Matricula(Guid cursoId, string nomeCurso, int totalAulasCurso, decimal valor)
        {
            CursoId = cursoId;
            NomeCurso = nomeCurso;
            Valor = valor;
            DataMatricula = DateTime.Now;
            SituacaoMatricula = SituacaoMatricula.PendentePagamento;
            HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.PendentePagamento(totalAulasCurso);
            Certificado = null;
            _progressoAulas = new List<ProgressoAula>();

            Validar();
        }

        public void AssociarAluno(Guid alunoId)
        {
            AlunoId = alunoId;
        }

        public bool EstaAtiva()
        {
            return SituacaoMatricula == SituacaoMatricula.Ativa;
        }

        public void Ativar()
        {
            if (EstaAtiva())
            {
                throw new DomainException("Matrícula já ativa.");
            }
            SituacaoMatricula = SituacaoMatricula.Ativa;
            AdicionarEvento(new MatriculaAtivadaEvent(Id));
        }

        public void Desativar()
        {
            SituacaoMatricula = SituacaoMatricula.PendentePagamento;
        }

        public void IniciarPagamento()
        {
            if (SituacaoMatricula == SituacaoMatricula.Ativa)
            {
                throw new DomainException("Pagamento já realizado.");
            }
            if (SituacaoMatricula == SituacaoMatricula.ProcessoPagamento)
            {
                throw new DomainException("Pagamento em processo de pagamento.");
            }
            SituacaoMatricula = SituacaoMatricula.ProcessoPagamento;
        }

        public bool AulaRealizada(ProgressoAula progressoAula)
        {
            return _progressoAulas.Any(p => p.AulaId == progressoAula.AulaId);
        }

        public void RegistrarAula(ProgressoAula progressoAula)
        {
            if (!EstaAtiva())
            {
                throw new DomainException("Matrícula pendente de pagamento.");
            }

            if (AulaRealizada(progressoAula))
            {
                throw new DomainException("Aula já registrada no progresso.");
            }
            progressoAula.AssociarMatricula(Id);
            _progressoAulas.Add(progressoAula);

            AtualizaProgressoCurso();
        }

        private void AtualizaProgressoCurso()
        {
            var totalAulasCurso = HistoricoAprendizado.TotalAulasCurso;
            int aulasConcluidas = _progressoAulas.Count;

            double novoProgresso = (double)aulasConcluidas / totalAulasCurso * 100;

            HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.Progresso(totalAulasCurso, novoProgresso);
        }

        public void FinalizarCurso()
        {
            if (!EstaAtiva())
            {
                throw new DomainException("Matrícula pendente de pagamento.");
            }
            int aulasConcluidas = _progressoAulas.Count;
            int totalAulasCurso = HistoricoAprendizado.TotalAulasCurso;

            if (HistoricoAprendizado.ProgressoGeralCurso < 100 || aulasConcluidas != totalAulasCurso)
            {
                throw new DomainException("Existem aulas pendentes de visualização.");
            }
            if (HistoricoAprendizado.SituacaoCurso == SituacaoCurso.Concluido)
            {
                throw new DomainException("Curso já finalizado");
            }
            
            double progresso = HistoricoAprendizado.ProgressoGeralCurso;

            HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.FinalizarCurso(totalAulasCurso, progresso);
            AdicionarEvento(new CursoFinalizadoEvent(Id));
        }

        public void GerarCertificado()
        {
            if (HistoricoAprendizado.SituacaoCurso != SituacaoCurso.Concluido)
            {
                throw new DomainException("O curso deve estar concluído para gerar o certificado.");
            }
            Certificado = new Certificado(Id);            
        }

        protected void Validar()
        {
            Validacoes.ValidarSeVazio(CursoId, "O id do curso é obrigatório.");
            Validacoes.ValidarSeVazio(NomeCurso, "O nome do curso é obrigatório.");
            Validacoes.ValidarSeMenorOuIgualQue(Valor, 0, "O Valor do curso deve ser maior que 0.");
        }

        public static class MatriculaFactory
        {
            public static Matricula GerarMatriculaPagamentoAprovado(Guid cursoId, string nomeCurso, int totalAulasCurso, decimal valor, Aluno aluno)
            {
                var matricula = new Matricula
                {
                    Aluno = aluno,
                    AlunoId = aluno.Id,
                    NomeCurso = nomeCurso,
                    CursoId = cursoId,
                    HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.Progresso(totalAulasCurso, 0),
                    SituacaoMatricula = SituacaoMatricula.Ativa,
                    DataMatricula = DateTime.Now,
                    Valor = valor
                };

                matricula.Validar();

                return matricula;
            }

            public static Matricula GerarMatriculaCursoFinalizado(Guid cursoId, string nomeCurso, int totalAulasCurso, decimal valor, Aluno aluno)
            {
                var matricula = new Matricula
                {
                    Aluno = aluno,
                    AlunoId = aluno.Id,
                    NomeCurso = nomeCurso,
                    CursoId = cursoId,
                    HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.Progresso(totalAulasCurso, 100),
                    SituacaoMatricula = SituacaoMatricula.Ativa,
                    DataMatricula = DateTime.Now,                    
                    Valor = valor
                };

                matricula.Validar();
                matricula.RegistrarAula(new ProgressoAula(Guid.NewGuid()));
                matricula.FinalizarCurso();

                return matricula;
            }

            public static Matricula GerarMatriculaPagamentoProcessamento(Guid cursoId, string nomeCurso, int totalAulasCurso, decimal valor, Aluno aluno)
            {
                var matricula = new Matricula
                {
                    Aluno = aluno,
                    AlunoId = aluno.Id,
                    NomeCurso = nomeCurso,
                    CursoId = cursoId,
                    HistoricoAprendizado = HistoricoAprendizado.HistoricoAprendizadoFactory.Progresso(totalAulasCurso, 0),
                    SituacaoMatricula = SituacaoMatricula.ProcessoPagamento,
                    DataMatricula = DateTime.Now,
                    Valor = valor
                };

                matricula.Validar();

                return matricula;
            }
        }
    }
}