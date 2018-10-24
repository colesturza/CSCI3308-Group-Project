





CREATE view [dbo].[vSchoolClubs]
as

	select
		ent.ID,
		ent.EntTypeID,
		ent.IsEnabled,
		ent.IsReadonly,
		xref_Name.PropValue			as [Name],
		xref_Description.PropValue	as [Description],
		xref_School.ParentEntID		as [SchoolID],
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
		and xref_Description.PropID = 11


	--ENT SCHOOL TYPE => 2
	inner join dbo.EntChildXRef xref_School
	on
		xref_School.ChildEntID = ent.ID
		and xref_School.ParentEntType = 2



	where
		ent.EntTypeID = 4
		AND ent.IsDeleted = 0







