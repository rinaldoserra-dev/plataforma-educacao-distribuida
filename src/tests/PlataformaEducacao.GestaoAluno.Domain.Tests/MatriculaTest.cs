using PlataformaEducacao.Core.DomainObjects;
using PlataformaEducacao.GestaoAluno.Domain.Events;

namespace PlataformaEducacao.GestaoAluno.Domain.Tests
{
    public class MatriculaTest
    {
        [Fact(DisplayName = "Criar matricula com dados válidos deve inicializar propriedades")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void CriarMatricula_ComDadosValidos_DeveInicializar()
        {
            // Arrange
            var cursoId = Guid.NewGuid();

            // Act
            var matricula = new Matricula(cursoId, nomeCurso: "Curso X", totalAulasCurso: 4, valor: 200m);

            // Assert
            Assert.Equal(cursoId, matricula.CursoId);
            Assert.Equal("Curso X", matricula.NomeCurso);
            Assert.Equal(200m, matricula.Valor);
            Assert.Equal(SituacaoMatricula.PendentePagamento, matricula.SituacaoMatricula);
            Assert.Empty(matricula.ProgressoAulas);
            Assert.Equal(4, matricula.HistoricoAprendizado.TotalAulasCurso);
        }

        [Fact(DisplayName = "Ativar matrícula quando não ativa deve alterar situação e adicionar evento")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void Ativar_NaoAtiva_DeveAtivarEAdicionarEvento()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 3, valor: 50m);

            // Act
            matricula.Ativar();

            // Assert
            Assert.Equal(SituacaoMatricula.Ativa, matricula.SituacaoMatricula);
            Assert.Contains(matricula.Notificacoes, e => e is MatriculaAtivadaEvent);
        }

        [Fact(DisplayName = "Ativar matrícula quando já ativa deve lançar DomainException")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void Ativar_QuandoJaAtiva_DeveLancarDomainException()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 3, valor: 50m);

            // Act
            matricula.Ativar();

            // Assert
            Assert.Throws<DomainException>(() => matricula.Ativar());
        }

        [Fact(DisplayName = "Iniciar pagamento deve alterar situacao para processo de pagamento")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void IniciarPagamento_DeveDefinirProcessoPagamento()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 3, valor: 50m);

            // Act
            matricula.IniciarPagamento();

            // Assert
            Assert.Equal(SituacaoMatricula.ProcessoPagamento, matricula.SituacaoMatricula);
        }

        [Fact(DisplayName = "Iniciar pagamento quando já em processo de pagamento deve lançar DomainException")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void IniciarPagamento_QuandoJaEmProcesso_DeveLancarDomainException()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 3, valor: 50m);

            // Act
            matricula.IniciarPagamento();

            // Assert
            Assert.Throws<DomainException>(() => matricula.IniciarPagamento());
        }

        [Fact(DisplayName = "Registrar aula antes de pagamento deve lançar DomainException")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void RegistrarAula_QuandoNaoAtiva_DeveLancarDomainException()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 2, valor: 50m);
            var progressoAula = new ProgressoAula(Guid.NewGuid());

            // Act & Assert
            Assert.Throws<DomainException>(() => matricula.RegistrarAula(progressoAula));
        }

        [Fact(DisplayName = "Registrar aula quando já realizada deve lançar DomainException")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void RegistrarAula_QuandoJaRealizada_DeveLancarDomainException()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 2, valor: 50m);
            matricula.Ativar();
            var progressoAula = new ProgressoAula(Guid.NewGuid());

            // Act
            matricula.RegistrarAula(progressoAula);

            // Assert
            Assert.Throws<DomainException>(() => matricula.RegistrarAula(new ProgressoAula(progressoAula.AulaId)));
        }

        [Fact(DisplayName = "Finalizar curso sem todas aulas assistidas deve lançar DomainException")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void FinalizarCurso_QuandoNemTodasAulasAssistidas_DeveLancarDomainException()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 3, valor: 50m);
            matricula.Ativar();
            matricula.RegistrarAula(new ProgressoAula(Guid.NewGuid()));

            // Act & Assert
            Assert.Throws<DomainException>(() => matricula.FinalizarCurso());
        }

        [Fact(DisplayName = "Finalizar curso quando todas aulas assistidas deve marcar concluído e adicionar evento")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void FinalizarCurso_QuandoTodasAulasAssistidas_DeveConcluirEAdicionarEvento()
        {
            // Arrange
            var matricula = new Matricula(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 2, valor: 50m);
            matricula.Ativar();

            var progressoAula1 = new ProgressoAula(Guid.NewGuid());
            var progressoAula2 = new ProgressoAula(Guid.NewGuid());

            matricula.RegistrarAula(progressoAula1);
            matricula.RegistrarAula(progressoAula2);

            // Act
            matricula.FinalizarCurso();

            // Assert
            Assert.Equal(SituacaoCurso.Concluido, matricula.HistoricoAprendizado.SituacaoCurso);
            Assert.Contains(matricula.Notificacoes, e => e is CursoFinalizadoEvent);
        }

        [Fact(DisplayName = "Gerar certificado quando curso concluído deve criar certificado")]
        [Trait("Categoria", "Gestao Aluno - Matricula")]
        public void GerarCertificado_QuandoConcluido_DeveCriarCertificado()
        {
            // Arrange
            var aluno = new Aluno(Guid.NewGuid(), nome: "Aluno", "aluno@exemplo.com");
            var matricula = Matricula.MatriculaFactory.CriarComCursoFinalizado(Guid.NewGuid(), nomeCurso: "Curso", totalAulasCurso: 1, valor: 10m, aluno);

            // Act
            matricula.GerarCertificado();

            // Assert
            Assert.NotNull(matricula.Certificado);
            Assert.Equal(matricula.Id, matricula.Certificado.MatriculaId);
        }
    }
}