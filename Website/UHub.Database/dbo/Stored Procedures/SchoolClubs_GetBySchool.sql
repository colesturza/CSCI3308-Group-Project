










CREATE proc [dbo].[SchoolClubs_GetBySchool]

	@SchoolID bigint

as
begin


	select
		vu.ID,
		vu.IsEnabled,
		vu.IsReadonly,
		vu.[Name],
		vu.[Description],
		vu.[SchoolID],
		vu.[IsDeleted],
		vu.[CreatedBy],
		vu.[CreatedDate],
		vu.[ModifiedBy],
		vu.[ModifiedDate],
		vu.[DeletedBy],
		vu.[DeletedDate]

	from dbo.vSchoolClubs vu

	INNER JOIN dbo.EntChildXRef
	ON 
		ParentEntID = @SchoolID
		AND ChildEntID = vu.ID

end




