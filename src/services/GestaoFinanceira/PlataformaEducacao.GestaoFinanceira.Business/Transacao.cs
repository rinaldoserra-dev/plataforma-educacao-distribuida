using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;

namespace PlataformaEducacao.GestaoFinanceira.Business
{
    public class Transacao : Entity
    {
        public Transacao(Guid pagamentoId, decimal total)
        {
            PagamentoId = pagamentoId;
            Total = total;

            Validar();
        }

        public Guid PagamentoId { get; private set; }
        public decimal Total { get; private set; }
        public StatusTransacao StatusTransacao { get; private set; }
        public Pagamento Pagamento { get; private set; } = null!;

        public void AlterarStatus(StatusTransacao statusTransacao)
        {
            StatusTransacao = statusTransacao;
        }

        protected void Validar()
        {
            Validacoes.ValidarSeMenorOuIgualQue(Total, 0, "O valor do pagamento deve ser maior que 0.");
            Validacoes.ValidarSeVazio(PagamentoId, "O pagamento não pode ser vazio.");
        }
    }
}
