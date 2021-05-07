using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class CityService
    {
        public List<City> GetCityList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Cities.Include("District").OrderBy(d => d.Name).ToList();
            }
        }

        public City GetCityById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Cities
                    .Include("District")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(City city)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (city.Id == 0)
                {
                    entities.Cities.Add(city);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(city).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

    }

}
