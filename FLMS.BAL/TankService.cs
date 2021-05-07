using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class TankService
    {
        public List<Tank> GetTankList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Tanks.Include("FuelType").OrderBy(d => d.Code).ToList();
            }
        }

        public Tank GetTankById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Tanks
                    .Include("FuelType")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Tank tank)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (tank.Id == 0)
                {
                    entities.Tanks.Add(tank);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(tank).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

    }

}
