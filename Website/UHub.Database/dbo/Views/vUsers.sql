











CREATE view [dbo].[vUsers]
as

--USER ENT TYPE => 1


	select
		ent.ID,
		ent.EntTypeID,
		cast(case when (u.IsConfirmed = 1 AND u.IsApproved = 1 AND ent.IsEnabled = 1 AND ent.IsDeleted = 0) then 1 else 0 end as bit) as IsEnabled,
		ent.IsReadOnly,
		u.RefUID,
		u.Email,
		u.Domain,
		u.Username,
		u.IsConfirmed,
		u.IsApproved,
		u.[Version],
		u.IsAdmin,
		xref_Name.PropValue									as [Name],
		xref_PhoneNumber.PropValue							as [PhoneNumber],
		xref_Major.PropValue								as [Major],
		xref_Year.PropValue									as [Year],
		xref_GradDate.PropValue								as [GradDate],
		xref_Company.PropValue								as [Company],
		xref_JobTitle.PropValue								as [JobTitle],
		cast(xref_IsFinished.PropValue as bit)				as [IsFinished],
		xref_School.ParentEntID								as [SchoolID],
		ent.IsDeleted,
		ent.CreatedBy,
		ent.CreatedDate,
		ent.ModifiedBy,
		ent.ModifiedDate,
		ent.DeletedBy,
		ent.DeletedDate


	from 
		dbo.Entities ent

	inner join dbo.Users u
	on
		ent.ID = u.EntID


	inner join dbo.EntPropertyXRef xref_Name
	on 
		xref_Name.EntID = ent.ID
		and xref_Name.PropID = 2


	inner join dbo.EntPropertyXRef xref_PhoneNumber
	on 
		xref_PhoneNumber.EntID = ent.ID
		and xref_PhoneNumber.PropID = 6


	inner join dbo.EntPropertyXRef xref_Major
	on 
		xref_Major.EntID = ent.ID
		and xref_Major.PropID = 7


	inner join dbo.EntPropertyXRef xref_Year
	on 
		xref_Year.EntID = ent.ID
		and xref_Year.PropID = 8


	inner join dbo.EntPropertyXRef xref_GradDate
	on 
		xref_GradDate.EntID = ent.ID
		and xref_GradDate.PropID = 10


	inner join dbo.EntPropertyXRef xref_Company
	on 
		xref_Company.EntID = ent.ID
		and xref_Company.PropID = 30


	inner join dbo.EntPropertyXRef xref_JobTitle
	on 
		xref_JobTitle.EntID = ent.ID
		and xref_JobTitle.PropID = 31


	inner join dbo.EntPropertyXRef xref_IsFinished
	on 
		xref_IsFinished.EntID = ent.ID
		and xref_IsFinished.PropID = 36


	--ENT SCHOOL TYPE => 2
	left join dbo.EntChildXRef xref_School
	on
		xref_School.ChildEntID = ent.ID
		and xref_School.ParentEntType = 2



	where
		ent.EntTypeID = 1
		AND ent.IsDeleted = 0













