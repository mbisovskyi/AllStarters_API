DECLARE @RoleName AS VARCHAR(20)

SET @RoleName = 'Super'
IF NOT EXISTS(SELECT 1 FROM dbo.ValidUserRoles WHERE RoleName = @RoleName)
	BEGIN
		INSERT INTO dbo.ValidUserRoles (RoleName)
		VALUES (@RoleName)
	END

SET @RoleName = 'Admin'
IF NOT EXISTS(SELECT 1 FROM dbo.ValidUserRoles WHERE RoleName = @RoleName)
	BEGIN
		INSERT INTO dbo.ValidUserRoles (RoleName)
		VALUES (@RoleName)
	END

SET @RoleName = 'User'
IF NOT EXISTS(SELECT 1 FROM dbo.ValidUserRoles WHERE RoleName = @RoleName)
	BEGIN
		INSERT INTO dbo.ValidUserRoles (RoleName)
		VALUES (@RoleName)
	END
