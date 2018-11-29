












CREATE proc [dbo].[Comments_GetByPost]

	@PostID bigint

as
begin

with recursiveFind as
(

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

	inner JOIN dbo.EntChildXRef
	ON
		ChildEntID = vu.ID
		and ChildEntType = 7
		and ParentEntID = @PostID 
		and ParentEntType = 6 --POST TYPE [6]


	UNION ALL

	
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

	inner JOIN dbo.EntChildXRef ecx
	ON
		ChildEntID = vu.ID
		and ChildEntType = 7

	inner join recursiveFind rf
	on
		ecx.ParentEntID = rf.ID
		and ParentEntType = 7 --COMMENT TYPE [6]
)



select * from recursiveFind

end






