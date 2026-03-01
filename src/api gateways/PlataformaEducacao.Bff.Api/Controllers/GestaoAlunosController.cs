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

        [HttpGet("matriculas-pendentes-pagamento")]
        public async Task<ActionResult<IEnumerable<MatriculaPendentePagamentoDTO>>> ObterMatriculasPendentesPagamento()
        {
            var matriculas = await _alunosService.ObterMatriculasPendentesPagamento();

            return matriculas is null ? NotFound() : CustomResponse(matriculas);
        }

        [HttpPost("matricular")]
        public async Task<IActionResult> Matricular(MatricularDTO matricular)
        {
            if (ModelState.IsValid is false)
                return CustomResponse(ModelState);

            var resposta = await _alunosService.Matricular(matricular);

            return CustomResponse(resposta);
        }
    }
}