using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class GPSDeviceService
    {
        public List<GPSDevice> GetGPSDeviceList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.GPSDevices.Include("Supplier").OrderBy(d => d.SerialNumber).ToList();
            }
        }

        public GPSDevice GetGPSDeviceById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.GPSDevices
                    .Include("Supplier")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(GPSDevice district)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (district.Id == 0)
                {
                    entities.GPSDevices.Add(district);
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
