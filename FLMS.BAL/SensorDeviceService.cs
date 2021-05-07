using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class SensorDeviceService
    {
        public List<SensorDevice> GetSensorDeviceList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.SensorDevices.Include("Supplier").OrderBy(d => d.SerialNumber).ToList();
            }
        }

        public SensorDevice GetSensorDeviceById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.SensorDevices
                    .Include("Supplier")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(SensorDevice district)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (district.Id == 0)
                {
                    entities.SensorDevices.Add(district);
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
