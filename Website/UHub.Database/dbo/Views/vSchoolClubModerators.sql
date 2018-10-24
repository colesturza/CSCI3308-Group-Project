




CREATE view [dbo].[vSchoolClubModerators]
as

	select
		ent.ID,
		ent.EntTypeID,
		ent.IsEnabled,
		ent.IsReadonly,
		cast(xref_UserID.PropValue as bigint)	as [UserID],
		cast(xref_IsOwner.PropValue as bit)		as [IsOwner],
		cast(xref_IsValid.PropValue as bit)		as [IsValid],
		ent.IsDeleted,
		ent.CreatedBy,
		ent.CreatedDate,
		ent.ModifiedBy,
		ent.ModifiedDate,
		ent.DeletedBy,
		ent.DeletedDate


	from 
		dbo.Entities ent

	inner join dbo.EntPropertyXRef xref_UserID
	on 
		xref_UserID.EntID = ent.ID
		and xref_UserID.PropID = 18


	inner join dbo.EntPropertyXRef xref_IsOwner
	on 
		xref_IsOwner.EntID = ent.ID
		and xref_IsOwner.PropID = 19


	inner join dbo.EntPropertyXRef xref_IsValid
	on 
		xref_IsValid.EntID = ent.ID
		and xref_IsValid.PropID = 22


	where
		ent.EntTypeID = 5
		AND ent.IsDeleted = 0







