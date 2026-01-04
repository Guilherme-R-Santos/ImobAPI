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

    }
}
