CREATE TABLE [dbo].[EntProperties] (
    [ID]               INT            IDENTITY (1, 1) NOT NULL,
    [PropName]         VARCHAR (100)  NOT NULL,
    [PropFriendlyName] NVARCHAR (100) NOT NULL,
    [Description]      NVARCHAR (250) NULL,
    [DataType]         VARCHAR (100)  NOT NULL,
    CONSTRAINT [PK_EntProperties] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_EntProperties_DataTypes] FOREIGN KEY ([DataType]) REFERENCES [dbo].[DataTypes] ([Name])
);

