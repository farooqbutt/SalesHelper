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
        public DbSet<VendorReference> VendorReference { get; set; } = default!;
        public DbSet<VendorContact> VendorContact { get; set; } = default!;
        public DbSet<VendorDocuments> VendorDocuments { get; set; } = default!;
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<Event> Events { get; set; } = default!;


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

            modelBuilder.Entity<Vendor>()
                .Property(a=>a.VendorReferenceId).IsRequired(false);

            modelBuilder.Entity<BusinessTypes>().HasData(new BusinessTypes
            {
                BusinessTypeId = 1,
                TypeName = "Cabinetry"
            },
            new BusinessTypes
            {
                BusinessTypeId = 2,
                TypeName = "Stone and Countertop"
            },
            new BusinessTypes
            {
                BusinessTypeId = 3,
                TypeName = "Cabinet Hardware"
            },
            new BusinessTypes
            {
                BusinessTypeId = 4,
                TypeName = "Sink and Faucet"
            },
            new BusinessTypes
            {
                BusinessTypeId = 5,
                TypeName = "Wood floor and Tile"
            });
        }
    }
}