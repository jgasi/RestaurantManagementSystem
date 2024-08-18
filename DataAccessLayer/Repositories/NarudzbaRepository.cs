using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public IQueryable<Narudzba> GetByIdNarudzbu(int id)
        {
            var query = from n in Entities
                        where n.id_narudzba == id
                        select n;

            return query;
        }

        public Task<List<Narudzba>> GetAllWithStavkaNarudzbeAsync()
        {
            return Entities.Include(n => n.Stavka_narudzbe).ToListAsync();
        }

        public IQueryable<Narudzba> GetAllById(int korisnikId)
        {
            var query = from n in Entities
                        where n.Korisnik_id_korisnik == korisnikId
                        select n;

            return query;
        }

        public IQueryable<Narudzba> GetByDate(DateTime? startDate, DateTime? endDate)
        {
            var query = from n in Entities
                        where n.datum_vrijeme >= startDate && n.datum_vrijeme <= endDate
                        select n;

            return query;
        }

        public Task<List<Narudzba>> GetByDateNow(DateTime? now)
        {
            return Entities.Include(n => n.Stavka_narudzbe)
                           .Where(n => n.datum_vrijeme.HasValue &&
                                       n.datum_vrijeme.Value >= now)
                           .ToListAsync();
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
            var narudzba = Entities.SingleOrDefault(n => n.id_narudzba == entity.id_narudzba);

            narudzba.datum_vrijeme = entity.datum_vrijeme;
            narudzba.racun = entity.racun;
            narudzba.status = entity.status;
            narudzba.Korisnik_id_korisnik = entity.Korisnik_id_korisnik;

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
