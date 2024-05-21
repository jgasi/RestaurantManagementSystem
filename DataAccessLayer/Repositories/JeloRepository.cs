using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class JeloRepository : Repository<Jelo>
    {
        public JeloRepository() : base(new RestaurantDatabaseModel())
        {
              
        }

        public override IQueryable<Jelo> GetAll()
        {
            var query = from j in Entities
                        select j;
            
            return query;
        }

        public override int Add(Jelo entity, bool saveChanges = true)
        {
            //var inventar = Context.Inventar.SingleOrDefault(i => i.id_inventar == entity.Inventar.id_inventar);
            var jelo = new Jelo
            {
                naziv = entity.naziv,
                cijena = entity.cijena,
                nutrivne_informacije = entity.nutrivne_informacije,
                alergeni = entity.alergeni,
                slika = entity.slika,
                Inventar_id_inventar = 1
            };

            Entities.Add(jelo);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Jelo entity, bool saveChanges = true)
        {
            var inventar = Context.Inventar.SingleOrDefault(i => i.id_inventar == entity.Inventar.id_inventar);
            var jelo = Entities.SingleOrDefault(j => j.id_jelo == entity.id_jelo);

            jelo.naziv = entity.naziv;
            jelo.cijena = entity.cijena;
            jelo.nutrivne_informacije = entity.nutrivne_informacije;
            jelo.alergeni = entity.alergeni;
            jelo.slika = entity.slika;
            jelo.Inventar = inventar;

            if(saveChanges) 
            {
                return SaveChanges();
            } else
            { 
                return 0;
            }
        }
    }
}
