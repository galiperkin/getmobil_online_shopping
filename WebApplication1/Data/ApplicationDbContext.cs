using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    

public DbSet<WebApplication1.Models.Cart> Cart { get; set; } = default!;

public DbSet<WebApplication1.Models.Product> Product { get; set; } = default!;

public DbSet<WebApplication1.Models.Order> Order { get; set; } = default!;

public DbSet<WebApplication1.Models.Invoice> Invoice { get; set; } = default!;

public DbSet<WebApplication1.Models.CartItem> CartItem { get; set; } = default!;

public DbSet<WebApplication1.Models.OrderItem> OrderItem { get; set; } = default!;

public DbSet<WebApplication1.Models.InvoiceItem> InvoiceItem { get; set; } = default!;
}