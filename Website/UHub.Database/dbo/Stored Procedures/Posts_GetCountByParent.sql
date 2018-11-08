create proc [dbo].[Posts_GetCountByParent]

	@ParentID bigint
as
begin


	select
		count(*)
	from dbo.EntChildXRef
	where
		ParentEntID = @ParentID
		AND ChildEntType = 6	--POST TYPE [6]
		AND IsDeleted = 0


end