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
    }
}
