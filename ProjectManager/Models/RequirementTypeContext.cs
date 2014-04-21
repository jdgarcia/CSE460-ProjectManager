using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProjectManager.Utils;

namespace ProjectManager.Models
{
    public class RequirementTypeContext
    {
        public int TenantId { get; set; }
        public int TypeId { get; set; }
        public string Name { get; set; }

        // projects with a requirement of matching TypeId
        public List<ProjectContext> MatchedProjects { get; set; }

        public RequirementTypeContext() { }

        public RequirementTypeContext(RequirementType rType)
        {
            this.TenantId = rType.TenantId;
            this.TypeId = rType.TypeId;
            this.Name = rType.Name;

            this.MatchedProjects = DbUtils.GetProjectsByRequirementType(rType.TypeId);
        }
    }
}