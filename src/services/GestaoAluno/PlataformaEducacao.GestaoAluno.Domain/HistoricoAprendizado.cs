using PlataformaEducacao.Core;

namespace PlataformaEducacao.GestaoAluno.Domain
{
    public class HistoricoAprendizado
    {
        public int TotalAulasCurso { get; private set; }
        public double ProgressoGeralCurso { get; private set; }
        public SituacaoCurso SituacaoCurso { get; private set; }
        public DateTime? DataConclusao { get; private set; }

        protected HistoricoAprendizado() { }

        public static class HistoricoAprendizadoFactory
        {
            public static HistoricoAprendizado PendentePagamento(int totalAulasCurso)
            {
                var historico = new HistoricoAprendizado
                {
                    TotalAulasCurso = totalAulasCurso,
                    ProgressoGeralCurso = 0,
                    SituacaoCurso = SituacaoCurso.NaoIniciado,
                    DataConclusao = null
                };

                historico.Validar();
                return historico;
            }
            public static HistoricoAprendizado Progresso(int totalAulasCurso, double progressoGeral)
            {
                var historico = new HistoricoAprendizado
                {
                    TotalAulasCurso = totalAulasCurso,
                    ProgressoGeralCurso = progressoGeral,
                    SituacaoCurso = SituacaoCurso.EmAndamento,
                    DataConclusao = null
                };

                historico.Validar();
                return historico;
            }
            public static HistoricoAprendizado FinalizarCurso(int totalAulasCurso, double progressoGeral)
            {
                var historico = new HistoricoAprendizado
                {
                    TotalAulasCurso = totalAulasCurso,
                    ProgressoGeralCurso = progressoGeral,
                    SituacaoCurso = SituacaoCurso.Concluido,
                    DataConclusao = DateTime.Now
                };

                historico.Validar();
                return historico;
            }
        }

        protected void Validar()
        {
            Validacoes.ValidarSeMenorOuIgualQue(TotalAulasCurso, 0, "O número de aulas do curso deve ser maior que zero");
        }
    }
}
