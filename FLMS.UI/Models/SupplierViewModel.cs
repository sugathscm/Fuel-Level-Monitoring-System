using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class SupplierViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public string Website { get; set; }
        public string ContactDetails { get; set; }
    }
}