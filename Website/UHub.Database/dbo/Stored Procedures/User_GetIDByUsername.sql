create proc User_GetIDByUsername

	@Username nvarchar(100)

as
begin

	select
		entID
	from dbo.Users
	where
		Username = @Username
	


end