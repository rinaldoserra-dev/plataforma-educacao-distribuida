using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacao.Bff.Api.Models.GestaoAlunos
{
    public class MatricularDTO
    {
        [Required(ErrorMessage = "O id do curso é obrigatório.")]
        public Guid CursoId { get; set; }

        [Required(ErrorMessage = "O nome do curso é obrigatório.")]
        public string NomeCurso { get; set; } = string.Empty;

        [Required(ErrorMessage = "A quantidade de aulas do curso é obrigatória.")]
        public int TotalAulasCurso { get; set; }

        [Required(ErrorMessage = "O valor do curso é obrigatório.")]
        public decimal Valor { get; set; }
    }
}