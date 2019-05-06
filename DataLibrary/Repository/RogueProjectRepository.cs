using DataLibrary.DataModel;
using DataLibrary.DataModel.DTO;
using System.Collections.Generic;
using System.Linq;

namespace DataLibrary.Repository
{
    public class RogueProjectRepository
    {
        public List<ProjectLabelDomain> GetProjectLabels()
        {
            using (var ctx = new smbimAuditEntities())
            {
                return ctx.ProjectLabels.Select(p =>
                new ProjectLabelDomain
                {
                    Id = p.Id,
                    Name = p.Name,
                    ReleaseDate = p.ReleaseDate
                }).OrderBy(p => p.Name).ToList();
            }
        }

        public void AddLabel(ProjectLabelDomain label)
        {
            if (label == null) return;
            using (var ctx = new smbimAuditEntities())
            {
                var insertLabel = new ProjectLabel
                {
                    Name = label.Name,
                    ReleaseDate = label.ReleaseDate,
                    IsJunk = label.IsJunk
                };
                ctx.ProjectLabels.Add(insertLabel);
                ctx.SaveChanges();
            }
        }

        public void DeleteLabel(ProjectLabelDomain label)
        {
            if (label != null)
            {
                using (var ctx = new smbimAuditEntities())
                {
                    ProjectLabel deleteLabel = ctx.ProjectLabels.FirstOrDefault(l => l.Id == label.Id);
                    if (deleteLabel != null)
                    {
                        ctx.ProjectLabels.Remove(deleteLabel);
                        ctx.SaveChanges();
                    }
                }
            }
        }

        public void UpdateLabel(ProjectLabelDomain label)
        {
            if (label == null) return;
            using (var ctx = new smbimAuditEntities())
            {
                var editLabel = ctx.ProjectLabels.FirstOrDefault(l => l.Id == label.Id);
                if (editLabel == null) return;
                editLabel.Name = label.Name;
                editLabel.ReleaseDate = label.ReleaseDate;
                editLabel.IsJunk = label.IsJunk;
                ctx.SaveChanges();
            }
        }
    }
}
