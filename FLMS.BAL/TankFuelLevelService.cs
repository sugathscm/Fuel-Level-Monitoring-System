using FLMS.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLMS.BAL
{
    public class TankFuelLevelService
    {

        public string GetFuelLevels(int cityId)
        {
            string levels = "";

            using (FLMSEntities entities = new FLMSEntities())
            {
                var lastBatchId = entities.TankFuelLevelInputBatches.OrderByDescending(b => b.Id).Take(1).SingleOrDefault().Id;

                var depotTankLevels = entities.TankFuelLevels
                   .Include("DepotTank").Where(l => l.TankFuelLevelInputBatchId == lastBatchId)
                   .ToList();

                if(cityId != 0)
                {
                    depotTankLevels = depotTankLevels.Where(d => d.DepotTank.Depot.Geolocation.City.Id == cityId).ToList();
                }

                List<TamkFuelLveleModel> levelModels = new List<TamkFuelLveleModel>();

                foreach(var depotTankLevel in depotTankLevels)
                {
                    levelModels.Add(new TamkFuelLveleModel()
                    {
                        TankCode = depotTankLevel.DepotTank.Tank.Code,
                        Level = ((depotTankLevel.CurrentVolume/ depotTankLevel.DepotTank.Tank.Volume) * 100).Value.ToString("#.00"),
                        Location = depotTankLevel.DepotTank.Depot.Geolocation.City.Name,
                        Volume = depotTankLevel.CurrentVolume.ToString()
                    });
                }

                levels = JsonConvert.SerializeObject(levelModels);

                return levels;
            }
        }


        public class TamkFuelLveleModel
        {
            public string TankCode { get; set; }
            public string Level { get; set; }
            public string Location { get; set; }
            public string Volume { get; set; }
        }
    }
}
