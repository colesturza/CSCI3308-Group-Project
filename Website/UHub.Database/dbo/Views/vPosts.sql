







CREATE view [dbo].[vPosts]
as


select
	ent.ID,
	ent.EntTypeID,
	ent.IsEnabled,
	ent.IsReadonly,
	xref_Name.PropValue							as [Name],
	xref_Content.PropValue						as [Content],
	cast(xref_IsModified.PropValue as bit)		as [IsModified],
	cast(xref_ViewCount.PropValue as bigint)	as [ViewCount],
	cast(xref_IsLocked.PropValue as bit)		as [IsLocked],
	cast(xref_CanComment.PropValue as bit)		as [CanComment],
	cast(xref_IsPublic.PropValue as bit)		as [IsPublic],
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


inner join dbo.EntPropertyXRef xref_Content
on 
	xref_Content.EntID = ent.ID
	and xref_Content.PropID = 12


inner join dbo.EntPropertyXRef xref_IsModified
on 
	xref_IsModified.EntID = ent.ID
	and xref_IsModified.PropID = 13


inner join dbo.EntPropertyXRef xref_ViewCount
on 
	xref_ViewCount.EntID = ent.ID
	and xref_ViewCount.PropID = 14


inner join dbo.EntPropertyXRef xref_IsLocked
on 
	xref_IsLocked.EntID = ent.ID
	and xref_IsLocked.PropID = 33


inner join dbo.EntPropertyXRef xref_CanComment
on 
	xref_CanComment.EntID = ent.ID
	and xref_CanComment.PropID = 34


inner join dbo.EntPropertyXRef xref_IsPublic
on 
	xref_IsPublic.EntID = ent.ID
	and xref_IsPublic.PropID = 35


inner join dbo.EntChildXRef xref_Parent
on
	xref_Parent.ChildEntID = ent.ID


where ent.EntTypeID = 6









