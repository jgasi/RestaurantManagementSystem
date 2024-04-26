using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;

namespace BusinessLogicLayer.Services
{
    public class InformacijeServices
    {
        public List<Informacije> GetAllInformacije()
        {
            using(var repo = new InformacijeRepository())
            {
                List<Informacije> informacije = repo.GetAll().ToList();
                return informacije;
            }
        }

        public List<Informacije> GetInformacijeByName(string phrase) 
        {
            using (var repo = new InformacijeRepository())
            {
                List<Informacije> informacije = repo.GetInfoByName(phrase).ToList();
                return informacije;
            }
        }

        public bool AddInformacije(Informacije informacije) 
        {
            bool isSuccessful = false;
            
            using (var repo = new InformacijeRepository())
            {
                int affectedRows = repo.Add(informacije);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool UpdateInformacije(Informacije informacije)
        {
            bool isSuccessful = false;

            using (var repo = new InformacijeRepository())
            {
                int affectedRows = repo.Update(informacije);
                isSuccessful = affectedRows > 0;
            }

            return isSuccessful;
        }

        public bool RemoveInformacije(Informacije informacije)
        {
            bool isSuccessful = false;
            //bool canRemove = CheckIfCanBeRemoved(informacije);
            //if (canRemove) { 

            using(var repo = new InformacijeRepository())
            {
                int affectedRows = repo.Remove(informacije);
                isSuccessful = affectedRows > 0;
            }
            //}
            return isSuccessful;
        }

        //private bool CheckIfCanBeRemoved(Informacije informacije)
       // {
         //   if(informacije == null || informacije.naziv == )
         //   {
         //       return false;
         //   }
          //  else
          //  {
          //      return true;
          //  }
        //}
    }
}
