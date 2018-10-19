




CREATE view [dbo].[vSchoolMajors]
as

select
	ent.ID,
	ent.EntTypeID,
	ent.IsEnabled,
	ent.IsReadonly,
	xref_Name.PropValue			as [Name],
	xref_Description.PropValue	as [Description],
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


inner join dbo.EntPropertyXRef xref_Description
on 
	xref_Description.EntID = ent.ID
	and xref_Description.PropID = 24


where ent.EntTypeID = 3








