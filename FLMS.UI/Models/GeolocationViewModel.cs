using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class GeolocationViewModel
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public int CityId { get; set; }
        public bool IsActive { get; set; }
    }
}