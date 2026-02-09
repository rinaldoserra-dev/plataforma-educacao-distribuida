using PlataformaEducacao.GestaoAluno.Domain;

namespace PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels
{
    public class CertificadoViewModel
    {
        public CertificadoViewModel(Guid certificadoId, string nomeAluno, string nomeCurso, DateTime dataConclusao, string codigoVerificacao)
        {
            CertificadoId = certificadoId;
            NomeAluno = nomeAluno;
            NomeCurso = nomeCurso;
            DataConclusao = dataConclusao;
            CodigoVerificacao = codigoVerificacao;
        }
        public Guid CertificadoId { get; set; }
        public string NomeAluno { get; set; } = null!;
        public string NomeCurso { get; set; } = null!;
        public DateTime DataConclusao { get; set; }
        public string CodigoVerificacao { get; set; } = null!;

        public static CertificadoViewModel FromMatricula(Matricula matricula)
         => new(
             matricula.Certificado!.Id,
             matricula.Aluno?.Nome ?? string.Empty,
             matricula.NomeCurso,
             matricula.HistoricoAprendizado.DataConclusao!.Value,
             matricula.Certificado!.CodigoVerificacao
         );
    }
}
