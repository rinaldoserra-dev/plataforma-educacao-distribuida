using Microsoft.AspNetCore.Mvc;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected ActionResult CustomResponse(ResponseResult response)
        {
            if (response == null || !response.Sucesso || response.Erros.Mensagens.Any())
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
