USE [UHUB_DB]
GO
SET IDENTITY_INSERT [dbo].[DataTypes] ON 

INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (8, N'bigint')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (7, N'bit')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (3, N'datetimeoffset(7)')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (10, N'nvarchar(10)')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (5, N'nvarchar(100)')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (1, N'nvarchar(200)')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (9, N'nvarchar(250)')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (2, N'nvarchar(50)')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (6, N'nvarchar(500)')
INSERT [dbo].[DataTypes] ([ID], [Name]) VALUES (4, N'nvarchar(MAX)')
SET IDENTITY_INSERT [dbo].[DataTypes] OFF
