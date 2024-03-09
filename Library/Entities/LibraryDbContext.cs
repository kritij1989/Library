using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Library.Entities;

public partial class LibraryDbContext : DbContext
{
    private LibraryDbContext Context { get; }

    public LibraryDbContext() { }
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BookMaster> BookMasters { get; set; }

    public virtual DbSet<ReaderInfo> ReaderInfos { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;initial catalog=LibraryDB;Integrated Security=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookMaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BookMast__3214EC07FF9AC132");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.ToTable("BookMaster");

            
        });

        modelBuilder.Entity<ReaderInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReaderIn__3214EC077CF862D1");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.ToTable("ReaderInfo");

            
        });

        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Table__3214EC07CB2D6813");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.ToTable("Table");


            entity.HasOne(d => d.Book).WithMany(p => p.Tables)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Table_To_BookID");

            entity.HasOne(d => d.Reader).WithMany(p => p.Tables)
                .HasForeignKey(d => d.ReaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Table_ReaderId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
