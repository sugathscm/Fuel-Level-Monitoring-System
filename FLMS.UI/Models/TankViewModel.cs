using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class TankViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Nullable<int> Volume { get; set; }
        public Nullable<int> FuelTypeId { get; set; }
        public string FuelTypeName { get; set; }
        public bool IsActive { get; set; }
    }
}