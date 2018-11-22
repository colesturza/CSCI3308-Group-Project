create proc Post_IncrementViewCount

	@PostID bigint

as
begin


	if(exists (select * from Entities where ID = @PostID and EntTypeID = 6 and IsDeleted = 0))	--POST TYPE [6]
	begin
		
		update dbo.EntPropertyXRef
		set
			PropValue = cast(PropValue as bigint) + 1
		where
			EntID = @PostID
			and PropID = 14		--VIEW COUNT [14]




		select cast(1 as bit)
	end
	else begin

		select cast(0 as bit)

	end


end