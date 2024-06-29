using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class NarudzbaRepository : Repository<Narudzba>
    {
        public NarudzbaRepository() : base(new RestaurantDatabaseModel())
        {
            
        }

        public override IQueryable<Narudzba> GetAll()
        {
            var query = from n in Entities
                        select n;

            return query;
        }

        public IQueryable<Narudzba> GetAllById(int korisnikId)
        {
            var query = from n in Entities
                        where n.Korisnik_id_korisnik == korisnikId
                        select n;

            return query;
        }

        public Narudzba GetLastNarudzbaByKorisnik(int korisnikId)
        {
            var query = Entities
                .Where(n => n.Korisnik_id_korisnik == korisnikId)
                .OrderByDescending(n => n.datum_vrijeme)
                .FirstOrDefault();

            return query;
        }

        public override int Add(Narudzba entity, bool saveChanges = true)
        {
            var narudzba = new Narudzba
            {
                datum_vrijeme = entity.datum_vrijeme,
                racun = entity.racun,
                status = entity.status,
                Korisnik_id_korisnik = entity.Korisnik_id_korisnik
            };

            Entities.Add(narudzba);
            if (saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override async Task<int> AddAsync(Narudzba entity, bool saveChanges = true)
        {
            Entities.Add(entity);

            if (saveChanges)
            {
                return await SaveChangesAsync();
            }
            else
            {
                return 0;
            }
        }



        public override int Update(Narudzba entity, bool saveChanges = true)
        {
            var korisnik = Context.Korisnik.SingleOrDefault(k => k.id_korisnik == entity.Korisnik.id_korisnik);
            var narudzba = Entities.SingleOrDefault(n => n.id_narudzba == entity.id_narudzba);

            narudzba.datum_vrijeme = entity.datum_vrijeme;
            narudzba.racun = entity.racun;
            narudzba.status = entity.status;
            narudzba.Korisnik = entity.Korisnik;

            if (saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public void SpremiPromjene()
        {
            SaveChanges();
        }
    }
}
