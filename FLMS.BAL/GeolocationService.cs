using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class GeolocationService
    {
        public List<Geolocation> GetGeolocationList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Geolocations.Include("City").OrderBy(d => d.Address).ToList();
            }
        }

        public Geolocation GetGeolocationById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Geolocations
                    .Include("City")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Geolocation geolocation)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (geolocation.Id == 0)
                {
                    entities.Geolocations.Add(geolocation);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(geolocation).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

    }

}
