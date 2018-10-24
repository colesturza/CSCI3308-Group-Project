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



namespace UHub.CoreLib.Entities.Comments.DTOs 
			{						
						///<summary>
						///AutoGenerated DataConverters for Comment_C_PublicDTO
						///</summary>
						partial class Comment_C_PublicDTO
						{

						/// <summary>
						/// Convert Comment to Comment_C_PublicDTO
						/// </summary>
						/// <param name="arg1">The Comment that will be converted into an Comment_C_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Comments.DTOs.Comment_C_PublicDTO(UHub.CoreLib.Entities.Comments.Comment dtoObj)
						{
                            UHub.CoreLib.Entities.Comments.DTOs.Comment_C_PublicDTO nativeObj = new UHub.CoreLib.Entities.Comments.DTOs.Comment_C_PublicDTO();
							
						nativeObj.Content = dtoObj.Content;
nativeObj.ParentID = dtoObj.ParentID;
						return nativeObj;
						}


                        /// <summary>
						/// Convert Comment_C_PublicDTO to Comment
						/// </summary>
						/// <param name="arg1">The Comment_C_PublicDTO that will be converted into an UHub.CoreLib.Entities.Comments.Comment</param>
						public static explicit operator UHub.CoreLib.Entities.Comments.Comment(UHub.CoreLib.Entities.Comments.DTOs.Comment_C_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Comments.Comment dtoObj = new UHub.CoreLib.Entities.Comments.Comment();
							
						dtoObj.Content = nativeObj.Content;
dtoObj.ParentID = nativeObj.ParentID;
						return dtoObj;
						}}
                        

											
						///<summary>
						///AutoGenerated DataConverters for Comment_R_PublicDTO
						///</summary>
						partial class Comment_R_PublicDTO
						{

						/// <summary>
						/// Convert Comment to Comment_R_PublicDTO
						/// </summary>
						/// <param name="arg1">The Comment that will be converted into an Comment_R_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Comments.DTOs.Comment_R_PublicDTO(UHub.CoreLib.Entities.Comments.Comment dtoObj)
						{
                            UHub.CoreLib.Entities.Comments.DTOs.Comment_R_PublicDTO nativeObj = new UHub.CoreLib.Entities.Comments.DTOs.Comment_R_PublicDTO();
							
						nativeObj.ID = dtoObj.ID;
nativeObj.IsEnabled = dtoObj.IsEnabled;
nativeObj.IsReadOnly = dtoObj.IsReadOnly;
nativeObj.Content = dtoObj.Content;
nativeObj.IsModified = dtoObj.IsModified;
nativeObj.ViewCount = dtoObj.ViewCount;
nativeObj.ParentID = dtoObj.ParentID;
nativeObj.CreatedBy = dtoObj.CreatedBy;
nativeObj.CreatedDate = dtoObj.CreatedDate;
nativeObj.ModifiedBy = dtoObj.ModifiedBy;
nativeObj.ModifiedDate = dtoObj.ModifiedDate;
						return nativeObj;
						}


                        /// <summary>
						/// Convert Comment_R_PublicDTO to Comment
						/// </summary>
						/// <param name="arg1">The Comment_R_PublicDTO that will be converted into an UHub.CoreLib.Entities.Comments.Comment</param>
						public static explicit operator UHub.CoreLib.Entities.Comments.Comment(UHub.CoreLib.Entities.Comments.DTOs.Comment_R_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Comments.Comment dtoObj = new UHub.CoreLib.Entities.Comments.Comment();
							
						dtoObj.ID = nativeObj.ID;
dtoObj.IsEnabled = nativeObj.IsEnabled;
dtoObj.IsReadOnly = nativeObj.IsReadOnly;
dtoObj.Content = nativeObj.Content;
dtoObj.IsModified = nativeObj.IsModified;
dtoObj.ViewCount = nativeObj.ViewCount;
dtoObj.ParentID = nativeObj.ParentID;
dtoObj.CreatedBy = nativeObj.CreatedBy;
dtoObj.CreatedDate = nativeObj.CreatedDate;
dtoObj.ModifiedBy = nativeObj.ModifiedBy;
dtoObj.ModifiedDate = nativeObj.ModifiedDate;
						return dtoObj;
						}}
                        

					}
		 
		namespace UHub.CoreLib.Entities.Posts.DTOs 
			{						
						///<summary>
						///AutoGenerated DataConverters for Post_C_PublicDTO
						///</summary>
						partial class Post_C_PublicDTO
						{

						/// <summary>
						/// Convert Post to Post_C_PublicDTO
						/// </summary>
						/// <param name="arg1">The Post that will be converted into an Post_C_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Posts.DTOs.Post_C_PublicDTO(UHub.CoreLib.Entities.Posts.Post dtoObj)
						{
                            UHub.CoreLib.Entities.Posts.DTOs.Post_C_PublicDTO nativeObj = new UHub.CoreLib.Entities.Posts.DTOs.Post_C_PublicDTO();
							
						nativeObj.Name = dtoObj.Name;
nativeObj.Content = dtoObj.Content;
nativeObj.CanComment = dtoObj.CanComment;
nativeObj.IsPublic = dtoObj.IsPublic;
nativeObj.ParentID = dtoObj.ParentID;
						return nativeObj;
						}


                        /// <summary>
						/// Convert Post_C_PublicDTO to Post
						/// </summary>
						/// <param name="arg1">The Post_C_PublicDTO that will be converted into an UHub.CoreLib.Entities.Posts.Post</param>
						public static explicit operator UHub.CoreLib.Entities.Posts.Post(UHub.CoreLib.Entities.Posts.DTOs.Post_C_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Posts.Post dtoObj = new UHub.CoreLib.Entities.Posts.Post();
							
						dtoObj.Name = nativeObj.Name;
dtoObj.Content = nativeObj.Content;
dtoObj.CanComment = nativeObj.CanComment;
dtoObj.IsPublic = nativeObj.IsPublic;
dtoObj.ParentID = nativeObj.ParentID;
						return dtoObj;
						}}
                        

											
						///<summary>
						///AutoGenerated DataConverters for Post_R_PublicDTO
						///</summary>
						partial class Post_R_PublicDTO
						{

						/// <summary>
						/// Convert Post to Post_R_PublicDTO
						/// </summary>
						/// <param name="arg1">The Post that will be converted into an Post_R_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Posts.DTOs.Post_R_PublicDTO(UHub.CoreLib.Entities.Posts.Post dtoObj)
						{
                            UHub.CoreLib.Entities.Posts.DTOs.Post_R_PublicDTO nativeObj = new UHub.CoreLib.Entities.Posts.DTOs.Post_R_PublicDTO();
							
						nativeObj.ID = dtoObj.ID;
nativeObj.IsReadOnly = dtoObj.IsReadOnly;
nativeObj.Name = dtoObj.Name;
nativeObj.Content = dtoObj.Content;
nativeObj.IsModified = dtoObj.IsModified;
nativeObj.ViewCount = dtoObj.ViewCount;
nativeObj.IsLocked = dtoObj.IsLocked;
nativeObj.CanComment = dtoObj.CanComment;
nativeObj.IsPublic = dtoObj.IsPublic;
nativeObj.ParentID = dtoObj.ParentID;
nativeObj.CreatedBy = dtoObj.CreatedBy;
nativeObj.CreatedDate = dtoObj.CreatedDate;
nativeObj.ModifiedBy = dtoObj.ModifiedBy;
nativeObj.ModifiedDate = dtoObj.ModifiedDate;
						return nativeObj;
						}


                        /// <summary>
						/// Convert Post_R_PublicDTO to Post
						/// </summary>
						/// <param name="arg1">The Post_R_PublicDTO that will be converted into an UHub.CoreLib.Entities.Posts.Post</param>
						public static explicit operator UHub.CoreLib.Entities.Posts.Post(UHub.CoreLib.Entities.Posts.DTOs.Post_R_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Posts.Post dtoObj = new UHub.CoreLib.Entities.Posts.Post();
							
						dtoObj.ID = nativeObj.ID;
dtoObj.IsReadOnly = nativeObj.IsReadOnly;
dtoObj.Name = nativeObj.Name;
dtoObj.Content = nativeObj.Content;
dtoObj.IsModified = nativeObj.IsModified;
dtoObj.ViewCount = nativeObj.ViewCount;
dtoObj.IsLocked = nativeObj.IsLocked;
dtoObj.CanComment = nativeObj.CanComment;
dtoObj.IsPublic = nativeObj.IsPublic;
dtoObj.ParentID = nativeObj.ParentID;
dtoObj.CreatedBy = nativeObj.CreatedBy;
dtoObj.CreatedDate = nativeObj.CreatedDate;
dtoObj.ModifiedBy = nativeObj.ModifiedBy;
dtoObj.ModifiedDate = nativeObj.ModifiedDate;
						return dtoObj;
						}}
                        

					}
		 
		namespace UHub.CoreLib.Entities.ClubModerators.DTOs 
			{						
						///<summary>
						///AutoGenerated DataConverters for ClubModerator_R_PublicDTO
						///</summary>
						partial class ClubModerator_R_PublicDTO
						{

						/// <summary>
						/// Convert ClubModerator to ClubModerator_R_PublicDTO
						/// </summary>
						/// <param name="arg1">The ClubModerator that will be converted into an ClubModerator_R_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.ClubModerators.DTOs.ClubModerator_R_PublicDTO(UHub.CoreLib.Entities.ClubModerators.ClubModerator dtoObj)
						{
                            UHub.CoreLib.Entities.ClubModerators.DTOs.ClubModerator_R_PublicDTO nativeObj = new UHub.CoreLib.Entities.ClubModerators.DTOs.ClubModerator_R_PublicDTO();
							
						nativeObj.UserID = dtoObj.UserID;
nativeObj.IsOwner = dtoObj.IsOwner;
nativeObj.CreatedDate = dtoObj.CreatedDate;
						return nativeObj;
						}


                        /// <summary>
						/// Convert ClubModerator_R_PublicDTO to ClubModerator
						/// </summary>
						/// <param name="arg1">The ClubModerator_R_PublicDTO that will be converted into an UHub.CoreLib.Entities.ClubModerators.ClubModerator</param>
						public static explicit operator UHub.CoreLib.Entities.ClubModerators.ClubModerator(UHub.CoreLib.Entities.ClubModerators.DTOs.ClubModerator_R_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.ClubModerators.ClubModerator dtoObj = new UHub.CoreLib.Entities.ClubModerators.ClubModerator();
							
						dtoObj.UserID = nativeObj.UserID;
dtoObj.IsOwner = nativeObj.IsOwner;
dtoObj.CreatedDate = nativeObj.CreatedDate;
						return dtoObj;
						}}
                        

					}
		 
		namespace UHub.CoreLib.Entities.SchoolClubs.DTOs 
			{						
						///<summary>
						///AutoGenerated DataConverters for SchoolClub_C_PublicDTO
						///</summary>
						partial class SchoolClub_C_PublicDTO
						{

						/// <summary>
						/// Convert SchoolClub to SchoolClub_C_PublicDTO
						/// </summary>
						/// <param name="arg1">The SchoolClub that will be converted into an SchoolClub_C_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.SchoolClubs.DTOs.SchoolClub_C_PublicDTO(UHub.CoreLib.Entities.SchoolClubs.SchoolClub dtoObj)
						{
                            UHub.CoreLib.Entities.SchoolClubs.DTOs.SchoolClub_C_PublicDTO nativeObj = new UHub.CoreLib.Entities.SchoolClubs.DTOs.SchoolClub_C_PublicDTO();
							
						nativeObj.Name = dtoObj.Name;
nativeObj.Description = dtoObj.Description;
						return nativeObj;
						}


                        /// <summary>
						/// Convert SchoolClub_C_PublicDTO to SchoolClub
						/// </summary>
						/// <param name="arg1">The SchoolClub_C_PublicDTO that will be converted into an UHub.CoreLib.Entities.SchoolClubs.SchoolClub</param>
						public static explicit operator UHub.CoreLib.Entities.SchoolClubs.SchoolClub(UHub.CoreLib.Entities.SchoolClubs.DTOs.SchoolClub_C_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.SchoolClubs.SchoolClub dtoObj = new UHub.CoreLib.Entities.SchoolClubs.SchoolClub();
							
						dtoObj.Name = nativeObj.Name;
dtoObj.Description = nativeObj.Description;
						return dtoObj;
						}}
                        

											
						///<summary>
						///AutoGenerated DataConverters for SchoolClub_R_PublicDTO
						///</summary>
						partial class SchoolClub_R_PublicDTO
						{

						/// <summary>
						/// Convert SchoolClub to SchoolClub_R_PublicDTO
						/// </summary>
						/// <param name="arg1">The SchoolClub that will be converted into an SchoolClub_R_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.SchoolClubs.DTOs.SchoolClub_R_PublicDTO(UHub.CoreLib.Entities.SchoolClubs.SchoolClub dtoObj)
						{
                            UHub.CoreLib.Entities.SchoolClubs.DTOs.SchoolClub_R_PublicDTO nativeObj = new UHub.CoreLib.Entities.SchoolClubs.DTOs.SchoolClub_R_PublicDTO();
							
						nativeObj.ID = dtoObj.ID;
nativeObj.IsEnabled = dtoObj.IsEnabled;
nativeObj.IsReadOnly = dtoObj.IsReadOnly;
nativeObj.Name = dtoObj.Name;
nativeObj.Description = dtoObj.Description;
nativeObj.CreatedDate = dtoObj.CreatedDate;
nativeObj.ModifiedDate = dtoObj.ModifiedDate;
						return nativeObj;
						}


                        /// <summary>
						/// Convert SchoolClub_R_PublicDTO to SchoolClub
						/// </summary>
						/// <param name="arg1">The SchoolClub_R_PublicDTO that will be converted into an UHub.CoreLib.Entities.SchoolClubs.SchoolClub</param>
						public static explicit operator UHub.CoreLib.Entities.SchoolClubs.SchoolClub(UHub.CoreLib.Entities.SchoolClubs.DTOs.SchoolClub_R_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.SchoolClubs.SchoolClub dtoObj = new UHub.CoreLib.Entities.SchoolClubs.SchoolClub();
							
						dtoObj.ID = nativeObj.ID;
dtoObj.IsEnabled = nativeObj.IsEnabled;
dtoObj.IsReadOnly = nativeObj.IsReadOnly;
dtoObj.Name = nativeObj.Name;
dtoObj.Description = nativeObj.Description;
dtoObj.CreatedDate = nativeObj.CreatedDate;
dtoObj.ModifiedDate = nativeObj.ModifiedDate;
						return dtoObj;
						}}
                        

					}
		 
		namespace UHub.CoreLib.Entities.SchoolMajors.DTOs 
			{						
						///<summary>
						///AutoGenerated DataConverters for SchoolMajor_R_PublicDTO
						///</summary>
						partial class SchoolMajor_R_PublicDTO
						{

						/// <summary>
						/// Convert SchoolMajor to SchoolMajor_R_PublicDTO
						/// </summary>
						/// <param name="arg1">The SchoolMajor that will be converted into an SchoolMajor_R_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.SchoolMajors.DTOs.SchoolMajor_R_PublicDTO(UHub.CoreLib.Entities.SchoolMajors.SchoolMajor dtoObj)
						{
                            UHub.CoreLib.Entities.SchoolMajors.DTOs.SchoolMajor_R_PublicDTO nativeObj = new UHub.CoreLib.Entities.SchoolMajors.DTOs.SchoolMajor_R_PublicDTO();
							
						nativeObj.Name = dtoObj.Name;
nativeObj.IsEnabled = dtoObj.IsEnabled;
nativeObj.Description = dtoObj.Description;
nativeObj.CreatedDate = dtoObj.CreatedDate;
nativeObj.ModifiedDate = dtoObj.ModifiedDate;
						return nativeObj;
						}


                        /// <summary>
						/// Convert SchoolMajor_R_PublicDTO to SchoolMajor
						/// </summary>
						/// <param name="arg1">The SchoolMajor_R_PublicDTO that will be converted into an UHub.CoreLib.Entities.SchoolMajors.SchoolMajor</param>
						public static explicit operator UHub.CoreLib.Entities.SchoolMajors.SchoolMajor(UHub.CoreLib.Entities.SchoolMajors.DTOs.SchoolMajor_R_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.SchoolMajors.SchoolMajor dtoObj = new UHub.CoreLib.Entities.SchoolMajors.SchoolMajor();
							
						dtoObj.Name = nativeObj.Name;
dtoObj.IsEnabled = nativeObj.IsEnabled;
dtoObj.Description = nativeObj.Description;
dtoObj.CreatedDate = nativeObj.CreatedDate;
dtoObj.ModifiedDate = nativeObj.ModifiedDate;
						return dtoObj;
						}}
                        

					}
		 
		namespace UHub.CoreLib.Entities.Schools.DTOs 
			{						
						///<summary>
						///AutoGenerated DataConverters for School_R_PublicDTO
						///</summary>
						partial class School_R_PublicDTO
						{

						/// <summary>
						/// Convert School to School_R_PublicDTO
						/// </summary>
						/// <param name="arg1">The School that will be converted into an School_R_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Schools.DTOs.School_R_PublicDTO(UHub.CoreLib.Entities.Schools.School dtoObj)
						{
                            UHub.CoreLib.Entities.Schools.DTOs.School_R_PublicDTO nativeObj = new UHub.CoreLib.Entities.Schools.DTOs.School_R_PublicDTO();
							
						nativeObj.ID = dtoObj.ID;
nativeObj.Name = dtoObj.Name;
nativeObj.State = dtoObj.State;
nativeObj.City = dtoObj.City;
nativeObj.IsEnabled = dtoObj.IsEnabled;
nativeObj.IsReadOnly = dtoObj.IsReadOnly;
nativeObj.Description = dtoObj.Description;
nativeObj.CreatedDate = dtoObj.CreatedDate;
nativeObj.ModifiedDate = dtoObj.ModifiedDate;
						return nativeObj;
						}


                        /// <summary>
						/// Convert School_R_PublicDTO to School
						/// </summary>
						/// <param name="arg1">The School_R_PublicDTO that will be converted into an UHub.CoreLib.Entities.Schools.School</param>
						public static explicit operator UHub.CoreLib.Entities.Schools.School(UHub.CoreLib.Entities.Schools.DTOs.School_R_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Schools.School dtoObj = new UHub.CoreLib.Entities.Schools.School();
							
						dtoObj.ID = nativeObj.ID;
dtoObj.Name = nativeObj.Name;
dtoObj.State = nativeObj.State;
dtoObj.City = nativeObj.City;
dtoObj.IsEnabled = nativeObj.IsEnabled;
dtoObj.IsReadOnly = nativeObj.IsReadOnly;
dtoObj.Description = nativeObj.Description;
dtoObj.CreatedDate = nativeObj.CreatedDate;
dtoObj.ModifiedDate = nativeObj.ModifiedDate;
						return dtoObj;
						}}
                        

					}
		 
		namespace UHub.CoreLib.Entities.Users.DTOs 
			{						
						///<summary>
						///AutoGenerated DataConverters for User_R_PrivateDTO
						///</summary>
						partial class User_R_PrivateDTO
						{

						/// <summary>
						/// Convert User to User_R_PrivateDTO
						/// </summary>
						/// <param name="arg1">The User that will be converted into an User_R_PrivateDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Users.DTOs.User_R_PrivateDTO(UHub.CoreLib.Entities.Users.User dtoObj)
						{
                            UHub.CoreLib.Entities.Users.DTOs.User_R_PrivateDTO nativeObj = new UHub.CoreLib.Entities.Users.DTOs.User_R_PrivateDTO();
							
						nativeObj.Email = dtoObj.Email;
nativeObj.Name = dtoObj.Name;
nativeObj.PhoneNumber = dtoObj.PhoneNumber;
nativeObj.SchoolID = dtoObj.SchoolID;
nativeObj.Username = dtoObj.Username;
nativeObj.Major = dtoObj.Major;
nativeObj.Year = dtoObj.Year;
nativeObj.GradDate = dtoObj.GradDate;
nativeObj.Company = dtoObj.Company;
nativeObj.JobTitle = dtoObj.JobTitle;
						return nativeObj;
						}


                        /// <summary>
						/// Convert User_R_PrivateDTO to User
						/// </summary>
						/// <param name="arg1">The User_R_PrivateDTO that will be converted into an UHub.CoreLib.Entities.Users.User</param>
						public static explicit operator UHub.CoreLib.Entities.Users.User(UHub.CoreLib.Entities.Users.DTOs.User_R_PrivateDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Users.User dtoObj = new UHub.CoreLib.Entities.Users.User();
							
						dtoObj.Email = nativeObj.Email;
dtoObj.Name = nativeObj.Name;
dtoObj.PhoneNumber = nativeObj.PhoneNumber;
dtoObj.SchoolID = nativeObj.SchoolID;
dtoObj.Username = nativeObj.Username;
dtoObj.Major = nativeObj.Major;
dtoObj.Year = nativeObj.Year;
dtoObj.GradDate = nativeObj.GradDate;
dtoObj.Company = nativeObj.Company;
dtoObj.JobTitle = nativeObj.JobTitle;
						return dtoObj;
						}}
                        

											
						///<summary>
						///AutoGenerated DataConverters for User_R_PublicDTO
						///</summary>
						partial class User_R_PublicDTO
						{

						/// <summary>
						/// Convert User to User_R_PublicDTO
						/// </summary>
						/// <param name="arg1">The User that will be converted into an User_R_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Users.DTOs.User_R_PublicDTO(UHub.CoreLib.Entities.Users.User dtoObj)
						{
                            UHub.CoreLib.Entities.Users.DTOs.User_R_PublicDTO nativeObj = new UHub.CoreLib.Entities.Users.DTOs.User_R_PublicDTO();
							
						nativeObj.Username = dtoObj.Username;
nativeObj.Major = dtoObj.Major;
nativeObj.Year = dtoObj.Year;
nativeObj.GradDate = dtoObj.GradDate;
nativeObj.Company = dtoObj.Company;
nativeObj.JobTitle = dtoObj.JobTitle;
						return nativeObj;
						}


                        /// <summary>
						/// Convert User_R_PublicDTO to User
						/// </summary>
						/// <param name="arg1">The User_R_PublicDTO that will be converted into an UHub.CoreLib.Entities.Users.User</param>
						public static explicit operator UHub.CoreLib.Entities.Users.User(UHub.CoreLib.Entities.Users.DTOs.User_R_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Users.User dtoObj = new UHub.CoreLib.Entities.Users.User();
							
						dtoObj.Username = nativeObj.Username;
dtoObj.Major = nativeObj.Major;
dtoObj.Year = nativeObj.Year;
dtoObj.GradDate = nativeObj.GradDate;
dtoObj.Company = nativeObj.Company;
dtoObj.JobTitle = nativeObj.JobTitle;
						return dtoObj;
						}}
                        

											
						///<summary>
						///AutoGenerated DataConverters for User_U_PrivateDTO
						///</summary>
						partial class User_U_PrivateDTO
						{

						/// <summary>
						/// Convert User to User_U_PrivateDTO
						/// </summary>
						/// <param name="arg1">The User that will be converted into an User_U_PrivateDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Users.DTOs.User_U_PrivateDTO(UHub.CoreLib.Entities.Users.User dtoObj)
						{
                            UHub.CoreLib.Entities.Users.DTOs.User_U_PrivateDTO nativeObj = new UHub.CoreLib.Entities.Users.DTOs.User_U_PrivateDTO();
							
						nativeObj.ID = dtoObj.ID;
nativeObj.Name = dtoObj.Name;
nativeObj.PhoneNumber = dtoObj.PhoneNumber;
nativeObj.Major = dtoObj.Major;
nativeObj.Year = dtoObj.Year;
nativeObj.GradDate = dtoObj.GradDate;
nativeObj.Company = dtoObj.Company;
nativeObj.JobTitle = dtoObj.JobTitle;
						return nativeObj;
						}


                        /// <summary>
						/// Convert User_U_PrivateDTO to User
						/// </summary>
						/// <param name="arg1">The User_U_PrivateDTO that will be converted into an UHub.CoreLib.Entities.Users.User</param>
						public static explicit operator UHub.CoreLib.Entities.Users.User(UHub.CoreLib.Entities.Users.DTOs.User_U_PrivateDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Users.User dtoObj = new UHub.CoreLib.Entities.Users.User();
							
						dtoObj.ID = nativeObj.ID;
dtoObj.Name = nativeObj.Name;
dtoObj.PhoneNumber = nativeObj.PhoneNumber;
dtoObj.Major = nativeObj.Major;
dtoObj.Year = nativeObj.Year;
dtoObj.GradDate = nativeObj.GradDate;
dtoObj.Company = nativeObj.Company;
dtoObj.JobTitle = nativeObj.JobTitle;
						return dtoObj;
						}}
                        

											
						///<summary>
						///AutoGenerated DataConverters for User_C_PublicDTO
						///</summary>
						partial class User_C_PublicDTO
						{

						/// <summary>
						/// Convert User to User_C_PublicDTO
						/// </summary>
						/// <param name="arg1">The User that will be converted into an User_C_PublicDTO</param>
						public static explicit operator UHub.CoreLib.Entities.Users.DTOs.User_C_PublicDTO(UHub.CoreLib.Entities.Users.User dtoObj)
						{
                            UHub.CoreLib.Entities.Users.DTOs.User_C_PublicDTO nativeObj = new UHub.CoreLib.Entities.Users.DTOs.User_C_PublicDTO();
							
						nativeObj.Email = dtoObj.Email;
nativeObj.Username = dtoObj.Username;
nativeObj.Password = dtoObj.Password;
nativeObj.Name = dtoObj.Name;
nativeObj.PhoneNumber = dtoObj.PhoneNumber;
nativeObj.Major = dtoObj.Major;
nativeObj.Year = dtoObj.Year;
nativeObj.GradDate = dtoObj.GradDate;
nativeObj.Company = dtoObj.Company;
nativeObj.JobTitle = dtoObj.JobTitle;
						return nativeObj;
						}


                        /// <summary>
						/// Convert User_C_PublicDTO to User
						/// </summary>
						/// <param name="arg1">The User_C_PublicDTO that will be converted into an UHub.CoreLib.Entities.Users.User</param>
						public static explicit operator UHub.CoreLib.Entities.Users.User(UHub.CoreLib.Entities.Users.DTOs.User_C_PublicDTO nativeObj)
						{
                            UHub.CoreLib.Entities.Users.User dtoObj = new UHub.CoreLib.Entities.Users.User();
							
						dtoObj.Email = nativeObj.Email;
dtoObj.Username = nativeObj.Username;
dtoObj.Password = nativeObj.Password;
dtoObj.Name = nativeObj.Name;
dtoObj.PhoneNumber = nativeObj.PhoneNumber;
dtoObj.Major = nativeObj.Major;
dtoObj.Year = nativeObj.Year;
dtoObj.GradDate = nativeObj.GradDate;
dtoObj.Company = nativeObj.Company;
dtoObj.JobTitle = nativeObj.JobTitle;
						return dtoObj;
						}}
                        

					}
		 
		