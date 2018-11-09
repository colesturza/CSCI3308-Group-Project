﻿using System;
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

        public static async Task<IEnumerable<School>> GetAllSchoolsAsync()
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            return await SqlWorker.ExecBasicQueryAsync(
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
        public static async Task<School> GetSchoolAsync(long SchoolID)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var temp = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[School_GetByID]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolID", SqlDbType.BigInt).Value = SchoolID;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<School>();
                });


            return temp.SingleOrDefault();
        }


        /// <summary>
        /// Get Db school full detail by Name
        /// </summary>
        /// <param name="ID">School ID</param>
        /// <returns></returns>
        public static async Task<School> GetSchoolByNameAsync(string SchoolName)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var temp = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[School_GetByName]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@SchoolName", SqlDbType.NVarChar).Value = @SchoolName;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<School>();
                });

            return temp.SingleOrDefault();
        }



        /// <summary>
        /// Get Db school full detail by user email. Used to get a user's school at account creation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<School> GetSchoolByEmailAsync(string Email)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }


            var temp = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[School_GetByEmail]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = Email;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<School>();
                });

            return temp.SingleOrDefault();
        }


        /// <summary>
        /// Get Db school full detail by email domain. Used to get a user's school at account creation
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public static async Task<School> GetSchoolByDomainAsync(string Domain)
        {
            if (!CoreFactory.Singleton.IsEnabled)
            {
                throw new SystemDisabledException();
            }

            var temp = await SqlWorker.ExecBasicQueryAsync(
                _dbConn,
                "[dbo].[School_GetByDomain]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                },
                (reader) =>
                {
                    return reader.ToCustomDBType<School>();
                });
            
            return temp.SingleOrDefault();
        }


        public static async Task<bool> IsEmailValidAsync(string Email)
        {
            if (!Email.IsValidEmail())
            {
                return false;
            }


            var domain = Email.GetEmailDomain();


            return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[School_IsDomainValid]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = domain;
                });
        }

        public static async Task<bool> IsDomainValidAsync(string Domain)
        {

            if (Domain.IsEmpty())
            {
                return false;
            }
            if (!Domain.StartsWith("@"))
            {
                return false;
            }


            return await SqlWorker.ExecScalarAsync<bool>(
                _dbConn,
                "[dbo].[School_IsDomainValid]",
                (cmd) =>
                {
                    cmd.Parameters.Add("@Domain", SqlDbType.NVarChar).Value = Domain;
                });
        }
    }
}