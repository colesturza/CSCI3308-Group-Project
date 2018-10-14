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

namespace UHub.CoreLib.Entities.SchoolClubs.Management
{
    public static partial class SchoolClubReader
    {
        private static string _dbConn = null;

        static SchoolClubReader()
        {
            _dbConn = CoreFactory.Singleton.Properties.CmsDBConfig;
        }

        #region Individual
        /// <summary>
        /// Get DB school club full detail by GUID UID
        /// </summary>
        /// <param name="SchoolMajorID"></param>
        /// <returns></returns>
        public static SchoolClub GetSchoolClub(long SchoolClubID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[SchoolClub_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<SchoolClub>();
                }).SingleOrDefault();
        }
        #endregion Individual

        #region Group
        /// <summary>
        /// Get all the school clubs in the DB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SchoolClub> GetSchoolClubs()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[SchoolClubs_GetAll]",
                (cmd) => { },
                (row) =>
                {
                    return row.ToCustomDBType<SchoolClub>();
                });
        }

        /// <summary>
        /// Get all the school clubs in the DB
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SchoolClub> GetSchoolClubsBySchool(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return SqlWorker.ExecBasicQuery(
                _dbConn,
                "[dbo].[SchoolClubs_GetBySchool]",
                (cmd) => {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                },
                (row) =>
                {
                    return row.ToCustomDBType<SchoolClub>();
                });
        }
        #endregion Group
    }
}
