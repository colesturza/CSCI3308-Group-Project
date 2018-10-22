using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.ErrorHandling.Exceptions;
using UHub.CoreLib.Extensions;
using UHub.CoreLib.Management;

namespace UHub.CoreLib.Entities.Schools.Management
{
    public static partial class SchoolReader
    {
        private static string _dbConn = null;

        static SchoolReader()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

        public static IEnumerable<School> GetAllSchools()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[Schools_GetAll]",
                (cmd) => { },
                (reader) =>
                {
                    return reader.ToCustomDBType<School>();
                });

        }


        /// <summary>
        /// Get Db school full detail by ID
        /// </summary>
        /// <param name="ID">School ID</param>
        /// <returns></returns>
        public static School GetSchool(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[School_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<School>();
                }).SingleOrDefault();
        }


        /// <summary>
        /// Get Db school full detail by Name
        /// </summary>
        /// <param name="ID">School ID</param>
        /// <returns></returns>
        public static School GetSchoolByName(string SchoolName)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[School_GetByName]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolName", SqlDbType.NVarChar).Value = @SchoolName;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<School>();
                }).SingleOrDefault();
        }



        /// <summary>
        /// Get Db school full detail by user email. Used to get a user's school at account creation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static School GetSchoolByEmail(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[School_GetByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<School>();
                }).SingleOrDefault();
        }

    }
}
