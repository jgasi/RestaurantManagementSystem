using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class RecenzijaRepository : Repository<Recenzija>
    {
        public RecenzijaRepository() : base (new RestaurantDatabaseModel())
        {
            
        }

        public override IQueryable<Recenzija> GetAll()
        {
            var query = from r in Entities
                        select r;

            return query;
        }

        public override int Add(Recenzija entity, bool saveChanges = true)
        {
            var korisnik = Context.Korisnik.SingleOrDefault(k => k.id_korisnik == entity.Korisnik.id_korisnik);
            var jelo = Context.Jelo.SingleOrDefault(j => j.id_jelo == entity.Jelo.id_jelo);
            var pice = Context.Pice.SingleOrDefault(p => p.id_pice == entity.Pice.id_pice);

            var recenzija = new Recenzija
            {
                ocjena = entity.ocjena,
                komentar = entity.komentar,
                Korisnik = korisnik,
                Jelo = jelo,
                Pice = pice
            };

            Entities.Add(recenzija);
            if (saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Recenzija entity, bool saveChanges = true)
        {
            var korisnik = Context.Korisnik.SingleOrDefault(k => k.id_korisnik == entity.Korisnik.id_korisnik);
            var jelo = Context.Jelo.SingleOrDefault(j => j.id_jelo == entity.Jelo.id_jelo);
            var pice = Context.Pice.SingleOrDefault(p => p.id_pice == entity.Pice.id_pice);

            var recenzija = Entities.SingleOrDefault(r => r.id_recenzija == entity.id_recenzija);

            recenzija.ocjena = entity.ocjena;
            recenzija.komentar = entity.komentar;
            recenzija.Korisnik = korisnik;
            recenzija.Jelo = jelo;
            recenzija.Pice = pice;

            if(saveChanges)
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
