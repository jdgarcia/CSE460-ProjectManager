﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class CurrentUserContext
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class LoginContext
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
