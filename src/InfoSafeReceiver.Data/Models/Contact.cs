using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace InfoSafeReceiver.Data.Models
{
    public class Contact : Entity, IEntityTypeConfiguration<Contact>
    {
        public int RefId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime DoB { get; set; }

        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.Property(e => e.RefId)
                .IsRequired();

            builder.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.DoB)
                .HasColumnType("date")
                .IsRequired();
        }
    }
}