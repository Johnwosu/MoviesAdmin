CREATE TABLE [AudienceRatings] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(15) NOT NULL,
    CONSTRAINT [PK_AudienceRatings] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [Genres] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(60) NOT NULL,
    CONSTRAINT [PK_Genres] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [UserSnapshot] (
    [UserId] nvarchar(450) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_UserSnapshot] PRIMARY KEY ([UserId])
);
GO


CREATE TABLE [Movies] (
    [Id] int NOT NULL IDENTITY,
    [GenreId] int NOT NULL,
    [AudienceRatingId] int NOT NULL,
    [Title] nvarchar(150) NOT NULL,
    [Synopsis] nvarchar(2000) NOT NULL,
    [RuntimeMinutes] int NOT NULL,
    [TheaterReleaseDate] datetime2 NOT NULL,
    [ImageUrl] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Movies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Movies_AudienceRatings_AudienceRatingId] FOREIGN KEY ([AudienceRatingId]) REFERENCES [AudienceRatings] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Movies_Genres_GenreId] FOREIGN KEY ([GenreId]) REFERENCES [Genres] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [CriticReviews] (
    [Id] int NOT NULL IDENTITY,
    [MovieId] int NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Rating] int NOT NULL,
    [IsPublished] bit NOT NULL,
    [CreatedBy] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_CriticReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CriticReviews_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]) ON DELETE CASCADE
);
GO


CREATE INDEX [IX_CriticReviews_MovieId] ON [CriticReviews] ([MovieId]);
GO


CREATE INDEX [IX_Movies_AudienceRatingId] ON [Movies] ([AudienceRatingId]);
GO


CREATE INDEX [IX_Movies_GenreId] ON [Movies] ([GenreId]);
GO


