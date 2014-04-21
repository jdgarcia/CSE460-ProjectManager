using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class RequirementContext
    {
        public int TenantId { get; set; }
        public int RequirementId { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
        public int StatusId { get; set; }
        public int TypeId { get; set; }
        public int? AssignedUserId { get; set; }

        public string Type { get; set; } 
        public string Status { get; set; }
        public string AssignedUser { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

        // store Statuses that the project is allowed to take on
        public List<ProjectManager.Models.Status> StatusList { get; set; }

        public RequirementContext()
        {

        }

        public RequirementContext(Requirement requirement)
        {
            this.TenantId = requirement.TenantId;
            this.RequirementId = requirement.RequirementId;
            this.Description = requirement.Description;
            this.StatusId = requirement.Status;
            this.TypeId = requirement.TypeId;
            this.AssignedUserId = requirement.AssignedUser;

            this.Type = requirement.RequirementType.Name;
            this.Status = requirement.Status1.Name;
            this.AssignedUser = requirement.User.Username;

            //Get statuses that are available to the user
            StatusList = new List<Models.Status>();
            ProjectManager.Models.DataClassesDataContext db = new ProjectManager.Models.DataClassesDataContext();
            var availableStatuses = (from s in db.Status
                               where s.TenantId == this.TenantId || s.TenantId == 1
                               select s);
            foreach (var status in availableStatuses)
            {
                StatusList.Add(status);
            }

            var project = (from r in db.Requirements
                          join pr in db.ProjectRequirements on r.RequirementId equals pr.RequirementId
                          join p in db.Projects on pr.ProjectId equals p.ProjectId
                          where r.RequirementId == this.RequirementId && r.TenantId == this.TenantId
                          select p).FirstOrDefault();
            
            this.ProjectId = project.ProjectId;
            this.ProjectName = project.Name;

        }
    }
}