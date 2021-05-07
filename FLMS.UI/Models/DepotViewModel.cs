using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class DepotViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Nullable<int> GeolocationId { get; set; }
        public Nullable<int> DepotTypeId { get; set; }
        public string GeolocationName { get; set; }
        public string DepotTypeName { get; set; }
        public bool IsActive { get; set; }
    }
}