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

            // 🔐 Kế thừa vai trò User: TPH strategy
            modelBuilder.Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<User>("User")
                .HasValue<Customer>("Customer")
                .HasValue<Admin>("Admin")
                .HasValue<Employee>("Employee");

            // 💵 Định dạng số tiền
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            // 🐔 Flock → Product: KHÔNG dùng Cascade
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Flock)
                .WithMany(f => f.Products)
                .HasForeignKey(p => p.FlockId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh lỗi khi Product đang nằm trong Order

            // 🧾 OrderDetail → Product: KHÔNG dùng Cascade
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Cấm xoá Product nếu đã được bán

            // 📦 OrderDetail → Order: OK dùng Cascade
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Xoá Order sẽ xoá luôn OrderDetails

            // 🏠 Flock → Cage: KHÔNG dùng Cascade
            modelBuilder.Entity<Flock>()
                .HasOne(f => f.Cage)
                .WithMany()
                .HasForeignKey(f => f.CageId)
                .OnDelete(DeleteBehavior.Restrict); // Cấm xoá Cage nếu còn Flock

            // 🧪 Flock → Category: KHÔNG dùng Cascade
            modelBuilder.Entity<Flock>()
                .HasOne(f => f.Category)
                .WithMany()
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh mất dữ liệu nếu xóa Category

            // 🧬 FlockStage → Flock: Có thể dùng Cascade
            modelBuilder.Entity<FlockStage>()
                .HasOne(fs => fs.Flock)
                .WithMany(f => f.FlockStages)
                .HasForeignKey(fs => fs.FlockId)
                .OnDelete(DeleteBehavior.Cascade); // Flock bị xoá → xoá luôn FlockStages

            // 📊 Tracking → Flock: Có thể dùng Cascade
            modelBuilder.Entity<Tracking>()
                .HasOne(t => t.Flock)
                .WithMany()
                .HasForeignKey(t => t.FlockId)
                .OnDelete(DeleteBehavior.Cascade); // Xoá Flock sẽ xoá theo Tracking

            // 🛒 CartItem → Product: KHÔNG cascade
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xung đột khi Product đã nằm trong giỏ

            // 🛒 CartItem → Customer (nếu có)
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Xoá Customer sẽ xoá giỏ hàng
        





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
