﻿using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class KorisnikRepository : Repository<Korisnik>
    {
        public KorisnikRepository() : base(new RestaurantDatabaseModel())
        {

        }

        public override IQueryable<Korisnik> GetAll()
        {
            var query = from k in Entities
                        select k;

            return query;
        }

        public IQueryable<Korisnik> GetKorisnikByKorime(string korime)
        {
            var query = from k in Entities
                        where k.korime.Contains(korime)
                        select k;
            return query;
        }

        public async Task<int> GetBrojKorisnika()
        {
            var query = from k in Entities
                        select k;

            return await query.CountAsync();
        }


        public Korisnik GetKorisnikById(int id)
        {
            var query = from k in Entities
                        where k.id_korisnik == id
                        select k;
            return query.FirstOrDefault();
        }

        public async Task<Korisnik> GetKorisnikByIdAsync(int id)
        {
            var query = from k in Entities
                        where k.id_korisnik == id
                        select k;
            return await query.FirstOrDefaultAsync();
        }

        public override int Add(Korisnik entity, bool saveChanges = true)
        {
            var korisnik = new Korisnik
            {
                korime = entity.korime,
                lozinka = entity.lozinka,
                ime = entity.ime,
                prezime = entity.prezime,
                email = entity.email,
                uloga = entity.uloga,
                slika = entity.slika
            };

            Entities.Add(korisnik);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Korisnik entity, bool saveChanges = true)
        {
            var korisnik = Entities.SingleOrDefault(k => k.id_korisnik == entity.id_korisnik);

            korisnik.korime = entity.korime;
            korisnik.lozinka = entity.lozinka;
            korisnik.ime = entity.ime;
            korisnik.prezime = entity.prezime;
            korisnik.email = entity.email;
            korisnik.uloga = entity.uloga;
            korisnik.slika = entity.slika;

            if (saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }
    }
}
