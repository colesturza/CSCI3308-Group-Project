USE [UHUB_DB]
GO
INSERT [dbo].[SchemaVersioning] ([Name], [Version]) VALUES (N'UserSchemaVersion', CAST(0.10000 AS Decimal(5, 5)))
INSERT [dbo].[SchemaVersioning] ([Name], [Version]) VALUES (N'EntitySchemaVersion', CAST(0.10000 AS Decimal(5, 5)))
INSERT [dbo].[SchemaVersioning] ([Name], [Version]) VALUES (N'InterfaceSchemaVersion', CAST(0.10000 AS Decimal(5, 5)))
INSERT [dbo].[SchemaVersioning] ([Name], [Version]) VALUES (N'AuthSchemaVersion', CAST(0.10000 AS Decimal(5, 5)))
