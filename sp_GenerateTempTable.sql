USE [AdventureWorks2019]
GO
/****** Object:  StoredProcedure [dbo].[GenerateTempTable]    Script Date: 19-03-2024 14:50:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GenerateTempTable]
AS
BEGIN
    SET NOCOUNT ON;

    -- Drop the temp table if it already exists
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'temp_dummy_table')
    BEGIN
        DROP TABLE dbo.temp_dummy_table;
    END

    -- Calculate the total number of rows in the table
    DECLARE @total_rows INT;
    SELECT @total_rows = COUNT(*) FROM dbo.dummy_table;

    -- Calculate the number of partitions needed based on the desired partition size (1 million records)
    DECLARE @partition_size INT = 1000000; -- 1 million records per partition
    DECLARE @ntile_value INT = CEILING(CAST(@total_rows AS DECIMAL) / @partition_size);

    -- Create the temp table with NTILE partitioning using the calculated ntile_value
    SELECT id, NTILE(@ntile_value) OVER (ORDER BY id) AS Part INTO dbo.temp_dummy_table FROM dbo.dummy_table;

    -- Create clustered index on id
    CREATE CLUSTERED INDEX ix_id ON dbo.temp_dummy_table(id);

    -- Create non-clustered index on part
    CREATE NONCLUSTERED INDEX ix_part ON dbo.temp_dummy_table(Part);

	SELECT @ntile_value as Part
END
