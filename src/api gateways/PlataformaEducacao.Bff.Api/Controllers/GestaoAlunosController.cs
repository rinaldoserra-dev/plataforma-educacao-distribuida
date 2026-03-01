using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Models.GestaoAlunos;
using PlataformaEducacao.Bff.Api.Services;
using PlataformaEducacao.WebApi.Core.Controllers;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [Authorize]
    [Route("alunos")]
    public class GestaoAlunosController : MainController
    {
        private readonly IAlunosService _alunosService;

        public GestaoAlunosController(IAlunosService alunosService)
        {
            _alunosService = alunosService;
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