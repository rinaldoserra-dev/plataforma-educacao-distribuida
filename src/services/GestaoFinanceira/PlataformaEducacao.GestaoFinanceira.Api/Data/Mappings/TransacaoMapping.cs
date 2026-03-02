using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaEducacao.GestaoFinanceira.Business.Models;

namespace PlataformaEducacao.GestaoFinanceira.Api.Data.Mappings
{
    public class TransacaoMapping : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.HasKey(c => c.Id);

            // 1 : N => Pagamento : Transacao
            builder.HasOne(c => c.Pagamento)
                .WithMany(c => c.Transacoes);

            builder.ToTable("Transacoes");
        }
    }
}
