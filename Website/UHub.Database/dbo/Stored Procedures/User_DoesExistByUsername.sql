create proc [dbo].[User_DoesExistByUsername]
	@Username nvarchar(100)


as
begin

	if exists(select * from dbo.Users where Username = @Username)
	begin
		select cast(1 as bit)
	end
	else begin
		select cast(0 as bit)
	end



end