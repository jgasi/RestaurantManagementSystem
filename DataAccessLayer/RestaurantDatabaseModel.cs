using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using EntitiesLayer.Entities;

namespace DataAccessLayer
{
    public partial class RestaurantDatabaseModel : DbContext
    {
        public RestaurantDatabaseModel()
            : base("name=RestaurantDatabaseModel")
        {
        }

        public virtual DbSet<Informacije> Informacije { get; set; }
        public virtual DbSet<Inventar> Inventar { get; set; }
        public virtual DbSet<Jelo> Jelo { get; set; }
        public virtual DbSet<Korisnik> Korisnik { get; set; }
        public virtual DbSet<Narudzba> Narudzba { get; set; }
        public virtual DbSet<Pice> Pice { get; set; }
        public virtual DbSet<Recenzija> Recenzija { get; set; }
        public virtual DbSet<Rezervacija> Rezervacija { get; set; }
        public virtual DbSet<Statistika> Statistika { get; set; }
        public virtual DbSet<Stavka_narudzbe> Stavka_narudzbe { get; set; }
        public virtual DbSet<Stol> Stol { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Informacije>()
                .Property(e => e.naziv)
                .IsUnicode(false);

            modelBuilder.Entity<Informacije>()
                .Property(e => e.adresa)
                .IsUnicode(false);

            modelBuilder.Entity<Informacije>()
                .Property(e => e.grad)
                .IsUnicode(false);

            modelBuilder.Entity<Informacije>()
                .Property(e => e.radno_vrijeme)
                .IsUnicode(false);

            modelBuilder.Entity<Informacije>()
                .Property(e => e.kontakt_telefon)
                .IsUnicode(false);

            modelBuilder.Entity<Informacije>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<Inventar>()
                .Property(e => e.kolicina_na_zalihi)
                .IsUnicode(false);

            modelBuilder.Entity<Inventar>()
                .Property(e => e.minimalna_kolicina_narudzbe)
                .IsUnicode(false);

            modelBuilder.Entity<Inventar>()
                .Property(e => e.dostavljac)
                .IsUnicode(false);

            modelBuilder.Entity<Inventar>()
                .Property(e => e.cijena)
                .IsUnicode(false);

            modelBuilder.Entity<Inventar>()
                .HasMany(e => e.Jelo)
                .WithRequired(e => e.Inventar)
                .HasForeignKey(e => e.Inventar_id_inventar)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Inventar>()
                .HasMany(e => e.Pice)
                .WithRequired(e => e.Inventar)
                .HasForeignKey(e => e.Inventar_id_inventar)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Jelo>()
                .Property(e => e.naziv)
                .IsUnicode(false);

            modelBuilder.Entity<Jelo>()
                .Property(e => e.cijena)
                .IsUnicode(false);

            modelBuilder.Entity<Jelo>()
                .Property(e => e.nutrivne_informacije)
                .IsUnicode(false);

            modelBuilder.Entity<Jelo>()
                .Property(e => e.alergeni)
                .IsUnicode(false);

            modelBuilder.Entity<Jelo>()
                .HasMany(e => e.Recenzija)
                .WithOptional(e => e.Jelo)
                .HasForeignKey(e => e.Jelo_id_jelo);

            modelBuilder.Entity<Jelo>()
                .HasMany(e => e.Stavka_narudzbe)
                .WithOptional(e => e.Jelo)
                .HasForeignKey(e => e.Jelo_id_jelo);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.korime)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.lozinka)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.ime)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.prezime)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.email)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .Property(e => e.uloga)
                .IsUnicode(false);

            modelBuilder.Entity<Korisnik>()
                .HasMany(e => e.Narudzba)
                .WithRequired(e => e.Korisnik)
                .HasForeignKey(e => e.Korisnik_id_korisnik)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Korisnik>()
                .HasMany(e => e.Recenzija)
                .WithRequired(e => e.Korisnik)
                .HasForeignKey(e => e.Korisnik_id_korisnik)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Korisnik>()
                .HasMany(e => e.Rezervacija)
                .WithRequired(e => e.Korisnik)
                .HasForeignKey(e => e.Korisnik_id_korisnik)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Narudzba>()
                .Property(e => e.racun)
                .IsUnicode(false);

            modelBuilder.Entity<Narudzba>()
                .Property(e => e.status)
                .IsUnicode(false);

            modelBuilder.Entity<Narudzba>()
                .HasMany(e => e.Stavka_narudzbe)
                .WithRequired(e => e.Narudzba)
                .HasForeignKey(e => e.Narudzba_id_narudzba)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pice>()
                .Property(e => e.naziv)
                .IsUnicode(false);

            modelBuilder.Entity<Pice>()
                .Property(e => e.cijena)
                .IsUnicode(false);

            modelBuilder.Entity<Pice>()
                .Property(e => e.nutrivne_informacije)
                .IsUnicode(false);

            modelBuilder.Entity<Pice>()
                .Property(e => e.alergeni)
                .IsUnicode(false);

            modelBuilder.Entity<Pice>()
                .HasMany(e => e.Recenzija)
                .WithOptional(e => e.Pice)
                .HasForeignKey(e => e.Pice_id_pice);

            modelBuilder.Entity<Pice>()
                .HasMany(e => e.Stavka_narudzbe)
                .WithOptional(e => e.Pice)
                .HasForeignKey(e => e.Pice_id_pice);

            modelBuilder.Entity<Recenzija>()
                .Property(e => e.ocjena)
                .IsUnicode(false);

            modelBuilder.Entity<Recenzija>()
                .Property(e => e.komentar)
                .IsUnicode(false);

            modelBuilder.Entity<Statistika>()
                .Property(e => e.broj_prodanih_jedinica)
                .IsUnicode(false);

            modelBuilder.Entity<Statistika>()
                .Property(e => e.ukupni_prihod)
                .IsUnicode(false);

            modelBuilder.Entity<Statistika>()
                .Property(e => e.prosjecna_ocjena)
                .IsUnicode(false);

            modelBuilder.Entity<Statistika>()
                .HasMany(e => e.Stavka_narudzbe)
                .WithOptional(e => e.Statistika)
                .HasForeignKey(e => e.Statistika_id_statistika);

            modelBuilder.Entity<Stavka_narudzbe>()
                .Property(e => e.kolicina)
                .IsUnicode(false);

            modelBuilder.Entity<Stavka_narudzbe>()
                .Property(e => e.prilagodbe)
                .IsUnicode(false);

            modelBuilder.Entity<Stol>()
                .Property(e => e.broj_stola)
                .IsUnicode(false);

            modelBuilder.Entity<Stol>()
                .Property(e => e.status)
                .IsUnicode(false);

            modelBuilder.Entity<Stol>()
                .HasMany(e => e.Rezervacija)
                .WithRequired(e => e.Stol)
                .HasForeignKey(e => e.Stol_id_stol)
                .WillCascadeOnDelete(false);
        }
    }
}
