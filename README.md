# RecipeReviews
## Database
```sql
CREATE DATABASE RecipeReviews;
CREATE TABLE [dbo].[Account] (
    [AccountId]        VARCHAR (30)   NOT NULL,
    [Username]         NVARCHAR (30)  NOT NULL,
    [Password]         NVARCHAR (MAX) NOT NULL,
    [Email]            NVARCHAR (256) NOT NULL,
    [Created]          DATETIME       NOT NULL,
    [Description]      VARCHAR (200)  NULL,
    [Picture Filename] NCHAR (255)    NULL,
    [Average Rating]   FLOAT (53)     NULL,
    PRIMARY KEY CLUSTERED ([AccountId] ASC)
);

CREATE TABLE [dbo].[Tag] (
    [TagId] INT        IDENTITY (1, 1) NOT NULL,
    [Name]  NCHAR (30) NOT NULL,
    PRIMARY KEY CLUSTERED ([TagId] ASC)
);

CREATE TABLE [dbo].[Recipe] (
    [RecipeId]       INT          IDENTITY (1, 1) NOT NULL,
    [AccountId]      VARCHAR (30) NOT NULL,
    [Title]          NCHAR (80)   NOT NULL,
    [Text]           TEXT         NOT NULL,
    [Created]        DATETIME     NOT NULL,
    [Rating]         FLOAT (53)   NOT NULL,
    [Image Filename] NCHAR (255)  NULL,
    PRIMARY KEY CLUSTERED ([RecipeId] ASC),
    CONSTRAINT [AccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([AccountId])
);

CREATE TABLE [dbo].[RecipeTag] (
    [RecipeId] INT NOT NULL,
    [TagId]    INT NOT NULL,
    PRIMARY KEY CLUSTERED ([RecipeId] ASC, [TagId] ASC),
    CONSTRAINT [CTagIdRT] FOREIGN KEY ([TagId]) REFERENCES [dbo].[Tag] ([TagId]),
    CONSTRAINT [CRecipeIdRT] FOREIGN KEY ([RecipeId]) REFERENCES [dbo].[Recipe] ([RecipeId])
);

CREATE TABLE [dbo].[RatingHistory] (
    [AccountId] VARCHAR (30) NOT NULL,
    [RecipeId]  INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([AccountId] ASC, [RecipeId] ASC),
    CONSTRAINT [CAccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Account] ([AccountId]) ON DELETE CASCADE,
    CONSTRAINT [CRecipeId] FOREIGN KEY ([RecipeId]) REFERENCES [dbo].[Recipe] ([RecipeId]) ON DELETE CASCADE
);
```
