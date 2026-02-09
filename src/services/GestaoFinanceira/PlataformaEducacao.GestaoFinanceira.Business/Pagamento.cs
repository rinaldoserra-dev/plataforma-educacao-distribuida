using PlataformaEducacao.Core;
using PlataformaEducacao.Core.DomainObjects;


namespace PlataformaEducacao.GestaoFinanceira.Business
{
    public class Pagamento : Entity, IAggregateRoot
    {
        public Pagamento(Guid matriculaId, decimal valor, DadosCartao dadosCartao)
        {
            MatriculaId = matriculaId;
            Valor = valor;
            DadosCartao = dadosCartao;

            Validar();
        }
        protected Pagamento() { }

        public Guid MatriculaId { get; private set; }
        public decimal Valor { get; private set; }
        public DadosCartao DadosCartao { get; private set; } = null!;
        public StatusPagamento Status { get; private set; }
        public Transacao Transacao { get; set; } = null!;
        
        public void AprovarPagamento()
        {
            Status = StatusPagamento.Aprovado;            
        }
        public void RecusarPagamento()
        {
            Status = StatusPagamento.Recusado;            
        }

        protected void Validar()
        {
            Validacoes.ValidarSeMenorOuIgualQue(Valor, 0, "O valor do pagamento deve ser maior que 0.");
            Validacoes.ValidarSeVazio(MatriculaId, "A matricula não pode ser vazio");
        }
    }
}
