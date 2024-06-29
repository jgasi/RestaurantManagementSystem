using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class Stavka_narudzbeRepository : Repository<Stavka_narudzbe>
    {
        public Stavka_narudzbeRepository() : base (new RestaurantDatabaseModel())
        {
            
        }

        public override IQueryable<Stavka_narudzbe> GetAll()
        {
            var query = from sn in Entities
                        select sn;

            return query;
        }

        public override int Add(Stavka_narudzbe entity, bool saveChanges = true)
        {
            //var narudzba = Context.Narudzba.SingleOrDefault(n => n.id_narudzba == entity.Narudzba.id_narudzba);
            //var statistika = Context.Statistika.SingleOrDefault(s => s.id_statistika == entity.Statistika.id_statistika);
            //var jelo = Context.Jelo.SingleOrDefault(j => j.id_jelo == entity.Jelo.id_jelo);
            //var pice = Context.Pice.SingleOrDefault(p => p.id_pice == entity.Pice.id_pice);

            var stavka_narudzbe = new Stavka_narudzbe
            {
                kolicina = entity.kolicina,
                prilagodbe = entity.prilagodbe,
                Narudzba_id_narudzba = entity.Narudzba_id_narudzba,
                Statistika_id_statistika = entity.Statistika_id_statistika,
                Jelo_id_jelo = entity.Jelo_id_jelo,
                Pice_id_pice = entity.Pice_id_pice
            };

            Entities.Add(stavka_narudzbe);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override Task<int> AddAsync(Stavka_narudzbe entity, bool saveChanges = true)
        {
            //var narudzba = Context.Narudzba.SingleOrDefault(n => n.id_narudzba == entity.Narudzba.id_narudzba);
            //var statistika = Context.Statistika.SingleOrDefault(s => s.id_statistika == entity.Statistika.id_statistika);
            //var jelo = Context.Jelo.SingleOrDefault(j => j.id_jelo == entity.Jelo.id_jelo);
            //var pice = Context.Pice.SingleOrDefault(p => p.id_pice == entity.Pice.id_pice);

            var stavka_narudzbe = new Stavka_narudzbe
            {
                kolicina = entity.kolicina,
                prilagodbe = entity.prilagodbe,
                Narudzba_id_narudzba = entity.Narudzba_id_narudzba,
                Statistika_id_statistika = null,
                Jelo_id_jelo = entity.Jelo_id_jelo,
                Pice_id_pice = entity.Pice_id_pice
            };

            Entities.Add(stavka_narudzbe);
            if (saveChanges)
            {
                return Task.FromResult(SaveChanges());
            }
            else
            {
                return Task.FromResult(0);
            }
        }

        public override int Update(Stavka_narudzbe entity, bool saveChanges = true)
        {
            var narudzba = Context.Narudzba.SingleOrDefault(n => n.id_narudzba == entity.Narudzba.id_narudzba);
            var statistika = Context.Statistika.SingleOrDefault(s => s.id_statistika == entity.Statistika.id_statistika);
            var jelo = Context.Jelo.SingleOrDefault(j => j.id_jelo == entity.Jelo.id_jelo);
            var pice = Context.Pice.SingleOrDefault(p => p.id_pice == entity.Pice.id_pice);

            var stavka_narudzbe = Entities.SingleOrDefault(sn => sn.id_stavka_narudzbe == entity.id_stavka_narudzbe);

            stavka_narudzbe.kolicina = entity.kolicina;
            stavka_narudzbe.prilagodbe = entity.prilagodbe;
            stavka_narudzbe.Narudzba = narudzba;
            stavka_narudzbe.Statistika = statistika;
            stavka_narudzbe.Jelo = jelo;
            stavka_narudzbe.Pice = pice;

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
