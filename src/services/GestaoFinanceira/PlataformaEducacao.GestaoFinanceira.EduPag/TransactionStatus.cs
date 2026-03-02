using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlataformaEducacao.GestaoFinanceira.EduPag
{
    public enum TransactionStatus
    {
        Authorized = 1,
        Paid,
        Refused,
        Chargedback,
        Cancelled
    }
}
