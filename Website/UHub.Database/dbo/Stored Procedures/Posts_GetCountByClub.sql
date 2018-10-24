create proc Posts_GetCountByClub

	@ClubID bigint
as
begin


	select
		count(*)
	from dbo.EntChildXRef
	where
		ParentEntID = @ClubID
		AND ChildEntType = 6	--POST TYPE [6]
		AND IsDeleted = 0


end