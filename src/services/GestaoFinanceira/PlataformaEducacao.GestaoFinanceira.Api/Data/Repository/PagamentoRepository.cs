

using Microsoft.EntityFrameworkCore;
using PlataformaEducacao.Core.Data;
using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Api.Data.Repository
{

    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly PagamentosContext _context;

        public PagamentoRepository(PagamentosContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public void AdicionarPagamento(Pagamento pagamento)
        {
            _context.Pagamentos.Add(pagamento);
        }

        public void AdicionarTransacao(Transacao transacao)
        {
            _context.Transacoes.Add(transacao);
        }

        public async Task<Pagamento> ObterPagamentoPorMatriculaId(Guid matriculaId)
        {
            return await _context.Pagamentos.AsNoTracking()
                .FirstOrDefaultAsync(p => p.MatriculaId == matriculaId);
        }

        public async Task<IEnumerable<Transacao>> ObterTransacaoesPorMatriculaId(Guid matriculaId)
        {
            return await _context.Transacoes.AsNoTracking()
                .Where(t => t.Pagamento.MatriculaId == matriculaId).ToListAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}