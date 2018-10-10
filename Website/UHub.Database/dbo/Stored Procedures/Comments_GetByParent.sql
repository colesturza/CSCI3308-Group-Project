













CREATE proc [dbo].[Comments_GetByParent]

	@ParentID bigint

as
begin


select
	vu.ID,
	vu.IsEnabled,
	vu.IsReadonly,
	vu.[Content],
	vu.[IsModified],
	vu.[ViewCount],
	vu.[IsDeleted],
	vu.[CreatedBy],
	vu.[CreatedDate],
	vu.[ModifiedBy],
	vu.[ModifiedDate],
	vu.[DeletedBy],
	vu.[DeletedDate]


from dbo.vComments vu

JOIN dbo.EntChildXRef ON ParentEntID = @ParentID AND ChildEntType = 7

end







