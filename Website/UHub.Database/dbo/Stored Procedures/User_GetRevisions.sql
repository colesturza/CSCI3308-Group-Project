CREATE proc [dbo].[User_GetRevisions]

	@UserID bigint

as
begin


	declare @entType smallint = 1


	declare @isEnabled bit
	declare @isReadOnly bit
	declare @isDeleted bit
	declare @createdBy bigint
	declare @createdDate datetimeoffset(7)
	declare @modDate datetimeoffset(7)
	select
		@isEnabled = IsEnabled,
		@isReadOnly = IsReadOnly,
		@isDeleted = IsDeleted,
		@createdBy = CreatedBy,
		@createdDate = CreatedDate,
		@modDate = ModifiedDate
	from dbo.Entities
	where
		ID = @UserID



	if(@isDeleted  is null or @isDeleted = 1)
	begin
		return;
	end


	declare @parentID bigint
	select
		@parentID = ParentEntID
	from EntChildXRef
	where
		ChildEntID = @UserID



	declare @uEmail nvarchar(250)
	declare @uDomain nvarchar(250)
	declare @uUsername nvarchar(100)
	declare @uIsConfirmed bit
	declare @uIsApproved bit
	declare @uVersion nvarchar(20)
	declare @uIsAdmin bit
	select
		@uEmail = Email,
		@uDomain = Domain,
		@uUsername = Username,
		@uIsConfirmed = IsConfirmed,
		@uIsApproved = IsApproved,
		@uVersion = [Version],
		@uIsAdmin = IsAdmin
	from Users
	where
		EntID = @UserID;


	with DynamicSet1 as
	(
		select 
			EntID,
			case when eprx.PropID = 2
			then PropValue
			else NULL
			end [Name],
			--
			case when eprx.PropID = 6
			then PropValue
			else NULL
			end [PhoneNumber],
			--
			case when eprx.PropID = 7
			then PropValue
			else NULL
			end [Major],
			--
			case when eprx.PropID = 8
			then PropValue
			else NULL
			end [Year],
			--
			case when eprx.PropID = 10
			then PropValue
			else NULL
			end [GradDate],
			--
			case when eprx.PropID = 30
			then PropValue
			else NULL
			end [Company],
			--
			case when eprx.PropID = 31
			then PropValue
			else NULL
			end [JobTitle],
			--
			case when eprx.PropID = 36
			then PropValue
			else NULL
			end [IsFinished],
			CreatedBy,
			CreatedDate
		from
			EntPropertyRevisionXRef eprx
		where
			eprx.EntID = @UserID
			and eprx.EntTypeID = @entType
			and CreatedDate != @modDate
	),
	DynamicSet2 as
	(
		select
			EntID,
			min([Name])						[Name],
			min(PhoneNumber)				PhoneNumber,
			min(Major)						Major,
			min([Year])						[Year],
			min(GradDate)					GradDate,
			min(Company)					Company,
			min(JobTitle)					JobTitle,
			cast(min(IsFinished) as bit)	IsFinished,
			min(CreatedBy)					ModifiedBy,
			min(CreatedDate)				ModifiedDate,
			ROW_NUMBER() over (order by CreatedDate)	as RowNum
		from DynamicSet1
		group by
			EntID,
			CreatedDate
	),	
	RecurseSet as
	(
		select
			prs.EntID,
			prs.[Name],
			prs.PhoneNumber,
			prs.Major,
			prs.[Year],
			prs.GradDate,
			prs.Company,
			prs.JobTitle,
			prs.IsFinished,
			prs.ModifiedBy,
			prs.ModifiedDate,
			prs.RowNum
		from DynamicSet2 prs
		where
			RowNum = 1


		UNION ALL


		select
			rs.EntID,
			coalesce(prs.[Name], rs.[Name])					as [Name],
			coalesce(prs.PhoneNumber, rs.PhoneNumber)		as PhoneNumber,
			coalesce(prs.Major, rs.Major)					as Major,
			coalesce(prs.[Year], rs.[Year])					as [Year],
			coalesce(prs.GradDate, rs.GradDate)				as GradDate,
			coalesce(prs.Company, rs.Company)				as Company,
			coalesce(prs.JobTitle, rs.JobTitle)				as JobTitle,
			coalesce(prs.IsFinished, rs.IsFinished)			as IsFinished,
			coalesce(prs.ModifiedBy, rs.ModifiedBy)			as ModifiedBy,
			coalesce(prs.ModifiedDate, rs.ModifiedDate)		as ModifiedDate,
			prs.RowNum										as RowNum
		from DynamicSet2 prs

		inner join RecurseSet rs
		on 
			prs.EntID = rs.EntID
			and prs.RowNum = rs.RowNum + 1

	)


	select * from vUsers
	where 
		ID = @UserID

	UNION ALL

	select
		@UserID			as ID,
		@entType		as EntTypeID,
		@isEnabled		as IsEnabled,
		@isReadOnly		as IsReadOnly,
		@uEmail			as Email,
		@uDomain		as Domain,
		@uUsername		as Username,
		@uIsConfirmed	as IsConfirmed,
		@uIsApproved	as IsApproved,
		@uVersion		as [Version],
		@uIsAdmin		as IsAdmin,
		rs.[Name],
		rs.PhoneNumber,
		rs.Major,
		rs.[Year],
		rs.GradDate,
		rs.Company,
		rs.JobTitle,
		rs.IsFinished,
		@parentID		as ParentID,
		0				as IsDeleted,
		@createdBy		as CreatedBy,
		@createdDate	as CreatedDate,
		rs.ModifiedBy,
		rs.ModifiedDate,
		NULL			as DeletedBy,
		NULL			as DeletedDate
	from RecurseSet rs

	order by ModifiedDate desc

end