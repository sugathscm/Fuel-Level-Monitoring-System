using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class VehicleViewModel
    {
        public int Id { get; set; }
        public string VehicleNo { get; set; }
        public string GPSDeviceSN { get; set; }
        public Nullable<decimal> Capacity { get; set; }
        public Nullable<int> GPSDeviceId { get; set; }
        public bool IsActive { get; set; }

    }
}