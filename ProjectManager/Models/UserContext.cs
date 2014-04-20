using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class UserContext
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public UserContext(User user)
        {
            this.TenantId = user.TenantId;
            this.UserId = user.UserId;
            this.Username = user.Username;
            this.Password = user.Password;
            this.Role = user.Role.Title;
        }
    }
}