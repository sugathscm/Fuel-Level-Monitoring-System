using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class DepotTankService
    {
        public List<DepotTank> GetDepotTankList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.DepotTanks
                    .Include("Tank")
                    .Include("Depot")
                    .Include("SensorDevice")
                    .ToList();
            }
        }

        public DepotTank GetDepotTankById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.DepotTanks
                    .Include("Tank")
                    .Include("Depot")
                    .Include("SensorDevice")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(DepotTank depots)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (depots.Id == 0)
                {
                    entities.DepotTanks.Add(depots);
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
