using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faturamento.Infrastructure.Persistence.Configurations;

public class NotaFiscalItemConfiguration : IEntityTypeConfiguration<NotaFiscalItem>
{
    public void Configure(EntityTypeBuilder<NotaFiscalItem> builder)
    {
        builder.ToTable("NotaFiscalItens");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.ProdutoId)
            .IsRequired();

        builder.Property(i => i.Quantidade)
            .IsRequired();
    }
}
