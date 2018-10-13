﻿  


//FILE GENERATED BY A TOOL


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using UHub.CoreLib.Extensions;
using UHub.CoreLib.DataInterop;
using UHub.CoreLib.Attributes;



namespace UHub.CoreLib.Entities.Schools 
			{						
						///<summary>
						///AutoGenerated DataConverters for School
						///</summary>
						partial class School
						{

						/// <summary>
						/// Convert DataRow to School
						/// </summary>
						/// <param name="row">The DataRow that will be converted into an object</param>
						public static explicit operator UHub.CoreLib.Entities.Schools.School(DataRow row)
						{
							var columnSet = row.Table.Columns;
							var obj = new School();
														obj.ID = row.Field<System.Int64?>("ID");
																obj.Name = row.Field<System.String>("Name");
																obj.State = row.Field<System.String>("State");
																obj.City = row.Field<System.String>("City");
																obj.IsEnabled = row.Field<System.Boolean>("IsEnabled");
																obj.IsReadOnly = row.Field<System.Boolean>("IsReadOnly");
																obj.Description = row.Field<System.String>("Description");
																obj.CreatedDate = row.Field<System.DateTimeOffset>("CreatedDate");
																obj.ModifiedDate = row.Field<System.DateTimeOffset>("ModifiedDate");
																obj.DomainValidator = row.Field<System.String>("DomainValidator");
														return obj;
						}
						
						///<summary>
						///Convert SqlDataReader to School
						///</summary>
						/// <param name="reader">The SqlDataReader that is streaming the dataset to be converted.  With convert the row at the current iterator position</param>
						public static explicit operator UHub.CoreLib.Entities.Schools.School(SqlDataReader reader)
						{
							var columnSet = reader.GetSchemaTable().Columns;
							var obj = new School();
						
																	if(reader["ID"] != DBNull.Value)
									{
										obj.ID = (System.Int64)reader["ID"];
									}
									else
									{
										obj.ID = null;
									}
																		obj.Name = reader["Name"] as System.String;
																		obj.State = reader["State"] as System.String;
																		obj.City = reader["City"] as System.String;
																		obj.IsEnabled = (System.Boolean)reader["IsEnabled"];
																		obj.IsReadOnly = (System.Boolean)reader["IsReadOnly"];
																		obj.Description = reader["Description"] as System.String;
																		obj.CreatedDate = (System.DateTimeOffset)reader["CreatedDate"];
																		obj.ModifiedDate = (System.DateTimeOffset)reader["ModifiedDate"];
																		obj.DomainValidator = reader["DomainValidator"] as System.String;						return obj;
						}}

					}
		 
		namespace UHub.CoreLib.Entities.Users 
			{						
						///<summary>
						///AutoGenerated DataConverters for User
						///</summary>
						partial class User
						{

						/// <summary>
						/// Convert DataRow to User
						/// </summary>
						/// <param name="row">The DataRow that will be converted into an object</param>
						public static explicit operator UHub.CoreLib.Entities.Users.User(DataRow row)
						{
							var columnSet = row.Table.Columns;
							var obj = new User();
														obj.ID = row.Field<System.Int64?>("ID");
																obj.IsEnabled = row.Field<System.Boolean>("IsEnabled");
																obj.IsReadOnly = row.Field<System.Boolean>("IsReadOnly");
																obj.RefUID = row.Field<System.String>("RefUID");
																obj.IsConfirmed = row.Field<System.Boolean>("IsConfirmed");
																obj.IsApproved = row.Field<System.Boolean>("IsApproved");
																obj.Version = row.Field<System.String>("Version");
																obj.IsAdmin = row.Field<System.Boolean>("IsAdmin");
																obj.Email = row.Field<System.String>("Email");
																obj.Username = row.Field<System.String>("Username");
																obj.Name = row.Field<System.String>("Name");
																obj.PhoneNumber = row.Field<System.String>("PhoneNumber");
																obj.Major = row.Field<System.String>("Major");
																obj.Year = row.Field<System.String>("Year");
																obj.GradDate = row.Field<System.String>("GradDate");
																obj.Company = row.Field<System.String>("Company");
																obj.JobTitle = row.Field<System.String>("JobTitle");
																obj.IsFinished = row.Field<System.Boolean>("IsFinished");
																obj.SchoolID = row.Field<System.Int64?>("SchoolID");
																obj.CreatedBy = row.Field<System.Int64>("CreatedBy");
														return obj;
						}
						
						///<summary>
						///Convert SqlDataReader to User
						///</summary>
						/// <param name="reader">The SqlDataReader that is streaming the dataset to be converted.  With convert the row at the current iterator position</param>
						public static explicit operator UHub.CoreLib.Entities.Users.User(SqlDataReader reader)
						{
							var columnSet = reader.GetSchemaTable().Columns;
							var obj = new User();
						
																	if(reader["ID"] != DBNull.Value)
									{
										obj.ID = (System.Int64)reader["ID"];
									}
									else
									{
										obj.ID = null;
									}
																		obj.IsEnabled = (System.Boolean)reader["IsEnabled"];
																		obj.IsReadOnly = (System.Boolean)reader["IsReadOnly"];
																		obj.RefUID = reader["RefUID"] as System.String;
																		obj.IsConfirmed = (System.Boolean)reader["IsConfirmed"];
																		obj.IsApproved = (System.Boolean)reader["IsApproved"];
																		obj.Version = reader["Version"] as System.String;
																		obj.IsAdmin = (System.Boolean)reader["IsAdmin"];
																		obj.Email = reader["Email"] as System.String;
																		obj.Username = reader["Username"] as System.String;
																		obj.Name = reader["Name"] as System.String;
																		obj.PhoneNumber = reader["PhoneNumber"] as System.String;
																		obj.Major = reader["Major"] as System.String;
																		obj.Year = reader["Year"] as System.String;
																		obj.GradDate = reader["GradDate"] as System.String;
																		obj.Company = reader["Company"] as System.String;
																		obj.JobTitle = reader["JobTitle"] as System.String;
																		obj.IsFinished = (System.Boolean)reader["IsFinished"];
																	if(reader["SchoolID"] != DBNull.Value)
									{
										obj.SchoolID = (System.Int64)reader["SchoolID"];
									}
									else
									{
										obj.SchoolID = null;
									}
																		obj.CreatedBy = (System.Int64)reader["CreatedBy"];						return obj;
						}}

											
						///<summary>
						///AutoGenerated DataConverters for UserRecoveryContext
						///</summary>
						partial class UserRecoveryContext
						{

						/// <summary>
						/// Convert DataRow to UserRecoveryContext
						/// </summary>
						/// <param name="row">The DataRow that will be converted into an object</param>
						public static explicit operator UHub.CoreLib.Entities.Users.UserRecoveryContext(DataRow row)
						{
							var columnSet = row.Table.Columns;
							var obj = new UserRecoveryContext();
														obj.UserID = row.Field<System.Int64>("UserID");
																obj.UserUID = row.Field<System.Guid>("UserUID");
																obj.RecoveryID = row.Field<System.String>("RecoveryID");
																obj.RecoveryKey = row.Field<System.String>("RecoveryKey");
																obj.EffFromDate = row.Field<System.DateTimeOffset>("EffFromDate");
																obj.EffToDate = row.Field<System.DateTimeOffset>("EffToDate");
																obj.IsEnabled = row.Field<System.Boolean>("IsEnabled");
																obj.AttemptCount = row.Field<System.Byte>("AttemptCount");
																obj.IsOptional = row.Field<System.Boolean>("IsOptional");
														return obj;
						}
						
						///<summary>
						///Convert SqlDataReader to UserRecoveryContext
						///</summary>
						/// <param name="reader">The SqlDataReader that is streaming the dataset to be converted.  With convert the row at the current iterator position</param>
						public static explicit operator UHub.CoreLib.Entities.Users.UserRecoveryContext(SqlDataReader reader)
						{
							var columnSet = reader.GetSchemaTable().Columns;
							var obj = new UserRecoveryContext();
						
																		obj.UserID = (System.Int64)reader["UserID"];
																		obj.UserUID = (System.Guid)reader["UserUID"];
																		obj.RecoveryID = reader["RecoveryID"] as System.String;
																		obj.RecoveryKey = reader["RecoveryKey"] as System.String;
																		obj.EffFromDate = (System.DateTimeOffset)reader["EffFromDate"];
																		obj.EffToDate = (System.DateTimeOffset)reader["EffToDate"];
																		obj.IsEnabled = (System.Boolean)reader["IsEnabled"];
																		obj.AttemptCount = (System.Byte)reader["AttemptCount"];
																		obj.IsOptional = (System.Boolean)reader["IsOptional"];						return obj;
						}}

					}
		 
		namespace UHub.CoreLib.Security.Authentication 
			{						
						///<summary>
						///AutoGenerated DataConverters for TokenValidator
						///</summary>
						partial class TokenValidator
						{

						/// <summary>
						/// Convert DataRow to TokenValidator
						/// </summary>
						/// <param name="row">The DataRow that will be converted into an object</param>
						public static explicit operator UHub.CoreLib.Security.Authentication.TokenValidator(DataRow row)
						{
							var columnSet = row.Table.Columns;
							var obj = new TokenValidator();
														obj.IssueDate = row.Field<System.DateTimeOffset>("IssueDate");
																obj.MaxExpirationDate = row.Field<System.DateTimeOffset>("MaxExpirationDate");
																obj.TokenID = row.Field<System.String>("TokenID");
																obj.IsPersistent = row.Field<System.Boolean>("IsPersistent");
																obj.TokenHash = row.Field<System.String>("TokenHash");
																obj.RequestID = row.Field<System.String>("RequestID");
																obj.SessionID = row.Field<System.String>("SessionID");
																obj.IsValid = row.Field<System.Boolean>("IsValid");
														return obj;
						}
						
						///<summary>
						///Convert SqlDataReader to TokenValidator
						///</summary>
						/// <param name="reader">The SqlDataReader that is streaming the dataset to be converted.  With convert the row at the current iterator position</param>
						public static explicit operator UHub.CoreLib.Security.Authentication.TokenValidator(SqlDataReader reader)
						{
							var columnSet = reader.GetSchemaTable().Columns;
							var obj = new TokenValidator();
						
																		obj.IssueDate = (System.DateTimeOffset)reader["IssueDate"];
																		obj.MaxExpirationDate = (System.DateTimeOffset)reader["MaxExpirationDate"];
																		obj.TokenID = reader["TokenID"] as System.String;
																		obj.IsPersistent = (System.Boolean)reader["IsPersistent"];
																		obj.TokenHash = reader["TokenHash"] as System.String;
																		obj.RequestID = reader["RequestID"] as System.String;
																		obj.SessionID = reader["SessionID"] as System.String;
																		obj.IsValid = (System.Boolean)reader["IsValid"];						return obj;
						}}

											
						///<summary>
						///AutoGenerated DataConverters for UserAuthInfo
						///</summary>
						partial class UserAuthInfo
						{

						/// <summary>
						/// Convert DataRow to UserAuthInfo
						/// </summary>
						/// <param name="row">The DataRow that will be converted into an object</param>
						public static explicit operator UHub.CoreLib.Security.Authentication.UserAuthInfo(DataRow row)
						{
							var columnSet = row.Table.Columns;
							var obj = new UserAuthInfo();
														obj.UserID = row.Field<System.Int64>("UserID");
																obj.PswdHash = row.Field<System.String>("PswdHash");
																obj.Salt = row.Field<System.String>("Salt");
																obj.PswdCreatedDate = row.Field<System.DateTimeOffset>("PswdCreatedDate");
																obj.PswdModifiedDate = row.Field<System.DateTimeOffset>("PswdModifiedDate");
																obj.LastLockoutDate = row.Field<System.DateTimeOffset?>("LastLockoutDate");
																obj.StartFailedPswdWindow = row.Field<System.DateTimeOffset?>("StartFailedPswdWindow");
																obj.FailedPswdAttemptCount = row.Field<System.Byte>("FailedPswdAttemptCount");
														return obj;
						}
						
						///<summary>
						///Convert SqlDataReader to UserAuthInfo
						///</summary>
						/// <param name="reader">The SqlDataReader that is streaming the dataset to be converted.  With convert the row at the current iterator position</param>
						public static explicit operator UHub.CoreLib.Security.Authentication.UserAuthInfo(SqlDataReader reader)
						{
							var columnSet = reader.GetSchemaTable().Columns;
							var obj = new UserAuthInfo();
						
																		obj.UserID = (System.Int64)reader["UserID"];
																		obj.PswdHash = reader["PswdHash"] as System.String;
																		obj.Salt = reader["Salt"] as System.String;
																		obj.PswdCreatedDate = (System.DateTimeOffset)reader["PswdCreatedDate"];
																		obj.PswdModifiedDate = (System.DateTimeOffset)reader["PswdModifiedDate"];
																	if(reader["LastLockoutDate"] != DBNull.Value)
									{
										obj.LastLockoutDate = (System.DateTimeOffset)reader["LastLockoutDate"];
									}
									else
									{
										obj.LastLockoutDate = null;
									}
																	if(reader["StartFailedPswdWindow"] != DBNull.Value)
									{
										obj.StartFailedPswdWindow = (System.DateTimeOffset)reader["StartFailedPswdWindow"];
									}
									else
									{
										obj.StartFailedPswdWindow = null;
									}
																		obj.FailedPswdAttemptCount = (System.Byte)reader["FailedPswdAttemptCount"];						return obj;
						}}

					}
		 
		