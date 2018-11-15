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


            return SqlWorker.ExecBasicQuery<School>(_dbConn, "[dbo].[Schools_GetAll]");

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

            return SqlWorker.ExecBasicQuery<School>(
                _dbConn,
                "[dbo].[School_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                })
                .SingleOrDefault();
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

            return SqlWorker.ExecBasicQuery<School>(
                _dbConn,
                "[dbo].[School_GetByName]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolName", SqlDbType.NVarChar).Value = @SchoolName;
                })
                .SingleOrDefault();
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


            return SqlWorker.ExecBasicQuery<School>(
                _dbConn,
                "[dbo].[School_GetByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                })
                .SingleOrDefault();
        }


        /// <summary>
        /// Get Db school full detail by email domain. Used to get a user's school at account creation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static School GetSchoolByDomain(string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<School>(
                _dbConn,
                "[dbo].[School_GetByDomain]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                })
                .SingleOrDefault();
        }


        public static bool IsEmailValid(string Email)
        {
            if(!Email.IsValidEmail())
            {
                return false;
            }

            var domain = Email.GetEmailDomain();

            return SqlWorker.ExecScalar<bool>(
                _dbConn,
                "[dbo].[School_IsDomainValid]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = domain;
                });
        }

        public static bool IsDomainValid(string Domain)
        {

            if(Domain.IsEmpty())
            {
                return false;
            }
            if(!Domain.StartsWith("@"))
            {
                return false;
            }


            return SqlWorker.ExecScalar<bool>(
                _dbConn,
                "[dbo].[School_IsDomainValid]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                });
        }
    }
}
