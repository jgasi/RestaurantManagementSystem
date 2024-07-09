using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class RezervacijaRepository : Repository<Rezervacija>
    {
        public RezervacijaRepository() : base(new RestaurantDatabaseModel())
        {
            
        }

        public override IQueryable<Rezervacija> GetAll()
        {
            var query = from r in Entities
                        select r;

            return query;
        }

        public override int Add(Rezervacija entity, bool saveChanges = true)
        {
            var korisnik = entity.Korisnik_id_korisnik;
            var stol = entity.Stol_id_stol;

            var rezervacija = new Rezervacija
            {
                datum_vrijeme = entity.datum_vrijeme,
                Korisnik_id_korisnik = korisnik,
                Stol_id_stol = stol
            };

            Entities.Add(rezervacija);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Rezervacija entity, bool saveChanges = true)
        {
            var korisnik = Context.Korisnik.SingleOrDefault(k => k.id_korisnik == entity.Korisnik.id_korisnik);
            var stol = Context.Stol.SingleOrDefault(s => s.id_stol == entity.Stol.id_stol);

            var rezervacija = Entities.SingleOrDefault(r => r.id_rezervacija == entity.id_rezervacija);

            rezervacija.datum_vrijeme = entity.datum_vrijeme;
            rezervacija.Korisnik = korisnik;
            rezervacija.Stol = stol;

            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public async Task<int> RemoveByDateAndIdAsync(DateTime? vrijeme, int korisnikId)
        {
            var rezervacija = await Entities.FirstOrDefaultAsync(r => r.datum_vrijeme == vrijeme && r.Korisnik_id_korisnik == korisnikId);

            if (rezervacija != null)
            {
                Entities.Remove(rezervacija);
                return await SaveChangesAsync();
            }
            else
            {
                return 0;
            }
        }
    }
}
