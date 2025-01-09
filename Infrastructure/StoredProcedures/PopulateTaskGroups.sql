CREATE PROCEDURE [dbo].[PopulateTaskGroups]
AS
BEGIN
	DECLARE @User VARCHAR(100);

    select @User = Id FROM AspNetUsers where UserName = 'admin'

    IF NOT EXISTS (
        SELECT TOP 1 *
        FROM TaskGroups
        WHERE Name = 'Recurring'
    )
    BEGIN
        INSERT INTO TaskGroups(Name, [Description], Color, Created, Modified, ModifiedBy, CreatedBy)
        VALUES('Recurring', 'Tasks that i have to do every day', 1, GETUTCDATE(), NULL, NULL, @User)
    END

    IF NOT EXISTS (
        SELECT TOP 1 *
        FROM TaskGroups
        WHERE Name = 'Weekend'
    )
    BEGIN
        INSERT INTO TaskGroups(Name, [Description], Color, Created, Modified, ModifiedBy, CreatedBy)
        VALUES('Weekend', 'Tasks only for weekends', 2, GETUTCDATE(), NULL, NULL, @User)
    END
END;