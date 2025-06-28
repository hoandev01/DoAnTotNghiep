using ChickenF.Models;
using Microsoft.EntityFrameworkCore;

namespace ChickenF.Data
{
    public class FarmContext : DbContext
    {
        public FarmContext(DbContextOptions<FarmContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Cage> Cages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Flock> Flocks { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Tracking> Trackings { get; set; }
       
        public DbSet<NewsArticle> NewsArticles { get; set; }
       
         public DbSet<Slide> Slides { get; set; }
        public DbSet<FlockStage> FlockStages { get; set; }
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Employee> Employees { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Customer>("Customer")
                .HasValue<Admin>("Admin")
                .HasValue<Employee>("Employee");
            // Định dạng số thập phân
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            // Flock → Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Flock)
                .WithMany(f => f.Products)
                .HasForeignKey(p => p.FlockId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderDetail → Product
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails) // cần khai báo List<OrderDetail> trong Product
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // tránh xung đột khi xoá Product

            // OrderDetail → Order
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails) // cần khai báo List<OrderDetail> trong Order
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            
            // Flock → Cage
            modelBuilder.Entity<Flock>()
                .HasOne(f => f.Cage)
                .WithMany()
                .HasForeignKey(f => f.CageId)
                .OnDelete(DeleteBehavior.Cascade);

           

            

            // Seed dữ liệu Slide
            modelBuilder.Entity<Slide>().HasData(
                new Slide
                {
                    Id = 1,
                    Title = "Welcome to Smart Chicken Farm",
                    Subtitle = "Digital Transformation for Poultry",
                    ButtonText = "Learn More",
                    ButtonLink = "/news",
                    ImageUrl = "/Image/slide1.jpg",
                    IsActive = true,
                    DisplayOrder = 1
                },
                new Slide
                {
                    Id = 2,
                    Title = "Manage & Sell Easily",
                    Subtitle = "All-in-one system for farm owners",
                    ButtonText = "Start Now",
                    ButtonLink = "/shop",
                    ImageUrl = "/Image/slide2.jpg",
                    IsActive = true,
                    DisplayOrder = 2
                }
            );

            
        }
    }
}
