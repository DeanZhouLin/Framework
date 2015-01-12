using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace DeanZhou.Framework.Apps.ORMFramework
{
    public sealed class DapperHelper
    {
        private static readonly object LockObj;
        private static readonly Dictionary<string, DapperHelper> DapperHelpers;

        private readonly string _connStr;

        static DapperHelper()
        {
            DapperHelpers = new Dictionary<string, DapperHelper>();
            LockObj = new object();
        }

        private DapperHelper(string connStr)
        {
            _connStr = connStr;
        }

        public static DapperHelper GetInstance(string connStr)
        {
            lock (LockObj)
            {
                if (DapperHelpers.ContainsKey(connStr))
                {
                    return DapperHelpers[connStr];
                }
                DapperHelper dpRes = new DapperHelper(connStr);
                DapperHelpers.Add(connStr, dpRes);
                return dpRes;
            }
        }

        private IDbConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connStr);
            connection.Open();
            return connection;
        }

        public IEnumerable<T> Query<T>(string execSql)
        {
            using (IDbConnection con = OpenConnection())
            {
                return con.Query<T>(execSql);
            }
        }

        public object Execute<T>(string execSql, T t)
        {
            using (IDbConnection con = OpenConnection())
            {
                return con.Execute(execSql, t);
            }
        }
    }

}
