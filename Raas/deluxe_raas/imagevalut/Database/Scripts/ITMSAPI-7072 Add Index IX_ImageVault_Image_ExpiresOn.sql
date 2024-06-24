USE [ImageVault]

IF NOT EXISTS (
		SELECT *
		FROM sys.indexes i
		INNER JOIN sys.tables t ON i.object_id = t.object_id
		INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
		WHERE i.name = 'IX_ImageVault_Image_ExpiresOn'
			AND t.name = 'Image'
			AND s.name = 'ImageVault'
		)
	CREATE INDEX IX_ImageVault_Image_ExpiresOn ON [ImageVault].[Image] (ExpiresOn) Include (ImageID) 
	WITH (ONLINE=ON)
GO