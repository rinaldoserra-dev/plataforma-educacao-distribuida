namespace PlataformaEducacao.GestaoAluno.Application.Queries.ViewModels
{
    public class ArquivoViewModel
    {
        public byte[] PdfBytes { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public string NomeArquivo { get; set; } = null!;
    }
}
