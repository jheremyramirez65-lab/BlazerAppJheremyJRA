using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppJheremyJRA.API.Models;

public partial class BdbibliotecaContext : DbContext
{
    public BdbibliotecaContext()
    {
    }

    public BdbibliotecaContext(DbContextOptions<BdbibliotecaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Autor> Autors { get; set; }
    public virtual DbSet<Button> Buttons { get; set; }
    public virtual DbSet<DetalleReserva> DetalleReservas { get; set; }
    public virtual DbSet<Libro> Libros { get; set; }
    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<Pagina> Paginas { get; set; }
    public virtual DbSet<PaginaTipoUsuButton> PaginaTipoUsuButtons { get; set; }
    public virtual DbSet<PaginaTipoUsuario> PaginaTipoUsuarios { get; set; }
    public virtual DbSet<Pai> Pais { get; set; }
    public virtual DbSet<Reserva> Reservas { get; set; }
    public virtual DbSet<ReservaEstado> ReservaEstados { get; set; }
    public virtual DbSet<ReservaHistorial> ReservaHistorials { get; set; }
    public virtual DbSet<Sexo> Sexos { get; set; }
    public virtual DbSet<TipoLibro> TipoLibros { get; set; }
    public virtual DbSet<TipoUsuario> TipoUsuarios { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BDBIBLIOTECA;Integrated Security=True;Encrypt=False;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>(entity =>
        {
            entity.HasKey(e => e.Iidautor);

            entity.ToTable("Autor");

            entity.Property(e => e.Iidautor).HasColumnName("IIDAUTOR");
            entity.Property(e => e.Apmaterno)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("APMATERNO");
            entity.Property(e => e.Appaterno)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("APPATERNO");
            entity.Property(e => e.Bhabilitado).HasColumnName("BHABILITADO");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Iidpais).HasColumnName("IIDPAIS");
            entity.Property(e => e.Iidsexo).HasColumnName("IIDSEXO");
            entity.Property(e => e.Imagen).HasColumnName("IMAGEN");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<Button>(entity =>
        {
            entity.HasKey(e => e.Iidbutton).HasName("PK__Button__A6A446D2CF8841B0");

            entity.ToTable("Button");

            entity.Property(e => e.Iidbutton).HasColumnName("IIDBUTTON");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<DetalleReserva>(entity =>
        {
            entity.HasKey(e => e.Iiddetallereserva).HasName("PK__DetalleR__77393E1D1D017B82");

            entity.ToTable("DetalleReserva");

            entity.Property(e => e.Iiddetallereserva).HasColumnName("IIDDETALLERESERVA");
            entity.Property(e => e.Cantidad).HasColumnName("CANTIDAD");
            entity.Property(e => e.IdLibro).HasColumnName("idLibro");
            entity.Property(e => e.Iidreserva).HasColumnName("IIDRESERVA");

            entity.HasOne(d => d.IdLibroNavigation).WithMany(p => p.DetalleReservas)
                .HasForeignKey(d => d.IdLibro)
                .HasConstraintName("FK_Detalle_Libro");

            entity.HasOne(d => d.IidreservaNavigation).WithMany(p => p.DetalleReservas)
                .HasForeignKey(d => d.Iidreserva)
                .HasConstraintName("FK_Detalle_Reserva");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.IdLibro).HasName("PK__Libro__5BC0F026B1DAB463");

            entity.ToTable("Libro");

            entity.Property(e => e.IdLibro).HasColumnName("idLibro");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Archivo).HasColumnName("archivo");
            entity.Property(e => e.Autor)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("autor");
            entity.Property(e => e.Categoria)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("categoria");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.Editorial)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("editorial");
            entity.Property(e => e.IdTipoLibro).HasColumnName("idTipoLibro");
            entity.Property(e => e.Imagen).HasColumnName("imagen");
            entity.Property(e => e.Nombrearchivo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("nombrearchivo");
            entity.Property(e => e.NumeroPaginas).HasColumnName("numeroPaginas");
            entity.Property(e => e.Resumen)
                .IsUnicode(false)
                .HasColumnName("resumen");
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdTipoLibroNavigation).WithMany(p => p.Libros)
                .HasForeignKey(d => d.IdTipoLibro)
                .HasConstraintName("FK_Libro_TipoLibro");

            entity.Property(e => e.Iidautor)
    .HasColumnName("IIDAUTOR");


            entity.HasOne(d => d.IidautorNavigation)
                .WithMany(p => p.Libros)
                .HasForeignKey(d => d.Iidautor)
                .HasConstraintName("FK_Libro_Autor")
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Pagina>(entity =>
        {
            entity.HasKey(e => e.Iidpagina).HasName("PK__Pagina__8E759E4EE48E1AB9");

            entity.ToTable("Pagina");

            entity.Property(e => e.Iidpagina).HasColumnName("IIDPAGINA");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Url)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<PaginaTipoUsuButton>(entity =>
        {
            entity.HasKey(e => e.Iidpaginatipousubutton).HasName("PK__PaginaTi__3B72E98341C46465");

            entity.ToTable("PaginaTipoUsuButton");

            entity.Property(e => e.Iidpaginatipousubutton).HasColumnName("IIDPAGINATIPOUSUBUTTON");
            entity.Property(e => e.Iidbutton).HasColumnName("IIDBUTTON");
            entity.Property(e => e.Iidpaginatipousuario).HasColumnName("IIDPAGINATIPOUSUARIO");

            entity.HasOne(d => d.IidbuttonNavigation).WithMany(p => p.PaginaTipoUsuButtons)
                .HasForeignKey(d => d.Iidbutton)
                .HasConstraintName("FK__PaginaTip__IIDBU__03F0984C");

            entity.HasOne(d => d.IidpaginatipousuarioNavigation).WithMany(p => p.PaginaTipoUsuButtons)
                .HasForeignKey(d => d.Iidpaginatipousuario)
                .HasConstraintName("FK__PaginaTip__IIDPA__02FC7413");
        });

        modelBuilder.Entity<PaginaTipoUsuario>(entity =>
        {
            entity.HasKey(e => e.Iidpaginatipousuario).HasName("PK__PaginaTi__80FD03167F4F8FE2");

            entity.ToTable("PaginaTipoUsuario");

            entity.Property(e => e.Iidpaginatipousuario).HasColumnName("IIDPAGINATIPOUSUARIO");
            entity.Property(e => e.Iidpagina).HasColumnName("IIDPAGINA");
            entity.Property(e => e.Iidtipousuario).HasColumnName("IIDTIPOUSUARIO");

            entity.HasOne(d => d.IidpaginaNavigation).WithMany(p => p.PaginaTipoUsuarios)
                .HasForeignKey(d => d.Iidpagina)
                .HasConstraintName("FK__PaginaTip__IIDPA__7F2BE32F");

            entity.HasOne(d => d.IidtipousuarioNavigation).WithMany(p => p.PaginaTipoUsuarios)
                .HasForeignKey(d => d.Iidtipousuario)
                .HasConstraintName("FK__PaginaTip__IIDTI__00200768");
        });

        modelBuilder.Entity<Pai>(entity =>
        {
            entity.HasKey(e => e.Iidpais).HasName("PK__Pais__09289DAD13019425");

            entity.Property(e => e.Iidpais).HasColumnName("IIDPAIS");
            entity.Property(e => e.Bhabilitado).HasColumnName("BHABILITADO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Iidreserva).HasName("PK__Reserva__03C8C09A5E265E76");

            entity.ToTable("Reserva");

            entity.Property(e => e.Iidreserva).HasColumnName("IIDRESERVA");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA");
            entity.Property(e => e.Iidreservaestado).HasColumnName("IIDRESERVAESTADO");
            entity.Property(e => e.Iidusuario).HasColumnName("IIDUSUARIO");

            entity.HasOne(d => d.IidreservaestadoNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.Iidreservaestado)
                .HasConstraintName("FK_Reserva_Estado");

            entity.HasOne(d => d.IidusuarioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.Iidusuario)
                .HasConstraintName("FK_Reserva_Usuario");
        });

        modelBuilder.Entity<ReservaEstado>(entity =>
        {
            entity.HasKey(e => e.Iidreservaestado).HasName("PK__ReservaE__B308F5BF93850BEE");

            entity.ToTable("ReservaEstado");

            entity.Property(e => e.Iidreservaestado).HasColumnName("IIDRESERVAESTADO");
            entity.Property(e => e.Bhabilitado).HasColumnName("BHABILITADO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<ReservaHistorial>(entity =>
        {
            entity.HasKey(e => e.Iidhistorial).HasName("PK__ReservaH__2801282986B993E9");

            entity.ToTable("ReservaHistorial");

            entity.Property(e => e.Iidhistorial).HasColumnName("IIDHISTORIAL");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("DESCRIPCION");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("FECHA");
            entity.Property(e => e.Iidreserva).HasColumnName("IIDRESERVA");

            entity.HasOne(d => d.IidreservaNavigation).WithMany(p => p.ReservaHistorials)
                .HasForeignKey(d => d.Iidreserva)
                .HasConstraintName("FK_Historial_Reserva");
        });

        modelBuilder.Entity<Sexo>(entity =>
        {
            entity.HasKey(e => e.Iidsexo).HasName("PK__Sexo__50115E723CA6FFCC");

            entity.ToTable("Sexo");

            entity.Property(e => e.Iidsexo).HasColumnName("IIDSEXO");
            entity.Property(e => e.Bhabilitado).HasColumnName("BHABILITADO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<TipoLibro>(entity =>
        {
            entity.HasKey(e => e.IdTipoLibro).HasName("PK__TipoLibr__2BDD0DA65C67780D");

            entity.ToTable("TipoLibro");

            entity.Property(e => e.IdTipoLibro).HasColumnName("idTipoLibro");
            entity.Property(e => e.Bhabilitado).HasColumnName("BHABILITADO");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.NombreTipoLibro)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreTipoLibro");
        });

        modelBuilder.Entity<TipoUsuario>(entity =>
        {
            entity.HasKey(e => e.Iidtipousuario).HasName("PK__TipoUsua__A05A9116AF01F7A5");

            entity.ToTable("TipoUsuario");

            entity.Property(e => e.Iidtipousuario).HasColumnName("IIDTIPOUSUARIO");
            entity.Property(e => e.Bhabilitado).HasColumnName("BHABILITADO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Iidusuario).HasName("PK__Usuario__26DBFF59698460A1");

            entity.ToTable("Usuario");

            entity.Property(e => e.Iidusuario).HasColumnName("IIDUSUARIO");
            entity.Property(e => e.Bhabilitado).HasColumnName("BHABILITADO");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("CORREO");
            entity.Property(e => e.Iidtipousuario).HasColumnName("IIDTIPOUSUARIO");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");

            entity.HasOne(d => d.IidtipousuarioNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Iidtipousuario)
                .HasConstraintName("FK_Usuario_TipoUsuario");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(c => c.IdCategoria);

            entity.ToTable("Categoria");

            entity.Property(c => c.IdCategoria)
                .ValueGeneratedOnAdd();

            entity.Property(c => c.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });


        modelBuilder.Entity<Libro>()
       .HasMany(l => l.Categorias)
       .WithMany(c => c.Libros)
       .UsingEntity<Dictionary<string, object>>(
           "LibroCategoria",
           j => j
               .HasOne<Categoria>()
               .WithMany()
               .HasForeignKey("IdCategoria"),
           j => j
               .HasOne<Libro>()
               .WithMany()
               .HasForeignKey("IdLibro"),
           j =>
           {
               j.HasKey("IdLibro", "IdCategoria");
               j.ToTable("LibroCategoria");
           });


        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
