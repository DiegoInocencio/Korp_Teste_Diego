using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Faturamento.Infrastructure.Persistence.Configurations;

public class NotaFiscalConfiguration : IEntityTypeConfiguration<NotaFiscal>
{
    public void Configure(EntityTypeBuilder<NotaFiscal> builder)
    {
        builder.ToTable("NotasFiscais");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
            .ValueGeneratedNever();

        builder.Property(n => n.Status)
            .IsRequired();

        builder.Property(n => n.Numero)
            .UseIdentityByDefaultColumn()
            .ValueGeneratedOnAdd();

        builder.HasIndex(n => n.Numero)
            .IsUnique();

        builder.Metadata.FindNavigation(nameof(NotaFiscal.Itens))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(n => n.Itens)
            .WithOne(i => i.NotaFiscal)
            .HasForeignKey(i => i.NotaFiscalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
