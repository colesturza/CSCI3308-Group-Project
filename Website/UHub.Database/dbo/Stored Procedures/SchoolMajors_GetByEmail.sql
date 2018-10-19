









create proc [dbo].[SchoolMajors_GetByEmail]

	@Email nvarchar(250)

as
begin


	declare @_schoolID bigint

	select 
		@_schoolID = xr.EntID
	from dbo.EntPropertyXRef xr
	where
		xr.PropID = 17		--DomainValidator
		and @Email like '%' + xr.PropValue



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

	JOIN dbo.EntChildXRef 
	ON 
		ParentEntID = @_schoolID 
		AND ChildEntType = 3		--SCHOOL MAJOR [3]



end