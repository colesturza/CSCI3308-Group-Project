











CREATE proc [dbo].[Posts_GetBySchool]

	@SchoolID bigint

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

JOIN dbo.EntChildXRef ON ParentEntID = @SchoolID AND ChildEntType = 6

end





