
CREATE procedure [dbo].[_vEnts_Helper]

	@EntID int,
	@EntTypeID int,
	@PropID int,
	@PropValue nvarchar(max),
	@ModifiedBy bigint,
	@ModifiedDate datetimeoffset(7) = SysDateTimeOffset,
	@IsNewRecord bit

as
begin

	--empty values are pointless in this scheme
	--it takes less space to store an XREF null
	if(@PropValue = '')
	begin
		set @PropValue = null
	end

	--empty values are pointless in this scheme
	--it takes less space to store an XREF null

	begin try

		declare @_errorMsg nvarchar(500)

		--ensure property exists
		if((select top(1) ID from dbo.EntProperties where ID = @PropID) is null)
		begin
			;throw 51000, '400: Specified property ID does not exist', 1;
		end


		--get info about current modified data property
		--CONSTRAINED to the current entity type
		select epm.*, ep.PropFriendlyName into #EntProp_Current from dbo.EntPropertyMap epm
		left join dbo.EntProperties ep
		on
			ep.ID = @PropID
		where
			epm.EntTypeID = @EntTypeID
			and epm.PropID = @PropID



		--ensure that the specified entity can have the specified property
		--#EntProp_Current will have no records if the prop is not valid
		if((select count(*) from #EntProp_Current) = 0)
		begin
			--get prop friendly ID from table
			select @_errorMsg = '400: ''' + [PropFriendlyName] + ''' is invalid for the specified entity type'
			from dbo.EntProperties
			where
				ID = @PropID


			;throw 51000, @_errorMsg, 1;
		end


			
		declare @_canWriteToHistory bit = 0
		--try to cast the value to proper data type
		--if it fails, throw error
		declare @_out bit
		declare @_dataType nvarchar(100)
		select @_dataType = Datatype from dbo.EntProperties where ID = @PropID
		exec @_out = dbo._IsCastValid @PropValue, @_dataType
		if(@_out = 0)
		begin
				
			select @_errorMsg = '400: Supplied value for ''' + [PropFriendlyName] + ''' cannot be converted to proper data type'
			from #EntProp_Current

			;throw 51000, @_errorMsg, 1;
		end
		

		--nonNullable types need extra validation
		if((select IsNullable from #EntProp_Current) = 0)
		begin
			declare @_defaultPropVal nvarchar(max)
			select @_defaultPropVal = [DefaultValue] from #EntProp_Current
			--(new) throw error if user doesnt supply a value and there is no default
			--(existing) throw error if user doesnt supply a value
			if(@PropValue is NULL AND (@_defaultPropVal is NULL OR @IsNewRecord = 0))
			begin
				select @_errorMsg = '400: Entity property ''' + [PropFriendlyName] + ''' cannot be null or empty'
				from #EntProp_Current

				;throw 51000, @_errorMsg, 1;
			end
			--(new) use default value if user doesnt supply one
			else if(@IsNewRecord = 1 AND @PropValue is NULL AND @_defaultPropVal is not NULL)
			begin
				set @PropValue = @_defaultPropVal
			end
		end

		--drop prop validation table
		drop table #EntProp_Current

		--proceed after data validation

		--update/delete existing records
		if(exists (select * from dbo.EntPropertyXRef where EntTypeID = @EntTypeID AND EntID = @EntID AND PropID = @PropID))
		begin
			--keep empty prop record stubs
			--if(@PropValue is not NULL)
			--begin
				
				
				--compare old val to new val to check if the property has actually changed
				--useful for accurate change tracking log
				declare @_oldProp nvarchar(max)
				select
					@_oldProp = [PropValue]
				from dbo.EntPropertyXRef
				where
					EntTypeID = @EntTypeID
					AND EntID = @EntID
					AND PropID = @PropID

				if(
					(@_oldProp is NULL AND @PropValue is not NULL) OR
					(@_oldProp is not NULL AND @PropValue is NULL) OR
					(@_oldProp <> @PropValue)
				)
				begin

					set @_canWriteToHistory = 1
				--update property value
					update dbo.EntPropertyXRef
					set 
						PropValue = @PropValue,
						ModifiedBy = @ModifiedBy,
						ModifiedDate = @ModifiedDate
					where
						EntTypeID = @EntTypeID
						AND EntID = @EntID
						AND PropID = @PropID						
				end
			

			--end
			--else begin
			----delete standard property
			--	delete dbo.EntPropertyXRef
			--	where
			--		EntTypeID = @EntTypeID
			--		AND EntID = @EntID
			--		AND PropID = @PropID
			--end
		end
		--create new records
		else begin
			--create empty prop record stubs
			--if(@PropValue is not NULL)
			--begin

				
				set @_canWriteToHistory = 1
				--insert standard property
				insert into dbo.EntPropertyXRef
					(EntID, EntTypeID, PropID, PropValue, CreatedBy, CreatedDate, ModifiedBy)
				values
					(@EntID, @EntTypeID, @PropID, @PropValue, @ModifiedBy, @ModifiedDate, @ModifiedBy)
			
			
			--end
		end

		--if availble, write to revision history table
		if (@_canWriteToHistory = 1)
		begin
			if(exists (select EntTypeID from dbo.EntPropertyRevisionMap where EntTypeID = @EntTypeID AND PropID = @PropID))
			begin

				insert into dbo.EntPropertyRevisionXRef
					(EntID, EntTypeID, PropID, PropValue, CreatedBy, CreatedDate)
				values
					(@EntID, @EntTypeID, @PropID, @PropValue, @ModifiedBy, @ModifiedDate)

			end
		end

		
	end try
	begin catch

		;throw;

	end catch
end
