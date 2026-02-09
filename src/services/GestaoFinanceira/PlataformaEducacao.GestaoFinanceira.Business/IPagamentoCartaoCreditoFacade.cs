namespace PlataformaEducacao.GestaoFinanceira.Business
{
    public interface IPagamentoCartaoCreditoFacade
    {
        Transacao RealizarPagamento(Guid matriculaId, Pagamento pagamento);
    }
}
