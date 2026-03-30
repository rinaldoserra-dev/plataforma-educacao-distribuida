using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.GestaoFinanceira.Api.Models.Requests
{
    public class PagarMatriculaRequest
    {
        [Required]
        public Guid MatriculaId { get; set; }
        [Range(0.01, 9999999)]  
        public decimal Valor { get; set; }
        [Required]
        [StringLength(150)]
        public string NomeCartao { get; set; }
        [Required]
        [StringLength(19)]
        public string NumeroCartao { get; set; }
        [Required]
        [StringLength(7)]
        public string ExpiracaoCartao { get; set; }
        [Required]
        [StringLength(4)]
        public string CvvCartao { get; set; }
    }
}
