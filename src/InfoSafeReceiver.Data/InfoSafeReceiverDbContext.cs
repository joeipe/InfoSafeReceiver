using InfoSafeReceiver.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSafeReceiver.Data
{
    public class InfoSafeReceiverDbContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        public InfoSafeReceiverDbContext(DbContextOptions<InfoSafeReceiverDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Receiver");

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Contact).Assembly);
        }
    }
}
