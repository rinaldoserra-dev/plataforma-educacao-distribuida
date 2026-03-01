using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Bff.Api.Models.GestaoAlunos;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Services
{
    public interface IAlunosService
    {
        Task<ResponseResult> Matricular(MatricularDTO solicitarMatricula);

    }
    public class AlunosService : Service, IAlunosService
    {
        private readonly HttpClient _httpClient;

        public AlunosService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.GestaoAlunosUrl);
        }

        public async Task<ResponseResult> Matricular(MatricularDTO matricular)
        {
            var conteudoMatricular = ObterConteudo(matricular);

            var response = await _httpClient.PostAsync("/api/alunos/matricular", conteudoMatricular);

            return await DeserializarObjetoResponse(response);
        }
    }
}