using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Models.GestaoAlunos;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.WebApi.Core.Controllers;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [Authorize]
    [Route("alunos")]
    public class GestaoAlunosController(IAlunosService alunosService) : MainController
    {
        private readonly IAlunosService _alunosService = alunosService;

        [HttpPost("matricular")]
        public async Task<IActionResult> Matricular(MatricularDTO matricular)
        {
            if (ModelState.IsValid is false)
                return CustomResponse(ModelState);

            var resposta = await _alunosService.Matricular(matricular);

            return CustomResponse(resposta);
        }

        [HttpGet("matriculas-pendentes-pagamento")]
        public async Task<IActionResult> ObterMatriculasPendentesPagamento()
        {
            var matriculas = await _alunosService.ObterMatriculasPendentesPagamento();

            return matriculas is null ? NotFound() : CustomResponse(matriculas);
        }

        [HttpGet("matriculas-ativas")]
        public async Task<IActionResult> ObterMatriculasAtivas()
        {
            var matriculas = await _alunosService.ObterMatriculasAtivas();

            return matriculas is null ? NotFound() : CustomResponse(matriculas);
        }
    }
}