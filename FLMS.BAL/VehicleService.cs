using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class VehicleService
    {
        public List<Vehicle> GetVehicleList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Vehicles.Include("GPSDevice").OrderBy(d => d.VehicleNo).ToList();
            }
        }

        public Vehicle GetVehicleById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Vehicles
                    .Include("GPSDevice")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Vehicle district)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (district.Id == 0)
                {
                    entities.Vehicles.Add(district);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(district).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
