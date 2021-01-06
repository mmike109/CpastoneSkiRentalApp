using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CapstoneISki.Data;
using CpastroneSkiRental.Models;

namespace CpastroneSkiRental.Data
{
    public class ApplicationDbContext : IdentityDbContext<WebsiteUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }
        public DbSet<Item> Item { get; set; }
        public DbSet<CapstoneISki.Data.Rental> Rental { get; set; }
        public DbSet<CpastroneSkiRental.Models.Deal> Deal { get; set; }
        public DbSet<CpastroneSkiRental.Models.Repair> Repair { get; set; }
    }
}
