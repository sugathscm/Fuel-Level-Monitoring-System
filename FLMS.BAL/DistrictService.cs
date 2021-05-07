using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class DistrictService
    {
        public List<District> GetDistrictList()
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Districts.Include("Province").OrderBy(d => d.Name).ToList();
            }
        }

        public District GetDistrictById(int? id)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Districts
                    .Include("Province")
                    .Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(District district)
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (district.Id == 0)
                {
                    entities.Districts.Add(district);
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
