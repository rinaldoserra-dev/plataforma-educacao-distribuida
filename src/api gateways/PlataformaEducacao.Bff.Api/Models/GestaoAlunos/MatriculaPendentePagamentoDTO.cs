namespace PlataformaEducacao.Bff.Api.Models.GestaoAlunos
{
    public class MatriculaPendentePagamentoDTO
    {
        public Guid MatriculaId { get; set; }
        public Guid AlunoId { get; set; }
        public string NomeAluno { get; set; } = string.Empty;
        public Guid CursoId { get; set; }
        public string NomeCurso { get; set; } = string.Empty;
        public DateTime DataMatricula { get; set; }
    }
}