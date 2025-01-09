CREATE PROCEDURE [dbo].[PopulateTasks]
AS
BEGIN
    DECLARE @User VARCHAR(100);

    select @User = Id FROM AspNetUsers where UserName = 'admin'

    IF NOT EXISTS (
        SELECT TOP 1 * 
        FROM Tasks 
        WHERE [Name] = 'Workout'
    )
    BEGIN
        INSERT INTO Tasks(Name, [Description], StartDate, EndDate, Status, Created, Modified, ModifiedBy, CreatedBy, TaskGroupId)
        VALUES('Workout', 'Go to the GYM at 8:00 AM', GETUTCDATE(), GETUTCDATE() + 1, 1, GETUTCDATE(), NULL, NULL, @User, NULL)
    END

    IF NOT EXISTS (
        SELECT TOP 1 * 
        FROM Tasks 
        WHERE [Name] = 'Dry cleaner'
    )
    BEGIN
        INSERT INTO Tasks(Name, [Description], StartDate, EndDate, Status, Created, Modified, ModifiedBy, CreatedBy, TaskGroupId)
        VALUES('Dry cleaner', 'Pick up clothes from dry cleaner', GETUTCDATE(), GETUTCDATE() + 1, 1, GETUTCDATE(), NULL, NULL, @User, NULL)
    END

    IF NOT EXISTS (
        SELECT TOP 1 * 
        FROM Tasks 
        WHERE [Name] = 'Laptop Service'
    )
    BEGIN
        INSERT INTO Tasks(Name, [Description], StartDate, EndDate, Status, Created, Modified, ModifiedBy, CreatedBy, TaskGroupId)
        VALUES('Laptop Service', 'Call the IT service solutions for my laptop at 6:30 PM', GETUTCDATE(), GETUTCDATE() + 1, 1, GETUTCDATE(), NULL, NULL, @User, NULL)
    END
END