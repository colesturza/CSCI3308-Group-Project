﻿









CREATE proc [dbo].[Post_GetByID]

	@PostID bigint

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

WHERE

	vu.ID = @PostID

end





