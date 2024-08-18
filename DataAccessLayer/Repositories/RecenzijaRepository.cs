using System.Linq;
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

        public IQueryable<Recenzija> GetRecenzijeByKorisnikId(int proslijedenId)
        {
            var query = from r in Entities
                        where r.Korisnik_id_korisnik == proslijedenId
                        select r;

            return query;
        }

        public IQueryable<Recenzija> GetRecenzijeById(int proslijedenId)
        {
            var query = from r in Entities
                        where r.Jelo_id_jelo == proslijedenId
                        select r;

            return query;
        }

        public IQueryable<Recenzija> GetRecenzijePicaById(int proslijedenId)
        {
            var query = from r in Entities
                        where r.Pice_id_pice == proslijedenId
                        select r;

            return query;
        }

        public override int Add(Recenzija entity, bool saveChanges = true)
        {

            var recenzija = new Recenzija
            {
                ocjena = entity.ocjena,
                komentar = entity.komentar,
                Korisnik_id_korisnik = entity.Korisnik_id_korisnik,
                Jelo_id_jelo = entity.Jelo_id_jelo,
                Pice_id_pice = entity.Pice_id_pice
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
