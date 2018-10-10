﻿CREATE TABLE [dbo].[EntityTypes] (
    [ID]          SMALLINT         IDENTITY (1, 1) NOT NULL,
    [UID]         UNIQUEIDENTIFIER CONSTRAINT [DF_EntityTypes_UID] DEFAULT (newsequentialid()) NOT NULL,
    [Name]        NVARCHAR (100)   NOT NULL,
    [Description] NVARCHAR (250)   NULL,
    CONSTRAINT [PK_EntityTypes] PRIMARY KEY CLUSTERED ([ID] ASC)
);

