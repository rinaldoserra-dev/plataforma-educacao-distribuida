using Microsoft.Extensions.Options;
using PlataformaEducacao.Bff.Api.Extensions;
using PlataformaEducacao.Core.Communication;

namespace PlataformaEducacao.Bff.Api.Services
{
    public interface IAlunosService
    {
        Task<ResponseResult> Matricular(MatricularDTO matricular);

    }
    public class AlunosService : Service, IAlunosService
    {
        private readonly HttpClient _httpClient;

        public AlunosService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.GestaoConteudoUrl);
        }

        public async Task<ResponseResult> Matricular(MatricularDTO matricular)
        {
        }
    }
}
