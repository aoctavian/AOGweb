using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AOGweb.Models;
using AOGweb.Models.User;
using Microsoft.Extensions.Configuration;

namespace AOGweb.Data
{
    public class WebAppContext : DbContext
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<Device1> Devices1 { get; set; }
        public DbSet<Device2> Devices2 { get; set; }
        public DbSet<UserDevices> UserDevices { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>().ToTable("device");
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<UserDevices>().ToTable("userDevices").HasKey(k => new { k.OwnerID, k.DeviceMAC });
            modelBuilder.Entity<Device>().HasDiscriminator<string>("DeviceTYPE");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source = ws2019; Initial Catalog = diploma_db; Persist Security Info = True; User ID = user_diploma; Password = sH#Y-TyH_H@XH-zf*Gdb^jDxD-xFQcER=gYLfLMw+ZR8L%7h@EH*2@3fG7z$*4eDj=");
        }
    }
}
