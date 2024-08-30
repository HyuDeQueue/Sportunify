using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repositories.Models;

public partial class ListentogetherContext : DbContext
{
    public ListentogetherContext()
    {
    }

    public ListentogetherContext(DbContextOptions<ListentogetherContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistSong> PlaylistSongs { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    //private string GetConnectionString()
    //{
    //    IConfiguration config = new ConfigurationBuilder()
    //         .SetBasePath(Directory.GetCurrentDirectory())
    //                .AddJsonFile("appsettings.json", true, true)
    //                .Build();
    //    var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];

    //    return strConn;
    //}

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    //=> optionsBuilder.UseMySql(GetConnectionString(), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));
    //    => optionsBuilder.UseMySql("server=115.73.218.193;port=3307;database=listentogether;user=root;password=12345", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(GetConnectionString(), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));
    }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        var strConn = config.GetConnectionString("DefaultConnectionStringDB");

        return strConn;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PRIMARY");

            entity.ToTable("account");

            entity.HasIndex(e => e.Username, "Username").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("Category_Name");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("PRIMARY");

            entity.ToTable("playlists");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.HasIndex(e => e.PlaylistName, "Playlist_name").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PlaylistName).HasColumnName("Playlist_name");

            entity.HasOne(d => d.Account).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playlists_ibfk_1");
        });

        modelBuilder.Entity<PlaylistSong>(entity =>
        {
            entity.HasKey(e => new { e.PlaylistId, e.SongId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("playlist_songs");

            entity.HasIndex(e => e.SongId, "SongId");

            entity.HasOne(d => d.Playlist).WithMany(p => p.PlaylistSongs)
                .HasForeignKey(d => d.PlaylistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playlist_songs_ibfk_1");

            entity.HasOne(d => d.Song).WithMany(p => p.PlaylistSongs)
                .HasForeignKey(d => d.SongId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("playlist_songs_ibfk_2");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.HasKey(e => e.SongId).HasName("PRIMARY");

            entity.ToTable("songs");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.HasIndex(e => e.CategoryId, "CategoryId");

            entity.HasIndex(e => e.Title, "Title").IsUnique();

            entity.Property(e => e.ArtistName)
                .HasMaxLength(255)
                .HasColumnName("Artist_name");

            entity.HasOne(d => d.Account).WithMany(p => p.Songs)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("songs_ibfk_1");

            entity.HasOne(d => d.Category).WithMany(p => p.Songs)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("songs_ibfk_2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
