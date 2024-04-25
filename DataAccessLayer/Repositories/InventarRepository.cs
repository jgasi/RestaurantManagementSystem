using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class InventarRepository : Repository<Inventar>
    {
        public InventarRepository() : base(new RestaurantDatabaseModel())
        {
              
        }

        public override IQueryable<Inventar> GetAll()
        {
            var query = from i in Entities
                        select i;

            return query;
        }

        public override int Add(Inventar entity, bool saveChanges = true)
        {
            var inventar = new Inventar
            {
                kolicina_na_zalihi = entity.kolicina_na_zalihi,
                minimalna_kolicina_narudzbe = entity.minimalna_kolicina_narudzbe,
                datum_nabave = entity.datum_nabave,
                dostavljac = entity.dostavljac,
                cijena = entity.cijena
            };

            Entities.Add(inventar);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Inventar entity, bool saveChanges = true)
        {
            var inventar = Entities.SingleOrDefault(i => i.id_inventar == entity.id_inventar);

            inventar.kolicina_na_zalihi = entity.kolicina_na_zalihi;
            inventar.minimalna_kolicina_narudzbe = entity.minimalna_kolicina_narudzbe;
            inventar.datum_nabave = entity.datum_nabave;
            inventar.dostavljac = entity.dostavljac;
            inventar.cijena = entity.cijena;

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
