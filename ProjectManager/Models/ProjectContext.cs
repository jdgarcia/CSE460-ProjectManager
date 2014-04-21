using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectManager.Models
{
    public class ProjectContext
    {
        public int TenantId { get; set; }
        public int ProjectId { get; set; }
        public int ManagerId { get; set; }
        public int StatusId { get; set; }
        public string Name { get; set; }
        public string Start { get; set; }
        public string ExpectedEnd { get; set; }
        public DateTime RawDateStart { get; set; }
        public DateTime RawDateEnd { get; set; }
        
        // store the string values of status and manager, instead of the Id int
        public string Status { get; set; }
        public string Manager { get; set; }
        // store Statuses that the project is allowed to take on
        public List<ProjectManager.Models.Status> StatusList { get; set; }

        public ProjectContext()
        {

        }

        public ProjectContext(Project project)
        {
            this.TenantId = project.TenantId;
            this.ProjectId = project.ProjectId;
            this.Name = project.Name;
            this.RawDateStart = (DateTime)project.Start;
            this.RawDateEnd = (DateTime)project.ExpectedEnd;
            this.StatusId = project.Status;
            this.ManagerId = project.ManagerId;

            //Format date time in preferred output style
            this.Start = ((DateTime)(project.Start)).ToString("MM/dd/yyyy");
            this.ExpectedEnd = ((DateTime)(project.ExpectedEnd)).ToString("MM/dd/yyyy");
            this.Status = project.Status1.Name;
            this.Manager = project.User.Username;

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
        }
    }
}