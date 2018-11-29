






CREATE view [dbo].[vSchools_OLD]
as


	select
		ent.ID,
		ent.EntTypeID,
		ent.IsEnabled,
		ent.IsReadonly,
		xref_Name.PropValue				as [Name],
		xref_State.PropValue			as [State],
		xref_City.PropValue				as [City],
		xref_DomainValidator.PropValue	as [DomainValidator],
		xref_Description.PropValue		as [Description],
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


	inner join dbo.EntPropertyXRef xref_State
	on 
		xref_State.EntID = ent.ID
		and xref_State.PropID = 15


	inner join dbo.EntPropertyXRef xref_City
	on 
		xref_City.EntID = ent.ID
		and xref_City.PropID = 16


	inner join dbo.EntPropertyXRef xref_DomainValidator
	on 
		xref_DomainValidator.EntID = ent.ID
		and xref_DomainValidator.PropID = 17


	inner join dbo.EntPropertyXRef xref_Description
	on 
		xref_Description.EntID = ent.ID
		and xref_Description.PropID = 24


	where
		ent.EntTypeID = 2
		AND ent.IsDeleted = 0