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

namespace UHub.CoreLib.Entities.SchoolMajors.DataInterop
{
    public static partial class SchoolMajorReader
    {
        private static string _dbConn = null;

        static SchoolMajorReader()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

        #region Individual
        /// <summary>
        /// Get DB school major full detail by LONG ID
        /// </summary>
        /// <param name="SchoolMajorID"></param>
        /// <returns></returns>
        public static SchoolMajor GetMajor(long SchoolMajorID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<SchoolMajor>(
                _dbConn,
                "[dbo].[SchoolMajor_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolMajorID", SqlDbType.BigInt).Value = SchoolMajorID;
                })
                .SingleOrDefault();
        }
        #endregion Individual

        #region Group
        /// <summary>
        /// Get all the school majors in the DB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SchoolMajor> GetAllMajors()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<SchoolMajor>(_dbConn, "[dbo].[SchoolMajors_GetAll]");
        }

        /// <summary>
        /// Get all the school majors in the DB by school
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolMajor> GetMajorsBySchool(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery<SchoolMajor>(
                _dbConn,
                "[dbo].[SchoolMajors_GetBySchool]",
                (cmd) => {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                });
        }


        /// <summary>
        /// Get all the school majors in the DB for a school using the email addr of a user
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolMajor> GetMajorsByEmail(string Email)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Email.IsValidEmail())
            {
                return Enumerable.Empty<SchoolMajor>();
            }

            return SqlWorker.ExecBasicQuery<SchoolMajor>(
                _dbConn,
                "[dbo].[SchoolMajors_GetByEmail]",
                (cmd) => {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                });
        }

        /// <summary>
        /// Get all the school majors in the DB for a school using the school domain
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static IEnumerable<SchoolMajor> GetMajorsByDomain(string Domain)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Domain.IsValidEmailDomain())
            {
                return Enumerable.Empty<SchoolMajor>();
            }

            return SqlWorker.ExecBasicQuery<SchoolMajor>(
                _dbConn,
                "[dbo].[SchoolMajors_GetByDomain]",
                (cmd) => {
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                });
        }

        #endregion Group
    }
}
