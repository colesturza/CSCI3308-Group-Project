

CREATE proc dbo.Posts_GetByPage

	@StartID bigint null,
	@PageNum int null,
	@ItemCount smallint

as
begin

	declare @adjStartIdx bigint
	declare @adjEndIdx bigint

	--set StartID to the id of the newest ent
	if(@StartID is null)
	begin

		select top(1)
			@StartID = ID
		from dbo.vPosts
		order by
			CreatedDate desc
	end
	select @StartID


	--return all items if count is -1
	if(@ItemCount = -1)
	begin
		select * from dbo.vPosts
		return;
	end


	--ensure that ItemCount is at least 1
	if(@ItemCount < 1)
	begin
		set @ItemCount = 1
	end


	--set default page number
	if(@PageNum is null)
	begin
		set @PageNum = 0

	end

	
	
	set @adjStartIdx = @StartID + @PageNum * @ItemCount;
	set @adjEndIdx = @adjStartIdx + @ItemCount;


	--get set of posts starting at the 
	with postSet as
	(

		select
			vp.*
			,ROW_NUMBER() over (order by CreatedDate desc) as RowNum
		from dbo.vPosts vp
		where 
			ID <= @StartID

	)

	select
		*
	from postSet
	where
		RowNum >= @adjStartIdx
		and RowNum < @adjEndIdx


end