using PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels;

namespace PlataformaEducacao.GestaoAluno.Application.Queries
{
    public interface IAlunoQueries
    {
        Task<MatriculaViewModel?> ObterMatricula(Guid matriculaId, CancellationToken cancellationToken);
        Task<IEnumerable<MatriculaViewModel>> ListarMatriculasPendentesPagamentoPorAlunoId(Guid alunoId, CancellationToken cancellationToken);
        Task<IEnumerable<MatriculaViewModel>> ObterMatriculasAtivasPorAlunoId(Guid alunoId, CancellationToken cancellationToken);
        Task<IEnumerable<MatriculaViewModel>> ObterAlunosMatriculadosPorCursoId(Guid cursoId, CancellationToken cancellationToken);
        Task<IEnumerable<MatriculaViewModel>> ObterAlunosPendentesPorCursoId(Guid cursoId, CancellationToken cancellationToken);
        Task<CertificadoViewModel?> ValidarCertificado(string codigoVerificacao, CancellationToken cancellationToken);
        Task<ArquivoViewModel?> DownloadCertificado(Guid certificadoId, CancellationToken cancellationToken);
        Task<HistoricoAlunoViewModel?> ObterHistoricoAluno(Guid alunoId, CancellationToken cancellationToken);
    }
}