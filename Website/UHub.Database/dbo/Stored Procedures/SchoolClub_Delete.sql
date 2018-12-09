create proc SchoolClub_Delete

	@ClubID bigint,
	@DeletedBy bigint

as
begin

	begin try
		BEGIN TRAN

			declare @deletedDate datetimeoffset(7) = sysdatetimeoffset()
	

			--mark club as deleted
			update Entities
			set
				IsDeleted = 1,
				DeletedBy = @DeletedBy,
				DeletedDate = @deletedDate
			where
				ID = @ClubID
				and EntTypeID = 4			--SCHOOL CLUB TYPE [4]


			--get descendant posts
			SELECT ChildEntID INTO #childPostSet
			FROM EntChildXRef
			where
				ParentEntID = @ClubID
				and ParentEntType = 4		--SCHOOL CLUB TYPE [4]


			--mark child posts as deleted
			update Entities
			set
				IsDeleted = 1,
				DeletedBy = @DeletedBy,
				DeletedDate = @deletedDate
			where
				ID in (select ChildEntID from #childPostSet) 
				and EntTypeID = 6;		--POST TYPE [6]



			--find all child comments of post set
			with recursiveFind as
			(
				select
					ChildEntID,
					ChildEntType,
					ParentEntID,
					ParentEntType
				from dbo.EntChildXRef ecx
				where
					ecx.ChildEntType = 7
					and ecx.ParentEntID in (select ChildEntID from #childPostSet) 
					and ecx.ParentEntType = 6 --POST TYPE [6]
					and ecx.IsDeleted = 0


				UNION ALL

	
				select
					ecx.ChildEntID,
					ecx.ChildEntType,
					ecx.ParentEntID,
					ecx.ParentEntType
				from dbo.EntChildXRef ecx

				inner join recursiveFind rf
				on
					ecx.ParentEntID = rf.ChildEntID
					and ecx.ParentEntType = 7 --COMMENT TYPE [6]

				where
					ecx.ChildEntType = 7
					and ecx.IsDeleted = 0
			)

			SELECT ChildEntID INTO #childCommentSet
			FROM recursiveFind


			--mark child comments as deleted
			update Entities
			set
				IsDeleted = 1,
				DeletedBy = @DeletedBy,
				DeletedDate = @deletedDate
			where
				ID in (select ChildEntID from #childCommentSet)



			--mark child relationships as deleted
			update EntChildXRef
			set
				IsDeleted = 1
			where
				ChildEntID = @ClubID
				or ParentEntID = @ClubID
				--
				or ChildEntID in (select ChildEntID from #childCommentSet)
				or ParentEntID in (select ChildEntID from #childCommentSet)
				--
				or ChildEntID in (select ChildEntID from #childPostSet)
				or ParentEntID in (select ChildEntID from #childPostSet)
	


			COMMIT TRAN

			drop table #childCommentSet
			drop table #childPostSet

	end try
	begin catch
		if(@@TRANCOUNT > 0)
		begin
			ROLLBACK TRAN
		end

		;throw;

	end catch

end