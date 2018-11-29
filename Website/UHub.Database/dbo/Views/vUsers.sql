create view vUsers

as

with set1 as
(
	select 
		EntID,
		case when epx.PropID = 2
		then PropValue
		else NULL
		end [Name],
		--
		case when epx.PropID = 6
		then PropValue
		else NULL
		end [PhoneNumber],
		--
		case when epx.PropID = 7
		then PropValue
		else NULL
		end [Major],
		--
		case when epx.PropID = 8
		then PropValue
		else NULL
		end [Year],
		--
		case when epx.PropID = 10
		then PropValue
		else NULL
		end [GradDate],
		--
		case when epx.PropID = 30
		then PropValue
		else NULL
		end [Company],
		--
		case when epx.PropID = 31
		then PropValue
		else NULL
		end [JobTitle],
		--
		case when epx.PropID = 36
		then PropValue
		else NULL
		end [IsFinished]

	from
		EntPropertyXRef epx
	where
		epx.EntTypeID = 1
),
set2 as
(
	select
		EntID,
		min([name])						[Name],
		min(PhoneNumber)				PhoneNumber,
		min(Major)						Major,
		min([Year])						[Year],
		min(GradDate)					GradDate,
		min(Company)					Company,
		min(JobTitle)					JobTitle,
		cast(min(IsFinished) as bit)	IsFInished

	from set1
	group by
		EntID
)

select
	ent.ID,
	ent.EntTypeID,
	cast(case when (u.IsConfirmed = 1 AND u.IsApproved = 1 AND ent.IsEnabled = 1 AND ent.IsDeleted = 0) then 1 else 0 end as bit) as IsEnabled,
	ent.IsReadOnly,
	u.Email,
	u.Domain,
	u.Username,
	u.IsConfirmed,
	u.IsApproved,
	u.[Version],
	u.IsAdmin,
	s2.[Name],
	s2.PhoneNumber,
	s2.Major,
	s2.[Year],
	s2.GradDate,
	s2.Company,
	s2.JobTitle,
	s2.IsFInished,
	xref_School.ParentEntID		[SchoolID],
	ent.IsDeleted,
	ent.CreatedBy,
	ent.CreatedDate,
	ent.ModifiedBy,
	ent.ModifiedDate,
	ent.DeletedBy,
	ent.DeletedDate
from Entities ent

inner join dbo.Users u
on
	ent.ID = u.EntID

inner join set2 s2
on
	ent.ID = s2.EntID

left join dbo.EntChildXRef xref_School
on
		xref_School.ChildEntID = ent.ID
		and xref_School.ParentEntType = 2

where
	ent.IsDeleted = 0













