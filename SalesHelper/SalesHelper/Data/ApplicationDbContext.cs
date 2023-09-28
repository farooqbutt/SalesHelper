﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalesHelper.Models;

namespace SalesHelper.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Account> Account { get; set; } = default!;
        public DbSet<Address> Address { get; set; } = default!;
        public DbSet<AccountBilling> AccountBilling { get; set; } = default!;
        public DbSet<AccountShipping> AccountShipping { get; set; } = default!;
        public DbSet<BusinessTypes> BusinessTypes { get; set; } = default!;
        public DbSet<Vendor> Vendor { get; set; } = default!;
        public DbSet<VendorContact> VendorContact { get; set; } = default!;
        public DbSet<MyVendor> MyVendor { get; set; } = default!;
        public DbSet<MyVendorContact> MyVendorContact { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .Property(a => a.AccountNumber)
                .UseIdentityColumn(236200, 1);

            modelBuilder.Entity<BusinessTypes>()
                .HasIndex(b => b.TypeName).IsUnique();
        }
    }
}