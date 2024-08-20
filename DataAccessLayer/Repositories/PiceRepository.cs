using System.Data.Entity;
using System.Linq;
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

        public async Task<int> GetBrojPica()
        {
            var query = from p in Entities
                        select p;

            return await query.CountAsync();
        }

        public IQueryable<Pice> GetPiceById(int givenId)
        {
            var query = from p in Entities
                        where p.id_pice == givenId
                        select p;

            return query;
        }

        public IQueryable<Pice> GetPiceByIdInventara(int givenId)
        {
            var query = from p in Entities
                        where p.Inventar_id_inventar == givenId
                        select p;

            return query;
        }

        public IQueryable<Pice> GetFirstThreePica()
        {
            var query = Entities.OrderBy(p => p.id_pice)
                                .Take(3);

            return query;
        }


        public IQueryable<Pice> GetAllByName(string name)
        {
            var query = from p in Entities
                        where p.naziv.Contains(name)
                        select p;

            return query;
        }

        public override int Add(Pice entity, bool saveChanges = true)
        {
            var pice = new Pice
            {
                naziv = entity.naziv,
                cijena = entity.cijena,
                nutrivne_informacije = entity.nutrivne_informacije,
                alergeni = entity.alergeni,
                slika = entity.slika,
                Inventar_id_inventar = entity.Inventar_id_inventar
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
            var pice = Entities.SingleOrDefault(p => p.id_pice == entity.id_pice);

            pice.naziv = entity.naziv;
            pice.cijena = entity.cijena;
            pice.nutrivne_informacije = entity.nutrivne_informacije;
            pice.alergeni = entity.alergeni;
            pice.slika = entity.slika;
            pice.Inventar_id_inventar = entity.Inventar_id_inventar;

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
