using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class GPSDeviceViewModel
    {
        public int Id { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public int SupplierId { get; set; }
        public bool IsActive { get; set; }
        public string SupplierName { get; set; }
    }
}