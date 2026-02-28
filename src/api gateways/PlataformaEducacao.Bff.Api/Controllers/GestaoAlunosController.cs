using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Bff.Api.Services;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [Route("alunos")]
    [Authorize]
    public class GestaoAlunosController : BaseController
    {
        private readonly IAlunosService _alunosService;

        public GestaoAlunosController(IAlunosService alunosService)
        {
            _alunosService = alunosService;
        }



    }
}