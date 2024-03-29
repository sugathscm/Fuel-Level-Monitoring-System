﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FLMS.UI.Models
{
    public class DriverViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string NICNo { get; set; }
        public string ContactNo { get; set; }
        public string DLNo { get; set; }
        public bool IsActive { get; set; }
    }
}