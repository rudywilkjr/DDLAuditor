using System.Linq;
using DataLibrary.DataModel;
using System.Collections.Generic;
using DataLibrary.DataModel.DTO;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Configuration;

namespace DataLibrary.Repository
{
    public class UtilityRepository
    {
        private Server _server;
        private Database _database;
        private Scripter _scripter;
        
        public string[] GetDatabaseSchemaNames(string databaseName)
        {
            using (var ctx = new StoneEdgeRogueEntities())
            {
                ctx.Database.Connection.ConnectionString = Core.Database.AllDatabases.Single(x => x.DatabaseName == databaseName).ConnectionString;
                var results =
                    ctx.Database.SqlQuery<string>(
                        "SELECT name FROM " + databaseName + ".sys.schemas where principal_id = 1 order by 1 asc").ToArray();

                return results;
            }
        }

        public string[] GetObjectNames(string databaseName, string schemaName, string typeCode)
        {
            using (var ctx = new StoneEdgeRogueEntities())
            {
                ctx.Database.Connection.ConnectionString = Core.Database.AllDatabases.Single(x => x.DatabaseName == databaseName).ConnectionString;
                var results =
                    ctx.Database.SqlQuery<string>(
                        "USE [" + databaseName + "] " +
                        "SELECT name FROM sys.all_objects where[schema_id] = (select[schema_id] " +
                        "from sys.schemas where [name] = N'" + schemaName + "') " +
                        "and [type] = '" + typeCode + "' " +
                        "order by 1 asc").ToArray();

                return results;
            }

        }

        public List<DatabaseObjectBasicInformation> GetObjectBasicInformationFromDatabaseAndType(string databaseName, DatabaseObjectTypeCode typeCode)
        {
            using (var ctx = new StoneEdgeRogueEntities())
            {

                string sql = string.Format(@"USE [{0}];
                    SELECT  DatabaseName        =   '{0}'
                            ,ObjectName         =   ao.name
                            ,ObjectSchema       =   s.name
                            ,LastDdlChange      =   ao.modify_date
                    FROM    sys.all_objects ao JOIN sys.schemas s ON ao.schema_id = s.schema_id
                    WHERE   [type] = '{1}' AND ao.is_ms_shipped = 0
                    ORDER   BY s.name, ao.name ASC",
                    databaseName,
                    DatabaseTypeCodes[typeCode]);
                List<DatabaseObjectBasicInformation> results =
                    ctx.Database.SqlQuery<DatabaseObjectBasicInformation>(sql).ToList();

                return results;
            }
        }

        public Dictionary<DatabaseObjectTypeCode, string> GetTypesOfObjectsLookup()
        {
            return DatabaseReadableTypeCodesLookup;
        }

        public void GetSmoObject(DatabaseObject dbObject)
        {
            if (_server == null)
            {
                _server = new Server();
                _server.ConnectionContext.LoginSecure = true;
                _server.ConnectionContext.ServerInstance = GetServerNameFromDatabaseName(dbObject.DatabaseName);
            }

            if (_database == null)
            {
                //Assume all databases are the same for now....
                _database = _server.Databases[dbObject.DatabaseName];
            }

            if (_scripter == null)
            {
                _scripter = new Scripter(_server);
                _scripter.Options.ScriptDrops = false;
                _scripter.Options.ScriptData = false;
                _scripter.Options.ScriptSchema = true;
                _scripter.Options.WithDependencies = false;
                _scripter.Options.DriAllConstraints = false;
                _scripter.Options.DriAllKeys = true;
                _scripter.Options.DriNonClustered = true;
                _scripter.Options.DriUniqueKeys = true;
                _scripter.Options.ScriptBatchTerminator = true;
                _scripter.Options.NoCommandTerminator = false;
                _scripter.Options.Statistics = true;
            }

            List<string> script = null;

            switch (dbObject.TypeCode)
            {
                case DatabaseObjectTypeCode.Table:
                    if (_database.Tables[dbObject.ObjectName, dbObject.ObjectSchema] != null)
                    {
                        script = _scripter.EnumScriptWithList(new[] { _database.Tables[dbObject.ObjectName, dbObject.ObjectSchema].Urn }).ToList();
                    }
                    break;
                case DatabaseObjectTypeCode.StoredProcedure:
                    if (_database.StoredProcedures[dbObject.ObjectName, dbObject.ObjectSchema] != null)
                    {
                        script = _scripter.EnumScriptWithList(new[] { _database.StoredProcedures[dbObject.ObjectName, dbObject.ObjectSchema].Urn }).ToList();
                    }
                    break;
                case DatabaseObjectTypeCode.View:
                    if (_database.Views[dbObject.ObjectName, dbObject.ObjectSchema] != null)
                    {
                        script = _scripter.EnumScript(new[] { _database.Views[dbObject.ObjectName, dbObject.ObjectSchema].Urn }).ToList();
                    }
                    break;
                case DatabaseObjectTypeCode.Function:
                    if (_database.UserDefinedFunctions[dbObject.ObjectName, dbObject.ObjectSchema] != null)
                    {
                        script = _scripter.EnumScript(new[] { _database.UserDefinedFunctions[dbObject.ObjectName, dbObject.ObjectSchema].Urn }).ToList();
                    }
                    break;
                default:
                    throw new ArgumentException($"Method GetSmoObject; Invalid category '{dbObject.Category}'");
            }

            if (script != null)
                dbObject.DbSql = $"{string.Join("\nGO\n", script)}\nGO".Replace(
                    @"SET ANSI_NULLS ON\nGO\nSET QUOTED_IDENTIFIER ON\nGO",
                    @"SET ANSI_NULLS ON\r\nSET QUOTED_IDENTIFIER ON\r\nGO").Replace("\n", "").Replace("SET ANSI_NULLS ONGOSET QUOTED_IDENTIFIER ONGO",
                    @"SET ANSI_NULLS ON
                    SET QUOTED_IDENTIFIER ON
                    GO
                    ").Replace("\r\n\r\n", "\r\n");
        }

        private string GetServerNameFromDatabaseName(string databaseName)
        {
            try
            {
                return ConfigurationManager.AppSettings["SomeDatabaseKey"];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"Method GetServerNameFromDatabaseName; Key/DatabaseName ({databaseName}) not found in server lookup");
            }
        }

        public static Dictionary<DatabaseObjectTypeCode, string> DatabaseTypeCodes =
            new Dictionary<DatabaseObjectTypeCode, string>
            {
                                { DatabaseObjectTypeCode.Function, "FN" },
                                { DatabaseObjectTypeCode.StoredProcedure, "P" },
                                { DatabaseObjectTypeCode.Table, "U" },
                                { DatabaseObjectTypeCode.View, "V" }
            };

        public static Dictionary<DatabaseObjectTypeCode, string> DatabaseReadableTypeCodesLookup =
            new Dictionary<DatabaseObjectTypeCode, string>
            {
                { DatabaseObjectTypeCode.Function, "Function" },
                { DatabaseObjectTypeCode.StoredProcedure, "Procedure" },
                { DatabaseObjectTypeCode.Table, "Table" },
                { DatabaseObjectTypeCode.View, "View" }
            };

    }
}
