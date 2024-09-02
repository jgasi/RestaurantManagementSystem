using System.Data.Entity;
using System.Linq;
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

        public async Task<int> GetBrojJela()
        {
            var query = from j in Entities
                        select j;

            return await query.CountAsync();
        }

        public IQueryable<Jelo> GetJeloById(int givenId)
        {
            var query = from j in Entities
                        where j.id_jelo == givenId
                        select j;

            return query;
        }

        public IQueryable<Jelo> GetFirstThree()
        {
            var query = from j in Entities
                        orderby j.id_jelo
                        select j;

            return query.Take(3);
        }

        public IQueryable<Jelo> GetJeloByIdInventara(int givenId)
        {
            var query = from j in Entities
                        where j.Inventar_id_inventar == givenId
                        select j;

            return query;
        }

        public IQueryable<Jelo> GetJeloByName(string name)
        {
            var query = from j in Entities
                        where j.naziv.Contains(name)
                        select j;

            return query;
        }

        public IQueryable<Jelo> GetJelaByPage(int pageNumber, int itemsPerPage)
        {
            var query = Entities
                         .OrderBy(j => j.id_jelo)
                         .Skip(pageNumber * itemsPerPage)
                         .Take(itemsPerPage);
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
                Inventar_id_inventar = entity.Inventar_id_inventar
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
            //var inventar = Context.Inventar.SingleOrDefault(i => i.id_inventar == entity.Inventar.id_inventar);
            var jelo = Entities.SingleOrDefault(j => j.id_jelo == entity.id_jelo);

            jelo.naziv = entity.naziv;
            jelo.cijena = entity.cijena;
            jelo.nutrivne_informacije = entity.nutrivne_informacije;
            jelo.alergeni = entity.alergeni;
            jelo.slika = entity.slika;
            jelo.Inventar_id_inventar = entity.Inventar_id_inventar;

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
