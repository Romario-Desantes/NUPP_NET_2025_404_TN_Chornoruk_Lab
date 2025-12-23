using Microsoft.EntityFrameworkCore;
using FigureProj.Infrastructure.Models;

namespace FigureProj.Infrastructure
{
    public class FigureContext : DbContext
    {
        public DbSet<FigureModel> Figures { get; set; }
        public DbSet<CircleModel> Circles { get; set; }
        public DbSet<RectangleModel> Rectangles { get; set; }
        public DbSet<SquareModel> Squares { get; set; }
        public DbSet<TriangleModel> Triangles { get; set; }
        public DbSet<CollectionModel> Collections { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<FigureMetadataModel> FigureMetadata { get; set; }
        public DbSet<FigureTagModel> FigureTags { get; set; }

        public FigureContext(DbContextOptions<FigureContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конфігурація Table-per-Type (TPT) для ієрархії фігур
            ConfigureFigureHierarchy(modelBuilder);

            // Конфігурація зв'язку один-до-одного (Figure - Metadata)
            ConfigureOneToOneRelationship(modelBuilder);

            // Конфігурація зв'язку один-до-багатьох (Collection - Figures)
            ConfigureOneToManyRelationship(modelBuilder);

            // Конфігурація зв'язку багато-до-багатьох (Figure - Tags)
            ConfigureManyToManyRelationship(modelBuilder);

            // Додаткові налаштування
            ConfigureIndexes(modelBuilder);
        }

        private void ConfigureFigureHierarchy(ModelBuilder modelBuilder)
        {
            // Базова таблиця для всіх фігур
            modelBuilder.Entity<FigureModel>()
                .ToTable("Figures")
                .HasKey(f => f.Id);

            modelBuilder.Entity<FigureModel>()
                .Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<FigureModel>()
                .Property(f => f.Color)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<FigureModel>()
                .Property(f => f.CreatedAt)
                .HasDefaultValueSql("NOW()");

            // Table-per-Type: окремі таблиці для кожного типу фігури
            modelBuilder.Entity<CircleModel>()
                .ToTable("Circles")
                .HasBaseType<FigureModel>();

            modelBuilder.Entity<CircleModel>()
                .Property(c => c.Radius)
                .IsRequired();

            modelBuilder.Entity<RectangleModel>()
                .ToTable("Rectangles")
                .HasBaseType<FigureModel>();

            modelBuilder.Entity<RectangleModel>()
                .Property(r => r.Height)
                .IsRequired();

            modelBuilder.Entity<RectangleModel>()
                .Property(r => r.Width)
                .IsRequired();

            modelBuilder.Entity<SquareModel>()
                .ToTable("Squares")
                .HasBaseType<FigureModel>();

            modelBuilder.Entity<SquareModel>()
                .Property(s => s.Side)
                .IsRequired();

            modelBuilder.Entity<TriangleModel>()
                .ToTable("Triangles")
                .HasBaseType<FigureModel>();

            modelBuilder.Entity<TriangleModel>()
                .Property(t => t.A)
                .IsRequired();

            modelBuilder.Entity<TriangleModel>()
                .Property(t => t.B)
                .IsRequired();

            modelBuilder.Entity<TriangleModel>()
                .Property(t => t.C)
                .IsRequired();
        }

        private void ConfigureOneToOneRelationship(ModelBuilder modelBuilder)
        {
            // Зв'язок один-до-одного: Figure <-> Metadata
            modelBuilder.Entity<FigureMetadataModel>()
                .ToTable("FigureMetadata")
                .HasKey(m => m.Id);

            modelBuilder.Entity<FigureMetadataModel>()
                .Property(m => m.Author)
                .HasMaxLength(100);

            modelBuilder.Entity<FigureMetadataModel>()
                .Property(m => m.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<FigureMetadataModel>()
                .Property(m => m.CreatedBy)
                .HasMaxLength(100);

            modelBuilder.Entity<FigureMetadataModel>()
                .Property(m => m.LastModified)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<FigureModel>()
                .HasOne(f => f.Metadata)
                .WithOne(m => m.Figure)
                .HasForeignKey<FigureMetadataModel>(m => m.FigureId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureOneToManyRelationship(ModelBuilder modelBuilder)
        {
            // Зв'язок один-до-багатьох: Collection -> Figures
            modelBuilder.Entity<CollectionModel>()
                .ToTable("Collections")
                .HasKey(c => c.Id);

            modelBuilder.Entity<CollectionModel>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<CollectionModel>()
                .Property(c => c.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<CollectionModel>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<CollectionModel>()
                .HasMany(c => c.Figures)
                .WithOne(f => f.Collection)
                .HasForeignKey(f => f.CollectionId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        private void ConfigureManyToManyRelationship(ModelBuilder modelBuilder)
        {
            // Конфігурація тегів
            modelBuilder.Entity<TagModel>()
                .ToTable("Tags")
                .HasKey(t => t.Id);

            modelBuilder.Entity<TagModel>()
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<TagModel>()
                .Property(t => t.Color)
                .HasMaxLength(30);

            modelBuilder.Entity<TagModel>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("NOW()");

            // Зв'язок багато-до-багатьох: Figures <-> Tags через FigureTags
            modelBuilder.Entity<FigureTagModel>()
                .ToTable("FigureTags")
                .HasKey(ft => new { ft.FigureId, ft.TagId });

            modelBuilder.Entity<FigureTagModel>()
                .Property(ft => ft.AssignedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<FigureTagModel>()
                .HasOne(ft => ft.Figure)
                .WithMany(f => f.FigureTags)
                .HasForeignKey(ft => ft.FigureId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FigureTagModel>()
                .HasOne(ft => ft.Tag)
                .WithMany(t => t.FigureTags)
                .HasForeignKey(ft => ft.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            // Індекси для оптимізації запитів
            modelBuilder.Entity<FigureModel>()
                .HasIndex(f => f.Name)
                .HasDatabaseName("IX_Figures_Name");

            modelBuilder.Entity<FigureModel>()
                .HasIndex(f => f.Color)
                .HasDatabaseName("IX_Figures_Color");

            modelBuilder.Entity<FigureModel>()
                .HasIndex(f => f.CreatedAt)
                .HasDatabaseName("IX_Figures_CreatedAt");

            modelBuilder.Entity<CollectionModel>()
                .HasIndex(c => c.Name)
                .HasDatabaseName("IX_Collections_Name");

            modelBuilder.Entity<TagModel>()
                .HasIndex(t => t.Name)
                .IsUnique()
                .HasDatabaseName("IX_Tags_Name_Unique");
        }
    }
}

