using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class NewTenant
    {
        public string OrgName { get; set; }
        public string AdminUserName { get; set; }
        public string AdminPassword { get; set; }
    }
}