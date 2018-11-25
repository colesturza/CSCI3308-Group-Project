CREATE proc [dbo].[Post_GetRevisions]

	@PostID bigint

as
begin

	declare @entType smallint = 6


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
		ID = @PostID



	if(@isDeleted  is null or @isDeleted = 1)
	begin
		return;
	end


	declare @parentID bigint
	select
		@parentID = ParentEntID
	from EntChildXRef
	where
		ChildEntID = @PostID;


	with DynamicSet1 as
	(
		select 
			EntID,
			case when eprx.PropID = 2
			then PropValue
			else NULL
			end [Name],
			--
			case when eprx.PropID = 12
			then PropValue
			else NULL
			end [Content],
			--
			case when eprx.PropID = 13
			then PropValue
			else NULL
			end [IsModified],
			--
			case when eprx.PropID = 14
			then PropValue
			else NULL
			end [ViewCount],
			--
			case when eprx.PropID = 33
			then PropValue
			else NULL
			end [IsLocked],
			--
			case when eprx.PropID = 34
			then PropValue
			else NULL
			end [CanComment],
			--
			case when eprx.PropID = 35
			then PropValue
			else NULL
			end [IsPublic],
			CreatedBy,
			CreatedDate

		from
			EntPropertyRevisionXRef eprx
		where
			eprx.EntID = @PostID
			and eprx.EntTypeID = @entType
			and CreatedDate != @modDate
	),
	DynamicSet2 as
	(
		select
			EntID,
			min([Name])						[Name],
			min(content)					Content,
			cast(min(IsModified) as bit)	IsModified,
			cast(min(ViewCount) as bigint)	ViewCount,
			cast(min(IsLocked) as bit)		IsLocked,
			cast(min(CanComment) as bit)	CanComment,
			cast(min(IsPublic) as bit)		IsPublic,
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
			prs.Content,
			prs.IsModified,
			prs.ViewCount,
			prs.IsLocked,
			prs.CanComment,
			prs.IsPublic,
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
			coalesce(prs.Content, rs.Content)				as [Content],
			coalesce(prs.IsModified, rs.IsModified)			as [IsModified],
			coalesce(prs.ViewCount, rs.ViewCount)			as [ViewCount],
			coalesce(prs.IsLocked, rs.IsLocked)				as [IsLocked],
			coalesce(prs.CanComment, rs.CanComment)			as [CanComment],
			coalesce(prs.IsPublic, rs.IsPublic)				as [IsPublic],
			coalesce(prs.ModifiedBy, rs.ModifiedBy)			as ModifiedBy,
			coalesce(prs.ModifiedDate, rs.ModifiedDate)		as ModifiedDate,
			prs.RowNum										as RowNum
		from DynamicSet2 prs

		inner join RecurseSet rs
		on 
			prs.EntID = rs.EntID
			and prs.RowNum = rs.RowNum + 1

	)


	select
		vp.ID,
		vp.EntTypeID,
		vp.IsEnabled,
		vp.IsReadOnly,
		vp.[Name],
		vp.Content,
		vp.IsModified,
		vp.ViewCount,
		vp.IsLocked,
		vp.CanComment,
		vp.IsPublic,
		vp.ParentID,
		vp.IsDeleted,
		vp.CreatedBy,
		vp.CreatedDate,
		vp.ModifiedBy,
		vp.ModifiedDate,
		vp.DeletedBy,
		vp.DeletedDate
	from vPosts vp
	where
		ID = @PostID


	UNION ALL


	select
		@PostID			as ID,
		@entType		as EntTypeID,
		@isEnabled		as IsEnabled,
		@isReadOnly		as IsReadOnly,
		rs.[Name],
		rs.Content,
		rs.IsModified,
		rs.ViewCount,
		rs.IsLocked,
		rs.CanComment,
		rs.IsPublic,
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