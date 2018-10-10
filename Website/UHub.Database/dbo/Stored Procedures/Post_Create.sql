

CREATE proc [dbo].[Post_Create]

	--CREATE PARAMETERS
	
	--DYNAMIC
	@Name nvarchar(200),
	@Content nvarchar(max),

	--HIERARCHY
	@ParentID bigint,


	--CONST ENT FIELDS
	@CreatedBy bigint,
	@IsReadonly bit

as
begin

	--wrap code in transaction -> any error will invalidate the entire insert
	begin try
		BEGIN TRAN

		--POST ENT ID => 6
		--use variable to manage ent type easier
		declare @_entTypeID smallint
		set @_entTypeID = 6


		--TODO
		--throw error if parent ID is not found in DB
		--Look in dbo.Entities
		--Error syntax:
		--;throw 51000, '400: Property is invalid for specified entity type', 1;
		--
		--best solution:
		--load parent into temp table
		--check if the temp table ID column is null
		--this will make validation faster later




		--TODO
		--get parent ent type ID
		--this will be important for creating the parent/child link

		

		--TODO
		--validate arguments
		--ensure that @IsReadonly is not null -> it should default to 0




		--insert item into ent table
		insert into dbo.Entities
		(
			EntTypeID,
			CreatedBy
		)
		values
		( 
			@_entTypeID,
			@CreatedBy
		)


		--TODO
		--get the id of the inserted object
		--SCOPE_IDENTITY()
		--use this to handle property inserts


		--TODO
		--ensure that parent is not deleted
		--use temptable record
		--Error Msg:
		-- '410: Parent no longer exists'



		--TODO
		--ensure that parent entity does not have the readonly flag set
		--use temptable record
		--Error Msg:
		-- '400: Parent cannot be modified'


		--TODO
		--ensure that parent entity is not the same as current entity (prevent circular links)
		--Error Msg:
		-- '400: Invalid parent entity'



		--try to insert
		--wrap in try/catch to control the error message output
		--this insert is heavily controlled by relationship constraints
		--the insert will only succeed if the parent/child mapping exists in dbo.EntChildMap
		begin try
			--TODO
			--remove select (its just a placeholder)
			--replace it with insert
			--add record to dbo.EntChildXRef

			select * from Entities --DELETE ME
		end try
		begin catch
			;throw 51000, '400: Invalid parent entity', 1;
		end catch




		--TODO
		--Use helper func to set properties
		--will need to be called for each dynamic property
		--if any insert fails, it will throw an error and invalidate the entire entity
		exec [dbo].[_vEnts_Helper]
			@EntID = 1,
			@EntTypeID = @_entTypeID,
			@PropID = 1,
			@PropValue = '',
			@ModifiedBy = 1,
			@IsNewRecord = 1

		

		--TODO
		--update Ent.ModifiedBy & Ent.ModifiedDate
		--Found in dbo.Entities
		

		COMMIT TRAN
	end try
	begin catch
		if(@@TRANCOUNT > 0)
		begin
			ROLLBACK TRAN
		end

		;throw;

	end catch


end
