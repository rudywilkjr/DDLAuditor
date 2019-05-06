using System;
using Microsoft.SqlServer.Management.Smo;

namespace DataLibrary.Repository
{
    public class ObjectManagement
    {
        public const string ServerName = @"S-SW-SQLDEV2\Release";

        public string GetObjectScript(string objectName, string schema, string objectType, string database)
        {
            switch (objectType.ToUpper())
            {
                case "TABLE":
                    return GetTableScript(objectName, schema, database);

                case "PROCEDURE":
                    return GetStoredProcedureScript(objectName, schema, database);

                case "VIEW":
                    return GetViewScript(objectName, schema, database);

                case "FUNCTION":
                    return GetFunctionScript(objectName, schema, database);

                default:
                    throw new ApplicationException("Unknown Object Type");

            }
        }
        private string GetTableScript(string table, string schema, string database)
        {
            Server srv = new Server(ServerName);
            Database db = srv.Databases[database];
            Scripter scrp = new Scripter(srv)
            {
                Options =
                {
                    ScriptDrops = false,
                    WithDependencies = false,
                    Indexes = true,
                    DriAllConstraints = true
                }
            };
            var script = "";
            foreach (var line in scrp.Script(new [] { db.Tables[table, schema].Urn }))
            {
                script += line + Environment.NewLine;
            }
            return script;
        }

        private string GetStoredProcedureScript(string procedureName, string schema, string database)
        {
            Server srv = new Server(ServerName);
            Database db = srv.Databases[database];
            Scripter scrp = new Scripter(srv)
            {
                Options =
                {
                    ScriptDrops = false,
                    WithDependencies = false,
                    Indexes = true,
                    DriAllConstraints = true
                }
            };
            var script = "";
            foreach (var line in scrp.Script(new [] { db.StoredProcedures[procedureName, schema].Urn }))
            {
                script += line + Environment.NewLine;
            }
            return script;
        }

        private string GetFunctionScript(string functionName, string schema, string database)
        {
            Server srv = new Server(ServerName);
            Database db = srv.Databases[database];
            Scripter scrp = new Scripter(srv)
            {
                Options =
                {
                    ScriptDrops = false,
                    WithDependencies = false,
                    Indexes = true,
                    DriAllConstraints = true
                }
            };
            var script = "";
            foreach (var line in scrp.Script(new [] { db.UserDefinedFunctions[functionName, schema].Urn }))
            {
                script += line + Environment.NewLine;
            }
            return script;
        }

        private string GetViewScript(string viewName, string schema, string database)
        {
            Server srv = new Server(ServerName);
            Database db = srv.Databases[database];
            Scripter scrp = new Scripter(srv)
            {
                Options =
                {
                    ScriptDrops = false,
                    WithDependencies = false,
                    Indexes = true,
                    DriAllConstraints = true
                }
            };
            var script = "";
            foreach (var line in scrp.Script(new [] { db.Views[viewName, schema].Urn }))
            {
                script += line + Environment.NewLine;
            }
            return script;
        }

    }
}
