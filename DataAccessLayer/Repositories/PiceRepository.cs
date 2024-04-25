using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class PiceRepository : Repository<Pice>
    {
        public PiceRepository() : base (new RestaurantDatabaseModel())
        {
            
        }

        public override IQueryable<Pice> GetAll()
        {
            var query = from p in Entities
                        select p;

            return query;
        }

        public override int Add(Pice entity, bool saveChanges = true)
        {
            var inventar = Context.Inventar.SingleOrDefault(i => i.id_inventar == entity.Inventar.id_inventar);
            var pice = new Pice
            {
                naziv = entity.naziv,
                cijena = entity.cijena,
                nutrivne_informacije = entity.nutrivne_informacije,
                alergeni = entity.alergeni,
                slika = entity.slika,
                Inventar = inventar
            };

            Entities.Add(pice);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Pice entity, bool saveChanges = true)
        {
            var inventar = Context.Inventar.SingleOrDefault(i => i.id_inventar == entity.Inventar.id_inventar);
            var pice = Entities.SingleOrDefault(p => p.id_pice == entity.id_pice);

            pice.naziv = entity.naziv;
            pice.cijena = entity.cijena;
            pice.nutrivne_informacije = entity.nutrivne_informacije;
            pice.alergeni = entity.alergeni;
            pice.slika = entity.slika;
            pice.Inventar = inventar;

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
