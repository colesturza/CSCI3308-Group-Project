﻿

create proc [dbo].[User_Update]

	--UPDATE PARAMETERS
	@EntID bigint,

	--DYNAMIC
	@Name nvarchar(max),
	@PhoneNumber nvarchar(max) null,
	@Major nvarchar(max),
	@Year nvarchar(max) null,
	@GradDate nvarchar(max) null,
	@Company nvarchar(max) null,
	@JobTitle nvarchar(max) null,
	@IsFinished bit null,

	--CONST ENT FIELDS
	@ModifiedBy bigint null = null

as
begin

	--wrap code in transaction -> any error will invalidate the entire insert
	begin try
		BEGIN TRAN

		--POST ENT ID => 6
		--use variable to manage ent type easier
		declare @_entTypeID smallint
		set @_entTypeID = 6
		declare @_isNew bit = 0
		declare @_prntID bigint



		if(@EntID is null)
		begin
			;throw 51000, '400: User is invalid', 1;
		end

		--CHECK ENTITY STATUS
		declare @_pst_IsDeleted bit
		declare @_pst_IsEnabled bit
		declare @_pst_IsReadonly bit 
		
		select
			@_pst_IsDeleted = e.IsDeleted,
			@_pst_IsEnabled = e.IsEnabled,
			@_pst_IsReadonly = e.IsReadOnly
		from dbo.Entities e
		where
			ID = @EntID
			and EntTypeID = @_entTypeID

		if(@_pst_IsDeleted is null)
		begin
			;throw 51000, '400: User is invalid', 1;
		end

		if(@_pst_IsDeleted = 1)
		begin
			;throw 51000, '410: User no longer exists', 1;
		end
		if(@_pst_IsEnabled = 1)
		begin
			;throw 51000, '400: User cannot be modified', 1;
		end
		if(@_pst_IsReadonly = 1)
		begin
			;throw 51000, '400: User cannot be modified', 1;
		end


		--CHECK PARENT STATUS
		select
			@_prntID = ParentEntID
		from dbo.EntChildXRef
		where
			ChildEntID = @EntID
			and ChildEntType = @_entTypeID


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
			e.ID = @_prntID


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
			;throw 51000, '403: Parent cannot be modified', 1;
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
		if(@ModifiedBy is null)
		begin
			set @ModifiedBy = 0
		end

		declare @ModifiedDate datetimeoffset(7) = sysDateTimeOffset()



		--set properties
		--if any insert fails, it will throw an error and invalidate the entire entity
		declare @updateStatus tinyint = 0
		declare @tmpStatus tinyint = 0

		--Name [2]
		exec @tmpStatus = [dbo].[_vEnts_Helper]
			@EntID = @EntID,
			@EntTypeID = @_entTypeID,
			@PropID = 2,
			@PropValue = @Name,
			@ModifiedBy = @ModifiedBy,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		set @updateStatus = @updateStatus + @tmpStatus


		--PhoneNum [6]
		exec [dbo].[_vEnts_Helper]
			@EntID = @EntID,
			@EntTypeID = @_entTypeID,
			@PropID = 6,
			@PropValue = @PhoneNumber,
			@ModifiedBy = @ModifiedBy,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		set @updateStatus = @updateStatus + @tmpStatus


		--Major [7]
		exec [dbo].[_vEnts_Helper]
			@EntID = @EntID,
			@EntTypeID = @_entTypeID,
			@PropID = 7,
			@PropValue = @Major,
			@ModifiedBy = @ModifiedBy,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		set @updateStatus = @updateStatus + @tmpStatus


		--Year [8]
		exec [dbo].[_vEnts_Helper]
			@EntID = @EntID,
			@EntTypeID = @_entTypeID,
			@PropID = 8,
			@PropValue = @Year,
			@ModifiedBy = @ModifiedBy,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		set @updateStatus = @updateStatus + @tmpStatus

		
		--GradDate [10]
		exec [dbo].[_vEnts_Helper]
			@EntID = @EntID,
			@EntTypeID = @_entTypeID,
			@PropID = 10,
			@PropValue = @GradDate,
			@ModifiedBy = @ModifiedBy,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		set @updateStatus = @updateStatus + @tmpStatus


		--Company [30]
		exec [dbo].[_vEnts_Helper]
			@EntID = @EntID,
			@EntTypeID = @_entTypeID,
			@PropID = 30,
			@PropValue = @Company,
			@ModifiedBy = @ModifiedBy,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		set @updateStatus = @updateStatus + @tmpStatus


		--JobTitle [30]
		exec [dbo].[_vEnts_Helper]
			@EntID = @EntID,
			@EntTypeID = @_entTypeID,
			@PropID = 31,
			@PropValue = @JobTitle,
			@ModifiedBy = @ModifiedBy,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		set @updateStatus = @updateStatus + @tmpStatus


		--IsFinished [36]
		exec [dbo].[_vEnts_Helper]
			@EntID = @EntID,
			@EntTypeID = @_entTypeID,
			@PropID = 36,
			@PropValue = @IsFinished,
			@ModifiedBy = @ModifiedBy,
			@ModifiedDate = @ModifiedDate,
			@IsNewRecord = @_isNew
		set @updateStatus = @updateStatus + @tmpStatus
		


		if(@updateStatus > 0)
		begin
			--IsModified [13]
			exec [dbo].[_vEnts_Helper]
				@EntID = @EntID,
				@EntTypeID = @_entTypeID,
				@PropID = 13,
				@PropValue = 1,
				@ModifiedBy = @ModifiedBy,
				@ModifiedDate = @ModifiedDate,
				@IsNewRecord = @_isNew


			update Entities
			set
				ModifiedDate = @ModifiedDate
			where
				ID = @EntID

		end





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