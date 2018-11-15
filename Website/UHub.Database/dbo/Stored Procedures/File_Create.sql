﻿


CREATE proc [dbo].[File_Create]

	--CREATE PARAMETERS
	
	--DYNAMIC
	@Name nvarchar(max),
	@FilePath nvarchar(max),
	@Description nvarchar(max),
	@FileHash_SHA256 nvarchar(max),
	@SourceName nvarchar(max),
	@SourceType nvarchar(max),
	@DownloadName nvarchar(max),

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

		--POST ENT ID => 8
		--use variable to manage ent type easier
		declare @_entTypeID smallint
		set @_entTypeID = 8
		declare @_entID bigint
		declare @_isNew bit = 1

		declare @_prnt_ID bigint
		declare @_prnt_IsDeleted bit
		declare @_prnt_IsEnabled bit
		declare @_prnt_IsReadOnly bit
		declare @_prnt_EntType smallint

		SELECT
			@_prnt_ID = e.ID,
			@_prnt_IsDeleted = e.IsDeleted,
			@_prnt_IsEnabled = e.IsEnabled,
			@_prnt_IsReadOnly = e.IsReadOnly,
			@_prnt_EntType = e.EntTypeID

		from dbo.Entities e
		where
			e.ID = @ParentID

		--validate parent existance
		if(@_prnt_ID is null)
		begin
			;throw 51000, '400: Invalid parent entity', 1;
		end
		--ensure that parent entity is not the same as current entity (prevent circular links)
		if(@_prnt_ID = @_entTypeID)
		begin
			;throw 51000, '400: Invalid parent entity', 1;
		end
		--validate parent deletion flag
		--ensure that parent is not deleted
		--use temptable record
		if(@_prnt_IsDeleted = 1)
		begin
			;throw 51000, '410: Parent no longer exists', 1;
		end
		--validate parent enabled flag
		if(@_prnt_IsEnabled = 0)
		begin
			;throw 51000, '403: This Parent cannot be modified', 1;
		end
		--validate parent readonly flag
		--ensure that parent entity does not have the readonly flag set
		--use temptable record
		if(@_prnt_IsReadOnly = 1)
		begin
			;throw 51000, '400: Parent cannot be modified', 1;
		end

		--handle empty args
		--default to system user
		if(@CreatedBy is null)
		begin
			set @CreatedBy = 0
		end

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


		--get the id of the inserted object
		--SCOPE_IDENTITY()
		--use this to handle property inserts
		select @_entID = SCOPE_IDENTITY()


		--try to insert
		--wrap in try/catch to control the error message output
		--this insert is heavily controlled by relationship constraints
		--the insert will only succeed if the parent/child mapping exists in dbo.EntChildMap
		begin try
			--add record to dbo.EntChildXRef
			insert into dbo.EntChildXRef
			(
				ParentEntID,
				ChildEntID,
				ParentEntType,
				ChildEntType
			)
			values
			(
				@ParentID,
				@_entID,
				@_prnt_EntType,
				@_entTypeID
			)
		end try
		begin catch
			;throw 51000, '400: Invalid parent entity', 1;
		end catch


		--set properties
		--if any insert fails, it will throw an error and invalidate the entire entity

		--Name [2]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 2,
			@PropValue = @Name,
			@ModifiedBy = @CreatedBy,
			@IsNewRecord = @_isNew

		--ViewCount [14]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 14,
			@PropValue = NULL,
			@ModifiedBy = @CreatedBy,
			@IsNewRecord = @_isNew

		--FilePath [23]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 12,
			@PropValue = @FilePath,
			@ModifiedBy = @CreatedBy,
			@IsNewRecord = @_isNew

		--Description [24]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 24,
			@PropValue = @Description,
			@ModifiedBy = @CreatedBy,
			@IsNewRecord = @_isNew

		--FileHash_SHA256 [25]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 25,
			@PropValue = @FileHash_SHA256,
			@ModifiedBy = @CreatedBy,
			@IsNewRecord = @_isNew

		--SourceName [26]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 26,
			@PropValue = @SourceName,
			@ModifiedBy = @CreatedBy,
			@IsNewRecord = @_isNew

		--SourceType [28]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 28,
			@PropValue = @SourceType,
			@ModifiedBy = @CreatedBy,
			@IsNewRecord = @_isNew

		--DownloadName [29]
		exec [dbo].[_vEnts_Helper]
			@EntID = @_entID,
			@EntTypeID = @_entTypeID,
			@PropID = 29,
			@PropValue = @DownloadName,
			@ModifiedBy = @CreatedBy,
			@IsNewRecord = @_isNew

		select @_entID

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