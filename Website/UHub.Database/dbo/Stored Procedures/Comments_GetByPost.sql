﻿












CREATE proc [dbo].[Comments_GetByPost]

	@PostID bigint

as
begin


select
	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.[Content],
	vu.[IsModified],
	vu.[ViewCount],
	vu.ParentID,
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]


from dbo.vComments vu

JOIN dbo.EntChildXRef ON ParentEntID = @PostID AND ChildEntType = 7

end





