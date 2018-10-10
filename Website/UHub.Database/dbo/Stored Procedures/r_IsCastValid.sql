
CREATE proc [dbo].[r_IsCastValid]

	@Value nvarchar(max),
	@Type nvarchar(100)

as
begin

	--validate datatype to prevent injection attacks
	if(@Type in (select [Name] from dbo.DataTypes))
	begin
		begin try
			--try to cast @Value into variable
			--if the cast is successful, return True
			--if the cast throws any errors, return False
			--perform extra end check to validate output against input - forces LENGTH validation for types like nvarchar(N)
			----this check will not prevent decimal rounding
			----if rounding validation is needed, cast @output to nvarchar
			declare @sqlCmd nvarchar(max)
			set @sqlCmd = N'declare @output '+ @Type + '; set @output = cast(@val as ' + @Type + '); if(@output <> @val) begin ;throw 51000, ''badOutput'', 1; end'


			EXEC sp_executesql @sqlCmd, 
				N'@val nvarchar(max)', 
				@val=@Value

			return cast(1 as bit)
		end try
		begin catch
			return cast(0 as bit)
		end catch

	end
	else begin
		return cast(0 as bit)
	end

end
