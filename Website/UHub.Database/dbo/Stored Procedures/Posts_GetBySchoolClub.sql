












CREATE proc [dbo].[Posts_GetBySchoolClub]

	@SchoolClubID bigint

as
begin


select
	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.[Name],
	vu.[Content],
	vu.[IsModified],
	vu.[ViewCount],
	vu.[IsLocked],
	vu.[CanComment],
	vu.[IsPublic],
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]


from dbo.vPosts vu

JOIN dbo.EntChildXRef ON ParentEntID = @SchoolClubID AND ChildEntType = 6

end






