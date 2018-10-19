CREATE TABLE [dbo].[Entities] (
    [ID]           BIGINT             IDENTITY (1, 1) NOT NULL,
    [UID]          UNIQUEIDENTIFIER   CONSTRAINT [DF_Entities_UID] DEFAULT (newsequentialid()) NOT NULL,
    [EntTypeID]    SMALLINT           NOT NULL,
    [IsEnabled]    BIT                CONSTRAINT [DF_Entities_IsEnabled] DEFAULT ((1)) NOT NULL,
    [IsReadOnly]   BIT                CONSTRAINT [DF_Entities_IsReadOnly] DEFAULT ((0)) NOT NULL,
    [IsDeleted]    BIT                CONSTRAINT [DF_Entities_IsDeleted] DEFAULT ((0)) NOT NULL,
    [CreatedDate]  DATETIMEOFFSET (7) CONSTRAINT [DF_Entities_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]    BIGINT             NOT NULL,
    [ModifiedDate] DATETIMEOFFSET (7) NULL,
    [ModifiedBy]   BIGINT             NULL,
    [DeletedDate]  DATETIMEOFFSET (7) NULL,
    [DeletedBy]    BIGINT             NULL,
    CONSTRAINT [PK_Entities] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Entities_EntityTypes] FOREIGN KEY ([EntTypeID]) REFERENCES [dbo].[EntityTypes] ([ID]),
    CONSTRAINT [FK_Entities_Users] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users] ([EntID]),
    CONSTRAINT [FK_Entities_Users1] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users] ([EntID]),
    CONSTRAINT [FK_Entities_Users2] FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users] ([EntID])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Entities]
    ON [dbo].[Entities]([ID] ASC, [EntTypeID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Entities_EntType]
    ON [dbo].[Entities]([EntTypeID] ASC);

