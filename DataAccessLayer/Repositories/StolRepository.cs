using System.Linq;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class StolRepository : Repository<Stol>
    {
        public StolRepository() : base (new RestaurantDatabaseModel())
        {
            
        }

        public override IQueryable<Stol> GetAll()
        {
            var query = from s in Entities
                        select s;

            return query;
        }

        public IQueryable<Stol> GetSlobodneStolove()
        {
            var query = from s in Entities
                        where s.status == "slobodan"
                        select s;

            return query;
        }

        public override int Add(Stol entity, bool saveChanges = true)
        {
            var stol = new Stol
            {
                broj_stola = entity.broj_stola,
                status = entity.status
            };

            Entities.Add(stol);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Stol entity, bool saveChanges = true)
        {
            var stol = Entities.SingleOrDefault(s => s.id_stol == entity.id_stol);

            stol.broj_stola = entity.broj_stola;
            stol.status = entity.status;

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
