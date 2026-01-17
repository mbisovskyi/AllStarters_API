IF OBJECT_ID ('ValidUserRoles', 'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[ValidUserRoles]
        (
	        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT LOWER(NEWID()), 
            [RoleName] VARCHAR(20) NOT NULL, 
            [RowAddedDt] DATE NOT NULL DEFAULT GETDATE() 
        )
    END