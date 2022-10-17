CREATE TABLE [dbo].[UserMessageStore]
(
	[UserMessageId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Message] NVARCHAR(255) NOT NULL, 
    [Timestamp] DATETIME2 NOT NULL
)
