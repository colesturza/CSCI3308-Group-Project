
CREATE proc [dbo].[User_UpdateConfirmFlag]

	@RefUID nvarchar(100)

as
begin

	if(exists(select * from dbo.Users where RefUID = @RefUID))
	begin
		update dbo.Users
		set
			IsConfirmed = 1
		where
			RefUID = @RefUID

		select cast(1 as bit)
	end
	else begin
		select cast(0 as bit)
	end


end
