using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImobAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImobAPI.Context
{
    public class ImobContext : DbContext
    {
        public ImobContext(DbContextOptions<ImobContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoUsuario> TiposUsuario { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<TipoCliente> TiposCliente { get; set; }
        public DbSet<TipoImovel> TiposImovel { get; set; }
        public DbSet<Intencao> Intencoes { get; set; }
        public DbSet<Imovel> Imoveis { get; set; }
        public DbSet<Foto> Fotos { get; set; }
        public DbSet<TipoContrato> TiposContrato { get; set; }
        public DbSet<ObjetoContrato> ObjetosContrato { get; set; }
        public DbSet<ModalidadeContrato> ModalidadesContrato { get; set; }
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<TipoFoto> TiposFoto { get; set; }
        public DbSet<Vistoria> Vistorias { get; set; }
        public DbSet<Finalidade> Finalidades { get; set; }
        public DbSet<TipoCobranca> TiposCobranca { get; set; }
        public DbSet<Cobranca> Cobrancas { get; set; }
        public DbSet<EventoWebhookAsaas> EventosWebhookAsaas { get; set; }
        public DbSet<EnvioCobrancaEmail> EnviosCobrancaEmail { get; set; }
        public DbSet<DocumentoAutentique> DocumentosAutentique { get; set; }
        public DbSet<SignatarioAutentique> SignatariosAutentique { get; set; }
        public DbSet<Repasse> Repasses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Contrato>().Property(c => c.Id).UseIdentityColumn();
            modelBuilder.Entity<Contrato>().Property(c => c.IdDocumentoAutentique).HasMaxLength(128).UseCollation("Latin1_General_100_BIN2");
            modelBuilder.Entity<Contrato>().Property(c => c.EnvioAutentiqueEmAndamento).IsConcurrencyToken();
            modelBuilder.Entity<Contrato>().Property(c => c.Testemunha1).HasMaxLength(255);
            modelBuilder.Entity<Contrato>().Property(c => c.Testemunha2).HasMaxLength(255);
            modelBuilder.Entity<Contrato>().Property(c => c.Testemunha3).HasMaxLength(255);
            modelBuilder.Entity<Contrato>().OwnsOne(c => c.ContaRepasse1, c => c.Property(p => p.Valor).HasPrecision(18, 4));
            modelBuilder.Entity<Contrato>().OwnsOne(c => c.ContaRepasse2, c => c.Property(p => p.Valor).HasPrecision(18, 4));
            modelBuilder.Entity<Cobranca>().OwnsOne(c => c.ContaRepasse1, c => c.Property(p => p.Valor).HasPrecision(18, 4));
            modelBuilder.Entity<Cobranca>().OwnsOne(c => c.ContaRepasse2, c => c.Property(p => p.Valor).HasPrecision(18, 4));
            modelBuilder.Entity<Repasse>().Property(r => r.Valor).HasPrecision(18, 2);
            modelBuilder.Entity<Repasse>().HasIndex(r => new { r.CobrancaId, r.Destinatario }).IsUnique();
            modelBuilder.Entity<Repasse>().HasIndex(r => r.ReferenciaExterna).IsUnique();
            modelBuilder.Entity<Repasse>().HasIndex(r => r.IdTransferenciaAsaas).IsUnique();
            modelBuilder.Entity<Repasse>().HasOne<Cobranca>().WithMany().HasForeignKey(r => r.CobrancaId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DocumentoAutentique>().Property(d => d.IdDocumentoAutentique).UseCollation("Latin1_General_100_BIN2");
            modelBuilder.Entity<SignatarioAutentique>().Property(s => s.IdDocumentoAutentique).UseCollation("Latin1_General_100_BIN2");
            modelBuilder.Entity<SignatarioAutentique>().Property(s => s.PublicIdAutentique).UseCollation("Latin1_General_100_BIN2");
            modelBuilder.Entity<DocumentoAutentique>().HasAlternateKey(d => new { d.ContratoId, d.IdDocumentoAutentique });
            modelBuilder.Entity<DocumentoAutentique>().HasOne<Contrato>().WithMany().HasForeignKey(d => d.ContratoId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Contrato>().HasOne<DocumentoAutentique>().WithMany().HasForeignKey(c => new { c.Id, c.IdDocumentoAutentique }).HasPrincipalKey(d => new { d.ContratoId, d.IdDocumentoAutentique }).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DocumentoAutentique>().HasMany(d => d.Signatarios).WithOne().HasForeignKey(s => s.IdDocumentoAutentique).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SignatarioAutentique>().HasOne<Cliente>().WithMany().HasForeignKey(s => s.ClienteId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SignatarioAutentique>().HasIndex(s => new { s.IdDocumentoAutentique, s.PublicIdAutentique }).IsUnique();
            modelBuilder.Entity<EventoWebhookAsaas>()
                .Property(e => e.IdEventoAsaas)
                .UseCollation("Latin1_General_100_BIN2");
            modelBuilder.Entity<EventoWebhookAsaas>()
                .HasIndex(e => e.IdEventoAsaas)
                .IsUnique();
            modelBuilder.Entity<EnvioCobrancaEmail>()
                .HasIndex(e => new { e.CobrancaId, e.Tipo, e.Referencia })
                .IsUnique();
        }
    }
}
