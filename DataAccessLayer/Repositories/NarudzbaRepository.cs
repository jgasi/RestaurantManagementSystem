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

        public override int Add(Narudzba entity, bool saveChanges = true)
        {
            var korisnik = Context.Korisnik.SingleOrDefault(k => k.id_korisnik == entity.Korisnik.id_korisnik);
            var narudzba = new Narudzba
            {
                datum_vrijeme = entity.datum_vrijeme,
                racun = entity.racun,
                status = entity.status,
                Korisnik = korisnik
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
    }
}
