using System.Linq;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class StatistikaRepository : Repository<Statistika>
    {
        public StatistikaRepository() : base (new RestaurantDatabaseModel())
        {
            
        }

        public override IQueryable<Statistika> GetAll()
        {
            var query = from s in Entities
                        select s;

            return query;
        }

        public override int Add(Statistika entity, bool saveChanges = true)
        {
            var statistika = new Statistika
            {
                datum_vrijeme = entity.datum_vrijeme,
                broj_prodanih_jedinica = entity.broj_prodanih_jedinica,
                ukupni_prihod = entity.ukupni_prihod,
                prosjecna_ocjena = entity.prosjecna_ocjena
            };

            Entities.Add(statistika);
            if(saveChanges)
            {
                return SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        public override int Update(Statistika entity, bool saveChanges = true)
        {
            var statistika = Entities.SingleOrDefault(s => s.id_statistika == entity.id_statistika);

            statistika.datum_vrijeme = entity.datum_vrijeme;
            statistika.broj_prodanih_jedinica = entity.broj_prodanih_jedinica;
            statistika.ukupni_prihod = entity.ukupni_prihod;
            statistika.prosjecna_ocjena = entity.prosjecna_ocjena;

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
