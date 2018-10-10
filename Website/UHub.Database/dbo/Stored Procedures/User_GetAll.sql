
CREATE proc [dbo].[User_GetAll]


as
begin


select
	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.RefUID,
	vu.Email,
	vu.Username,
	vu.IsConfirmed,
	vu.IsApproved,
	vu.[Version],
	vu.[IsAdmin],
	vu.[Name],
	vu.[PhoneNumber],
	vu.[Major],
	vu.[Year],
	vu.[GradDate],
	vu.[Company],
	vu.[JobTitle],
	vu.[IsFinished],
	vu.[SchoolID],
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]


from dbo.vUsers vu


end
