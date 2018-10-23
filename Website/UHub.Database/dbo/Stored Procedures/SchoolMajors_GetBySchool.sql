









CREATE proc [dbo].[SchoolMajors_GetBySchool]

	@SchoolID bigint

as
begin


	select
		vu.ID,
		vu.IsEnabled,
		vu.IsReadonly,
		vu.[Name],
		vu.[Description],
		vu.[IsDeleted],
		vu.[CreatedBy],
		vu.[CreatedDate],
		vu.[ModifiedBy],
		vu.[ModifiedDate],
		vu.[DeletedBy],
		vu.[DeletedDate]

	from dbo.vSchoolMajors vu

	INNER JOIN dbo.EntChildXRef
	ON 
		ParentEntID = @SchoolID
		AND ChildEntID = vu.ID

end



