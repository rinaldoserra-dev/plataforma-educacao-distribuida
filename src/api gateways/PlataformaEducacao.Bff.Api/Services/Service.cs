using PlataformaEducacao.Core.Communication;
using System.Text;
using System.Text.Json;

namespace PlataformaEducacao.Bff.Api.Services
{
    public abstract class Service
    {
        protected StringContent ObterConteudo(object dado)
        {
            return new StringContent(
                JsonSerializer.Serialize(dado),
                Encoding.UTF8,
                "application/json");
        }
        protected async Task<ResponseResult> DeserializarObjetoResponse(HttpResponseMessage responseMessage)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var stringContent = await responseMessage.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(stringContent))
            {
                try
                {
                    var result = JsonSerializer.Deserialize<ResponseResult>(stringContent, options);
                    if (result != null)
                    {
                        result.Status = (int)responseMessage.StatusCode;
                        return result;
                    }
                }
                catch (JsonException) {  }
            }
            
            return new ResponseResult
            {
                Status = (int)responseMessage.StatusCode,
                Sucesso = responseMessage.IsSuccessStatusCode,
                Erros = new ResponseErrorMessages
                {
                    Mensagens = new List<string> { responseMessage.ReasonPhrase ?? "Erro na comunicação com a API." }
                }
            };
        }

        //protected async Task<T> DeserializarObjetoResponse<T>(HttpResponseMessage responseMessage)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //    return JsonSerializer.Deserialize<T>(await responseMessage.Content.ReadAsStringAsync(), options);
        //}

        //protected bool TratarErrosResponse(HttpResponseMessage response)
        //{
        //    if (response.StatusCode == HttpStatusCode.BadRequest) return false;

        //    response.EnsureSuccessStatusCode();
        //    return true;
        //}
    }
}
