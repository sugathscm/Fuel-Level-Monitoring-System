using FLMS.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class CityViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DistrictName { get; set; }
        public int DistrictId { get; set; }
        public bool IsActive { get; set; }
    }
}