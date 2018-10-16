




CREATE view [dbo].[vComments]
as

select
	ent.ID,
	ent.EntTypeID,
	ent.IsEnabled,
	ent.IsReadonly,
	xref_Content.PropValue						as [Content],
	cast(xref_IsModified.PropValue as bit)		as [IsModified],
	cast(xref_ViewCount.PropValue as bigint)	as [ViewCount],
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


inner join dbo.EntChildXRef xref_Parent
on
	xref_Parent.ChildEntID = ent.ID


where ent.EntTypeID = 7







