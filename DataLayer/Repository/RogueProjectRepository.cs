using DataLayer.DTO;
using DataLayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataLayer.Repository
{
    public class RogueProjectRepository
    {
        public List<ProjectLabelDomain> GetProjectLabels()
        {
            using (var ctx = new smbimContext())
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
            using (var ctx = new smbimContext())
            {
                var insertLabel = new ProjectLabels
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
                using (var ctx = new smbimContext())
                {
                    ProjectLabels deleteLabel = ctx.ProjectLabels.FirstOrDefault(l => l.Id == label.Id);
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
            using (var ctx = new smbimContext())
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
