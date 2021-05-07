using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class DistrictViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProvinceName { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public bool IsActive { get; set; }
    }
}