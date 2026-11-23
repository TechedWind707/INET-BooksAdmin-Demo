CREATE TABLE [AudienceCategory] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_AudienceCategory] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Genre] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Genre] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [UserSnapshot] (
    [UserId] nvarchar(450) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_UserSnapshot] PRIMARY KEY ([UserId])
);
GO


CREATE TABLE [Book] (
    [Id] int NOT NULL IDENTITY,
    [GenreId] int NOT NULL,
    [AudienceCategoryId] int NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Author] nvarchar(max) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [YearPublished] int NOT NULL,
    [UploadedCoverUrl] nvarchar(max) NULL,
    [Isbn] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Book] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Book_AudienceCategory_AudienceCategoryId] FOREIGN KEY ([AudienceCategoryId]) REFERENCES [AudienceCategory] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Book_Genre_GenreId] FOREIGN KEY ([GenreId]) REFERENCES [Genre] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [BookReview] (
    [Id] int NOT NULL IDENTITY,
    [BookId] int NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Rating] int NOT NULL,
    [IsPublished] bit NOT NULL,
    [CreatedBy] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_BookReview] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BookReview_Book_BookId] FOREIGN KEY ([BookId]) REFERENCES [Book] ([Id]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_Book_AudienceCategoryId] ON [Book] ([AudienceCategoryId]);
GO


CREATE INDEX [IX_Book_GenreId] ON [Book] ([GenreId]);
GO


CREATE INDEX [IX_BookReview_BookId] ON [BookReview] ([BookId]);
GO


