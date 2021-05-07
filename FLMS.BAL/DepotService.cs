using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class DepotService
    {
        public List<Depot> GetDepotList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Depots
                    .Include("Geolocation")
                    .Include("DepotType")
                    .OrderBy(d => d.Code).ToList();
            }
        }

        public Depot GetDepotById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Depots
                    .Include("Geolocation")
                    .Include("DepotType")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Depot depots)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (depots.Id == 0)
                {
                    entities.Depots.Add(depots);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(depots).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

    }

}
