using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class DepotTankViewModel
    {
        public int Id { get; set; }
        public Nullable<int> SensorDeviceId { get; set; }
        public Nullable<int> TankId { get; set; }
        public Nullable<int> DepotId { get; set; }
        public string SensorDeviceSN { get; set; }
        public string TankCode { get; set; }
        public string DepotCode { get; set; }
    }
}