using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class InformacijeRepository : Repository<Informacije>
    {
        public InformacijeRepository() : base(new RestaurantDatabaseModel())
        {
              
        }

        public override IQueryable<Informacije> GetAll()
        {
            var query = from i in Entities
                        select i;

            return query;
        }

        public override int Add(Informacije entity, bool saveChanges = true)
        {
            var informacije = new Informacije
            {
                naziv = entity.naziv,
                adresa = entity.adresa,
                grad = entity.grad,
                radno_vrijeme = entity.radno_vrijeme,
                kontakt_telefon = entity.kontakt_telefon,
                email = entity.email,
                slika = entity.slika
            };

            Entities.Add(informacije);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Informacije entity, bool saveChanges = true)
        {
            var informacije = Entities.SingleOrDefault(i => i.id_informacije == entity.id_informacije);

            informacije.naziv = entity.naziv;
            informacije.adresa = entity.adresa;
            informacije.grad = entity.grad;
            informacije.radno_vrijeme = entity.radno_vrijeme;
            informacije.kontakt_telefon = entity.kontakt_telefon;
            informacije.email = entity.email;
            informacije.slika = entity.slika;

            if (saveChanges)
            {
                return SaveChanges();
            }else 
            { 
                return 0; 
            }
        }

    }
}
