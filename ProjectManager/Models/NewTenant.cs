﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class NewTenantContext
    {
        public string OrgName { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
    }
}