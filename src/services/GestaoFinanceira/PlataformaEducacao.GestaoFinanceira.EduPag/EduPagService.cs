using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaEducacao.GestaoFinanceira.EduPag
{
    public class EduPagService
    {
        public readonly string ApiKey;
        public readonly string EncryptionKey;

        public EduPagService(string apiKey, string encryptionKey)
        {
            ApiKey = apiKey;
            EncryptionKey = encryptionKey;
        }
    }
}
