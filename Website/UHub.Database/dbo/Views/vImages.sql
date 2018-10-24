





CREATE view [dbo].[vImages]
as

	select
		ent.ID,
		ent.EntTypeID,
		ent.IsEnabled,
		ent.IsReadonly,
		xref_Name.PropValue							as [Name],
		cast(xref_ViewCount.PropValue as bigint)	as [ViewCount],
		xref_FilePath.PropValue						as [FilePath],
		xref_Description.PropValue					as [Description],
		xref_FileHash_SHA256.PropValue				as [FileHash_SHA256],
		xref_SourceName.PropValue					as [SourceName],
		xref_SourceType.PropValue					as [SourceType],
		xref_DownloadName.PropValue					as [DownloadName],
		xref_Parent.ParentEntID						as [ParentID],
		ent.IsDeleted,
		ent.CreatedBy,
		ent.CreatedDate,
		ent.ModifiedBy,
		ent.ModifiedDate,
		ent.DeletedBy,
		ent.DeletedDate


	from 
		dbo.Entities ent

	inner join dbo.EntPropertyXRef xref_Name
	on 
		xref_Name.EntID = ent.ID
		and xref_Name.PropID = 2


	inner join dbo.EntPropertyXRef xref_ViewCount
	on 
		xref_ViewCount.EntID = ent.ID
		and xref_ViewCount.PropID = 14


	inner join dbo.EntPropertyXRef xref_FilePath
	on 
		xref_FilePath.EntID = ent.ID
		and xref_FilePath.PropID = 23


	inner join dbo.EntPropertyXRef xref_Description
	on 
		xref_Description.EntID = ent.ID
		and xref_Description.PropID = 24


	inner join dbo.EntPropertyXRef xref_FileHash_SHA256
	on 
		xref_FileHash_SHA256.EntID = ent.ID
		and xref_FileHash_SHA256.PropID = 25

	inner join dbo.EntPropertyXRef xref_SourceName
	on 
		xref_SourceName.EntID = ent.ID
		and xref_SourceName.PropID = 26


	inner join dbo.EntPropertyXRef xref_SourceType
	on 
		xref_SourceType.EntID = ent.ID
		and xref_SourceType.PropID = 28


	inner join dbo.EntPropertyXRef xref_DownloadName
	on 
		xref_DownloadName.EntID = ent.ID
		and xref_DownloadName.PropID = 29


	inner join dbo.EntChildXRef xref_Parent
	on
		xref_Parent.ChildEntID = ent.ID


	where
		ent.EntTypeID = 9
		AND ent.IsDeleted = 0







