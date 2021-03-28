using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace FLMS.BAL
{
    public class GenericService
    {
        public List<T> GetList<T>() where T : class
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                return entities.Set<T>().ToList();
            }
        }

        public void SaveOrUpdate<T>(T t, int id) where T : class
        {
            using (FLMSEntities entities = new FLMSEntities())
            {
                if (id == 0)
                {
                    entities.Set<T>().Add(t);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(t).State = EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
    }
}
