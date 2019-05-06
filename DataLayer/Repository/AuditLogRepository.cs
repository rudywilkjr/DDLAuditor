using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DataLayer.DTO;
using DataLayer.Model;
using Microsoft.EntityFrameworkCore;
// ReSharper disable RedundantAnonymousTypePropertyName
// Required in order for SQL to be properly generated on anonymous type joins

namespace DataLayer.Repository
{
    public class AuditLogRepository
    {
        public List<AuditRecord> GetDdlAudits(DateTime minimumDate)
        {
            using (var ctx = new smbimContext())
            {
                return ctx.AuditLog.Where(x => x.PostTime >= minimumDate).Select(m => new AuditRecord
                {
                    DatabaseName = m.DatabaseName,
                    Event = m.Event,
                    Id = m.Id,
                    ObjectName = string.IsNullOrEmpty(m.ParentTable) ? m.ObjectName : m.ParentTable,
                    ObjectType = m.ObjectType,
                    ObjectSchema = m.ObjectSchema,
                    PostDateTime = m.PostTime,
                    User = m.Login
                }).ToList();
            }
        }

        public List<string> GetDdlAutitsForChart(DateTime minimumDate)
        {
            using (var ctx = new smbimContext())
            {
                return ctx.AuditLog.Where(x => x.PostTime >= minimumDate).Select(m => m.Login.Remove(0, 6)).ToList();
            }
        }

        public List<AuditRecord> GetDdlAudits(DateTime minimumDate, string developer)
        {
            using (var ctx = new smbimContext())
            {
                return ctx.AuditLog.Where(x => x.PostTime >= minimumDate && x.Login.Contains(developer)).Select(m => new AuditRecord
                {
                    DatabaseName = m.DatabaseName,
                    Event = m.Event,
                    Id = m.Id,
                    ObjectName = string.IsNullOrEmpty(m.ParentTable) ? m.ObjectName : m.ParentTable,
                    ObjectType = m.ObjectType,
                    ObjectSchema = m.ObjectSchema,
                    PostDateTime = m.PostTime,
                    User = m.Login
                }).ToList();
            }
        }

        public List<AuditLog> GetDdlAudit(int id)
        {
            using (var ctx = new smbimContext())
            {
                return ctx.AuditLog.Where(x => x.Id == id).ToList();
            }
        }

        public List<DatabaseObject> GetUnclaimedDatabaseObjects(string developerUserName, DateTime minimumDateTime )
        {
            using (var ctx = new smbimContext())
            {
                var returnResults = ctx.AuditLog.FromSql($"GetUnclaimedDatabaseObjects {minimumDateTime}, {developerUserName}").AsNoTracking().ToList();

                return returnResults
                    .Select(r => new DatabaseObject
                    {
                        DatabaseName = r.DatabaseName,
                        ObjectSchema = r.ObjectSchema,
                        ObjectName = r.ObjectName,
                        Category = r.ObjectType
                    }).GroupBy(x => new { x.DatabaseName, x.ObjectSchema, x.ObjectName }).Select(y => y.First()).ToList();
            }
        }

        public List<DDLAuditClaimedObjectDomain> GetClaimedObjects(string loginUserName, DateTime minimumDateTime)
        {
            var yesterday = DateTime.Today.AddDays(-1);
            using (var ctx = new smbimContext())
            {
                return
                ctx.ClaimedObjects.Where(
                    x => x.Username.Equals(loginUserName, StringComparison.InvariantCultureIgnoreCase))
                    .Where(x => x.Username.Equals(loginUserName, StringComparison.InvariantCultureIgnoreCase))
                    .Where(z => z.ReleaseDate >= minimumDateTime)
                    .Where(x => x.ProjectLabel.IsJunk != true)
                    .Select(a =>
                    new DDLAuditClaimedObjectDomain
                    {
                        Id = a.Id,
                        ReleaseDate = a.ReleaseDate,
                        Notes = a.Notes,
                        ObjectDatabase = a.ObjectDatabase,
                        ObjectName = a.ObjectName,
                        ObjectSchema = a.ObjectSchema,
                        ObjectType = a.ObjectType,
                        Username = a.Username,
                        Label = ctx.ProjectLabels.Select(l => new ProjectLabelDomain
                        {
                            Id = l.Id,
                            Name = l.Name,
                            ReleaseDate = l.ReleaseDate
                        }).FirstOrDefault(l => l.Id == a.ProjectLabelId)
                    }).ToList();
            }
        }

        public List<DDLAuditClaimedObjectDomain> GetClaimedObjectsByReleaseDate(string loginUserName, DateTime releaseDate)
        {
            using (var ctx = new smbimContext())
            {
                return
                    ctx.ClaimedObjects.Where(
                        x => x.Username.Equals(loginUserName, StringComparison.InvariantCultureIgnoreCase))
                        .Where(z => z.ReleaseDate == releaseDate.Date)
                        .Where(x => x.ProjectLabel.IsJunk != true)
                        .Select(a =>
                        new DDLAuditClaimedObjectDomain
                        {
                            Id = a.Id,
                            ReleaseDate = a.ReleaseDate,
                            Notes = a.Notes,
                            ObjectDatabase = a.ObjectDatabase,
                            ObjectName = a.ObjectName,
                            ObjectSchema = a.ObjectSchema,
                            ObjectType = a.ObjectType,
                            Username = a.Username,
                            Label = ctx.ProjectLabels.Select(l => new ProjectLabelDomain
                            {
                                Id = l.Id,
                                Name = l.Name,
                                ReleaseDate = l.ReleaseDate
                            }).FirstOrDefault(l => l.Id == a.ProjectLabelId)
                        }).ToList();
            }
        }

        public List<DDLAuditClaimedObjectDomain> GetClaimedObjects(DateTime minimumDateTime)
        {
            var yesterday = DateTime.Today.AddDays(-1);
            using (var ctx = new smbimContext())
            {
                return ctx.ClaimedObjects.Where(z => z.ReleaseDate >= minimumDateTime)
                    .Where(x => x.ProjectLabel.IsJunk != true)
                    .Select(a =>
                    new DDLAuditClaimedObjectDomain
                    {
                        Id = a.Id,
                        ReleaseDate = a.ReleaseDate,
                        Notes = a.Notes,
                        ObjectDatabase = a.ObjectDatabase,
                        ObjectName = a.ObjectName,
                        ObjectSchema = a.ObjectSchema,
                        ObjectType = a.ObjectType,
                        Username = a.Username,
                        Label = ctx.ProjectLabels.Select(l => new ProjectLabelDomain
                        {
                            Id = l.Id,
                            Name = l.Name,
                            ReleaseDate = l.ReleaseDate
                        }).FirstOrDefault(l => l.Id == a.ProjectLabelId)
                    }).ToList();
            }
        }

        public List<DDLAuditClaimedObjectDomain> GetClaimedObjectsByReleaseDate(DateTime releaseDate)
        {
            using (var ctx = new smbimContext())
            {
                return ctx.ClaimedObjects.Where(z => !string.IsNullOrEmpty(z.ProjectLabel.ReleaseDate.ToString())
                    ? z.ProjectLabel.ReleaseDate == releaseDate.Date
                    : z.ReleaseDate == releaseDate.Date)
                    .Where(x => x.ProjectLabel.IsJunk != true)
                    .Select(a =>
                    new DDLAuditClaimedObjectDomain
                    {
                        Id = a.Id,
                        ReleaseDate = a.ReleaseDate,
                        Notes = a.Notes,
                        ObjectDatabase = a.ObjectDatabase,
                        ObjectName = a.ObjectName,
                        ObjectSchema = a.ObjectSchema,
                        ObjectType = a.ObjectType,
                        Username = a.Username,
                        Label = ctx.ProjectLabels.Select(l => new ProjectLabelDomain
                        {
                            Id = l.Id,
                            Name = l.Name,
                            ReleaseDate = l.ReleaseDate
                        }).FirstOrDefault(l => l.Id == a.ProjectLabelId)
                    }).ToList();
            }
        }

        public void ClaimAuditRecord(string userName, DateTime? releaseDate, string objectName, string objectSchema,
            string objectDatabase, string objectType, string notes, int projectLabelId)
        {
            if (releaseDate == null && projectLabelId == 0)
            {
                throw new ArgumentException("Either a release date or project label is needed.");
            }
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("User is required.");
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("Object Name is required.");
            if (string.IsNullOrEmpty(objectSchema))
                throw new ArgumentException("Object Schema is required.");
            if (string.IsNullOrEmpty(objectDatabase))
                throw new ArgumentException("Object Database is required.");
            using (var ctx = new smbimContext())
            {
                var claimedObject = new ClaimedObjects
                {
                    Username = userName,
                    ReleaseDate = releaseDate?.Date,
                    ObjectName = objectName,
                    ObjectSchema = objectSchema,
                    ObjectDatabase = objectDatabase,
                    ObjectType = objectType,
                    Notes = notes,
                    ProjectLabelId = projectLabelId == 0 ? (int?)null : projectLabelId
                };
                ctx.ClaimedObjects.Add(claimedObject);
                ctx.SaveChanges();
            }
        }

        public int GetCountOfUserUnclaimedObjects(string username, DateTime minimumDateTime)
        {
            int countOfResults;
            using (var ctx = new smbimContext())
            {
                countOfResults = ctx.AuditLog.FromSql($"GetUnclaimedDatabaseObjects {minimumDateTime}, {username}").Count();
            }

            return countOfResults;
        }

        public int GetCountOfTeamUnclaimedObjects(DateTime minimumDateTime)
        {
            int countOfResults;
            using (var ctx = new smbimContext())
            {
                countOfResults = ctx.AuditLog.FromSql($"GetUnclaimedDatabaseObjects {minimumDateTime}, null").Count();
            }

            return countOfResults;
        }

        public int GetCountOfUserClaimedObjects(string userName, DateTime minimumDateTime)
        {
            int countOfResults;
            var yesterday = DateTime.Today.AddDays(-1);
            using (var ctx = new smbimContext())
            {
                var userDdlRecords = (from ddl in ctx.AuditLog
                                      where ddl.PostTime >= minimumDateTime
                                      select new { ddl.ObjectName, ddl.ObjectSchema, ddl.DatabaseName }).Distinct().AsQueryable();
                var newClaimed = from newToClaim in ctx.ClaimedObjects
                                 where newToClaim.ReleaseDate >= yesterday
                                 where newToClaim.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase)
                                 where newToClaim.ProjectLabel.IsJunk != true
                                 select newToClaim;
                var query = from qry in userDdlRecords
                            join claimed in newClaimed on new
                            {
                                ObjectName = qry.ObjectName,
                                ObjectSchema = qry.ObjectSchema,
                                ObjectDatabase = qry.DatabaseName
                            }
                            equals new
                            {
                                ObjectName = claimed.ObjectName,
                                ObjectSchema = claimed.ObjectSchema,
                                ObjectDatabase = claimed.ObjectDatabase
                            }
                            select qry;

                countOfResults = query.Distinct().Count();
            }

            return countOfResults;
        }

        public int GetCountOfTeamClaimedObjects(DateTime minimumDateTime)
        {
            int countOfResults;
            var yesterday = DateTime.Today.AddDays(-1);
            using (var ctx = new smbimContext())
            {
                var userDdlRecords = (from ddl in ctx.AuditLog
                                      select new { ddl.ObjectName, ddl.ObjectSchema, ddl.DatabaseName }).Distinct().AsQueryable();
                var newClaimed = from newToClaim in ctx.ClaimedObjects
                                 where newToClaim.ReleaseDate >= minimumDateTime
                                 where newToClaim.ReleaseDate >= yesterday
                                 where newToClaim.ProjectLabel.IsJunk != true
                                 select newToClaim;
                var query = from qry in userDdlRecords
                            join claimed in newClaimed on new
                            {
                                ObjectName = qry.ObjectName,
                                ObjectSchema = qry.ObjectSchema,
                                ObjectDatabase = qry.DatabaseName
                            }
                            equals new
                            {
                                ObjectName = claimed.ObjectName,
                                ObjectSchema = claimed.ObjectSchema,
                                ObjectDatabase = claimed.ObjectDatabase
                            }
                            select qry;

                countOfResults = query.Distinct().Count();
            }

            return countOfResults;
        }

        //ToDo: Change to LINQ SQL or Stored Proc
        public List<DatabaseObject> GetClaimedDatabaseObjects(DateTime minimumDateTime)
        {
            using (var ctx = new smbimContext())
            {
                throw new NotImplementedException();
            }
        }

        //ToDo: Change to LINQ SQL or Stored Proc
        public List<DatabaseObject> GetClaimedDatabaseObjects(string userName, DateTime minimumDateTime)
        {
            using (var ctx = new smbimContext())
            {
                //Select(d =>
                //new DatabaseObject
                //{
                //    DatabaseName = d.DatabaseName,
                //    LastDdlChange = d.LastDdlChange,
                //    ObjectSchema = d.ObjectSchema,
                //    ObjectName = d.ObjectName
                //}
                //).ToList();
                throw new NotImplementedException();
            }
        }

        public List<DatabaseObjectStatus> GetObjectsWithUnclimainedChangesByDatabaseAndType(string databaseName, DatabaseObjectTypeCode type, DateTime startDate, string userName)
        {
            List<DatabaseObjectStatus> statusList = new List<DatabaseObjectStatus>();
            using (var ctx = new smbimContext())
            {
                var unclaimedChanges = ctx.AuditLog.FromSql($"GetUnclaimedDatabaseObjects {startDate}, {userName}").
                    Where(u => string.Compare(u.ObjectType, UtilityRepository.DatabaseReadableTypeCodesLookup[type], StringComparison.OrdinalIgnoreCase) == 0).ToList();
                statusList.AddRange(unclaimedChanges.Select(change => new DatabaseObjectStatus
                {
                    ObjectInformation = new DatabaseObjectBasicInformation
                    {
                        DatabaseName = databaseName,
                        ObjectName = change.ObjectName,
                        ObjectSchema = change.ObjectSchema
                    },
                    HasUnclaimedChanges = true
                }));
                return statusList;
            }
        }

        public void DeleteClaimedObject(int claimedObjectId)
        {
            using (var ctx = new smbimContext())
            {
                try
                {
                    var claimedObject = ctx.ClaimedObjects.Single(x => x.Id == claimedObjectId);
                    ctx.ClaimedObjects.Remove(claimedObject);
                    ctx.SaveChanges();
                }
                catch (InvalidOperationException)
                {
                    throw new ApplicationException("Claimed Object does not exist");
                }

            }
        }

        public List<IgnoredDatabaseObject> GetIgnoredDatabaseObjects()
        {
            var dateWindow = DateTime.Now.AddMonths(-1);
            using (var ctx = new smbimContext())
            {
                return
                    ctx.IgnoredObjects.Where(x => x.IgnoredByTime >= dateWindow)
                        .Select(x => new IgnoredDatabaseObject
                        {
                            Id = x.Id,
                            ObjectName = x.ObjectName,
                            ObjectSchema = x.ObjectSchema,
                            ObjectDatabase = x.ObjectDatabase,
                            IgnoredByUser = x.IgnoredByUser,
                            IgnoredByTime = x.IgnoredByTime
                        }).ToList();
            }
        }

        public void AddIgnoredDatabaseObject(string objectName, string objectSchema, string objectDatabase,
            string userName)
        {
            using (var ctx = new smbimContext())
            {
                var ignoredObjectRecord = new IgnoredObjects
                {
                    IgnoredByTime = DateTime.Now,
                    IgnoredByUser = userName,
                    ObjectName = objectName,
                    ObjectSchema = objectSchema,
                    ObjectDatabase = objectDatabase
                };

                ctx.IgnoredObjects.Add(ignoredObjectRecord);
                ctx.SaveChanges();
            }
        }

        public void DeleteIgnoredDatabaseObject(int id)
        {
            using (var ctx = new smbimContext())
            {
                try
                {
                    var ignoredObjectRecord = ctx.IgnoredObjects.Single(x => x.Id == id);

                    ctx.IgnoredObjects.Remove(ignoredObjectRecord);
                    ctx.SaveChanges();
                }
                catch (InvalidOperationException)
                {
                    throw new ApplicationException("Ignored Object Record Not Found.");
                }

            }
        }
    }
}
