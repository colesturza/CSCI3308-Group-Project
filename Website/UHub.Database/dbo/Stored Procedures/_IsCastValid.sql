
CREATE proc [dbo].[_IsCastValid]

	@Value nvarchar(max),
	@Type nvarchar(100)

as
begin

	--validate datatype to prevent injection attacks
	if(@Type in (select [Name] from dbo.DataTypes))
	begin

		--NULL value will pass conversion to any data type
		--No need to run complicated eval code
		if(@Value is NULL)
		begin
			return 1;
		end


		begin try
			--try to cast @Value into variable
			--if the cast is successful, return True
			--if the cast throws any errors, return False
			--perform extra end check to validate output against input - forces LENGTH validation for types like nvarchar(N)
			----this check will not prevent decimal rounding
			----if rounding validation is needed, cast @output to nvarchar
			declare @sqlCmd nvarchar(max)
			if(@Type like '%char%' or @Type like '%text%')
			begin
				--ASIDE
				--Append x to string values to acount for end-of-line whitespace truncation
				--This allows for proper string length comparisons
				--Without this, then '  ' == ''
				--
				--set @output = @output + ''x''
				--set @val = @val + ''x''

				set @sqlCmd = N'
				declare @output nvarchar(MAX)
				set @output = TRY_CONVERT(' + @Type + ', @val)
				if(@output is null)
				begin
					set @retValOUT = cast(0 as bit)
					return;
				end

				set @output = @output + ''x''
				set @val = @val + ''x''

				if(LEN(@output) <> LEN(@val))
				begin 
					set @retValOUT = cast(0 as bit)
				end
				else begin
					set @retValOUT = cast(1 as bit)
				end'
			end
			else begin
				set @sqlCmd = 
				N'declare @output '+ @Type + '
				set @output = TRY_CONVERT(' + @Type + ', @val)
				if(@output is null)
				begin
					set @retValOUT = cast(0 as bit)
				end
				else begin
					set @retValOUT = cast(1 as bit)
				end'
			end
			

			
			DECLARE @retVal bit
			EXEC sp_executesql
				@sqlCmd,
				N'@val nvarchar(max), @retValOUT bit OUTPUT',
				@retValOUT = @retVal OUTPUT,
				@val = @Value


			return @retVal
		end try
		begin catch
			return cast(0 as bit)
		end catch

	end
	else begin
		return cast(0 as bit)
	end

end