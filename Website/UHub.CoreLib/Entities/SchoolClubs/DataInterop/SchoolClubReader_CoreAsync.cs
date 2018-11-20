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

namespace UHub.CoreLib.Entities.SchoolClubs.DataInterop
{
    public static partial class SchoolClubReader
    {

        #region Individual
        /// <summary>
        /// Get DB school club full detail by LONG ID
        /// </summary>
        /// <param name="SchoolClubID"></param>
        /// <returns></returns>
        public static async Task<SchoolClub> GetClubAsync(long SchoolClubID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var temp = SqlWorker.ExecBasicQueryAsync<SchoolClub>(
                _dbConn,
                "[dbo].[SchoolClub_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolClubID", SqlDbType.BigInt).Value = SchoolClubID;
                });


            return (await temp).SingleOrDefault();
        }
        #endregion Individual

        #region Group
        /// <summary>
        /// Get all the school clubs in the DB
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolClub>> GetAllClubsAsync()
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync<SchoolClub>(
                _dbConn,
                "[dbo].[SchoolClubs_GetAll]");
        }

        /// <summary>
        /// Get all the school clubs in the DB
        /// </summary>
        /// <param name="SchoolID"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolClub>> GetClubsBySchoolAsync(long SchoolID)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync<SchoolClub>(
                _dbConn,
                "[dbo].[SchoolClubs_GetBySchool]",
                (cmd) => {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                });
        }

        /// <summary>
        /// Get all the school clubs in the DB for a school using the email addr of a user
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolClub>> GetClubsByEmailAsync(string Email)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Email.IsValidEmail())
            {
                return Enumerable.Empty<SchoolClub>();
            }


            return await SqlWorker.ExecBasicQueryAsync<SchoolClub>(
                _dbConn,
                "[dbo].[SchoolClubs_GetByEmail]",
                (cmd) => {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                });
        }


        /// <summary>
        /// Get all the school clubs in the DB for a school using the school domain
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<SchoolClub>> GetClubsByDomainAsync(string Domain)
        {

            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            if (!Domain.IsValidEmailDomain())
            {
                return Enumerable.Empty<SchoolClub>();
            }


            return await SqlWorker.ExecBasicQueryAsync<SchoolClub>(
                _dbConn,
                "[dbo].[SchoolClubs_GetByDomain]",
                (cmd) => {
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                });
        }
        #endregion Group

    }
}
