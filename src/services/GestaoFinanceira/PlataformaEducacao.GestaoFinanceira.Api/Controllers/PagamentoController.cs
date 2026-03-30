using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.Core.Messages.Integration;
using PlataformaEducacao.GestaoFinanceira.Api.Data;
using PlataformaEducacao.GestaoFinanceira.Api.Models.Requests;
using PlataformaEducacao.GestaoFinanceira.Api.Models.Response;
using PlataformaEducacao.GestaoFinanceira.Api.Services;
using PlataformaEducacao.GestaoFinanceira.Business.Models;
using PlataformaEducacao.MessageBus;
using PlataformaEducacao.WebApi.Core.Controllers;
using PlataformaEducacao.WebApi.Core.Usuario;
using System.Net;
using System.Threading;


namespace PlataformaEducacao.GestaoFinanceira.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : MainController
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAspNetUser _user;

        public PagamentoController( IPagamentoService pagamentoService, IServiceProvider serviceProvider, IAspNetUser user)
        {
            _pagamentoService = pagamentoService;
            _serviceProvider = serviceProvider;
            _user = user;
        }

        [HttpPost("pagar")]
        [Authorize(Roles = "ALUNO")]
        public async Task<IActionResult> PagarMatricula([FromBody] PagarMatriculaRequest dadosPagamento, CancellationToken cancellationToken)
        {
            var pagamento = new Pagamento
            {
                AlunoId = _user.ObterUserId(),
                MatriculaId = dadosPagamento.MatriculaId,
                TipoPagamento = TipoPagamento.CartaoCredito,
                Valor = dadosPagamento.Valor,
                DadosCartao = new DadosCartao(dadosPagamento.NomeCartao, dadosPagamento.NumeroCartao, dadosPagamento.ExpiracaoCartao, dadosPagamento.CvvCartao)
            };

            var result = await _pagamentoService.AutorizarPagamento(pagamento, cancellationToken);

            if (!result.ValidationResult.IsValid)
            {
                foreach (var error in result.ValidationResult.Errors)
                    AdicionarErroProcessamento(error.ErrorMessage);

                return CustomResponse();
            }

            return CustomResponse(HttpStatusCode.OK, "Pagamento autorizado com sucesso");

        }


        [HttpGet("{matriculaId:guid}/status")]
        [Authorize(Roles = "ALUNO")]
        public async Task<IActionResult> ObterStatus(Guid matriculaId, CancellationToken cancellationToken)
        {
            var usuarioId = _user.ObterUserId();
            var result = await _pagamentoService.ObterStatusPorMatricula(matriculaId, usuarioId);


            if (result == null)
                return NotFound("Pagamento não encontrado para esta matrícula.");



            return Ok(result);
        }


    }
}
