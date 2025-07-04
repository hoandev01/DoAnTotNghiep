USE [master]
GO
/****** Object:  Database [Testflock2]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE DATABASE [Testflock2]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Testflock2', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SSAS\MSSQL\DATA\Testflock2.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Testflock2_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SSAS\MSSQL\DATA\Testflock2_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [Testflock2] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Testflock2].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Testflock2] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Testflock2] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Testflock2] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Testflock2] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Testflock2] SET ARITHABORT OFF 
GO
ALTER DATABASE [Testflock2] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Testflock2] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Testflock2] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Testflock2] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Testflock2] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Testflock2] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Testflock2] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Testflock2] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Testflock2] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Testflock2] SET  ENABLE_BROKER 
GO
ALTER DATABASE [Testflock2] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Testflock2] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Testflock2] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Testflock2] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Testflock2] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Testflock2] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [Testflock2] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Testflock2] SET RECOVERY FULL 
GO
ALTER DATABASE [Testflock2] SET  MULTI_USER 
GO
ALTER DATABASE [Testflock2] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Testflock2] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Testflock2] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Testflock2] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Testflock2] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Testflock2] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'Testflock2', N'ON'
GO
ALTER DATABASE [Testflock2] SET QUERY_STORE = ON
GO
ALTER DATABASE [Testflock2] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [Testflock2]
GO
/****** Object:  User [Hoan]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE USER [Hoan] FOR LOGIN [Hoan] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cages]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CageName] [nvarchar](100) NOT NULL,
	[CageType] [nvarchar](max) NOT NULL,
	[CageCapacity] [int] NOT NULL,
	[CageArea] [nvarchar](100) NULL,
 CONSTRAINT [PK_Cages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CartItems]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[CartItemQuantity] [int] NOT NULL,
 CONSTRAINT [PK_CartItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[BroodingDays] [int] NOT NULL,
	[GrowthDays] [int] NOT NULL,
	[PreSaleDays] [int] NOT NULL,
	[ReadyDays] [int] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Flocks]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Flocks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FlockName] [nvarchar](100) NOT NULL,
	[CageId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[FlockQuantity] [int] NOT NULL,
	[FlockNote] [nvarchar](max) NULL,
	[ChickenSize] [nvarchar](max) NOT NULL,
	[FeedType] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[GrowthLevel] [nvarchar](max) NULL,
	[DayIn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Flocks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FlockStages]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FlockStages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FlockId] [int] NOT NULL,
	[StageName] [nvarchar](100) NOT NULL,
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NULL,
	[Note] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_FlockStages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NewsArticles]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewsArticles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ImageNews] [nvarchar](max) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Summary] [nvarchar](max) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
	[PublishedDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_NewsArticles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[OrderDetailQuantity] [int] NOT NULL,
	[OrderDetailPrice] [int] NOT NULL,
 CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[OrderDate] [datetime2](7) NOT NULL,
	[TotalAmount] [int] NOT NULL,
	[PaymentMethod] [nvarchar](max) NOT NULL,
	[Status] [nvarchar](max) NOT NULL,
	[CancelReason] [nvarchar](max) NULL,
	[IsReviewed] [bit] NULL,
	[CancelledAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FlockId] [int] NOT NULL,
	[ProductName] [nvarchar](100) NOT NULL,
	[Image] [nvarchar](max) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[ProductStock] [int] NOT NULL,
	[DateCreated] [datetime2](7) NULL,
	[ProductDescription] [nvarchar](max) NULL,
	[OutOfStockAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Slides]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Slides](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Subtitle] [nvarchar](max) NOT NULL,
	[ImageUrl] [nvarchar](max) NOT NULL,
	[ButtonText] [nvarchar](max) NOT NULL,
	[ButtonLink] [nvarchar](max) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Slides] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Trackings]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Trackings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FlockId] [int] NOT NULL,
	[TrackingDate] [datetime2](7) NOT NULL,
	[HealthStatus] [nvarchar](max) NULL,
	[Temperature] [real] NULL,
	[Humidity] [real] NULL,
	[Note] [nvarchar](max) NULL,
	[FeedCost] [int] NOT NULL,
 CONSTRAINT [PK_Trackings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 30/06/2025 5:11:26 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](50) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[UserType] [nvarchar](8) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Index [IX_CartItems_ProductId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_CartItems_ProductId] ON [dbo].[CartItems]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CartItems_UserId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_CartItems_UserId] ON [dbo].[CartItems]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Flocks_CageId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_Flocks_CageId] ON [dbo].[Flocks]
(
	[CageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Flocks_CategoryId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_Flocks_CategoryId] ON [dbo].[Flocks]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_FlockStages_FlockId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_FlockStages_FlockId] ON [dbo].[FlockStages]
(
	[FlockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDetails_OrderId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_OrderDetails_OrderId] ON [dbo].[OrderDetails]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDetails_ProductId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_OrderDetails_ProductId] ON [dbo].[OrderDetails]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_UserId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_Orders_UserId] ON [dbo].[Orders]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_FlockId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_Products_FlockId] ON [dbo].[Products]
(
	[FlockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Trackings_FlockId]    Script Date: 30/06/2025 5:11:26 CH ******/
CREATE NONCLUSTERED INDEX [IX_Trackings_FlockId] ON [dbo].[Trackings]
(
	[FlockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((0)) FOR [BroodingDays]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((0)) FOR [GrowthDays]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((0)) FOR [PreSaleDays]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((0)) FOR [ReadyDays]
GO
ALTER TABLE [dbo].[Flocks] ADD  DEFAULT ('0001-01-01T00:00:00.0000000') FOR [DayIn]
GO
ALTER TABLE [dbo].[Slides] ADD  DEFAULT (N'') FOR [ButtonLink]
GO
ALTER TABLE [dbo].[Slides] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[Slides] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Trackings] ADD  DEFAULT ((0)) FOR [FeedCost]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (N'') FOR [UserType]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK_CartItems_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Products_ProductId]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD  CONSTRAINT [FK_CartItems_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CartItems] CHECK CONSTRAINT [FK_CartItems_Users_UserId]
GO
ALTER TABLE [dbo].[Flocks]  WITH CHECK ADD  CONSTRAINT [FK_Flocks_Cages_CageId] FOREIGN KEY([CageId])
REFERENCES [dbo].[Cages] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Flocks] CHECK CONSTRAINT [FK_Flocks_Cages_CageId]
GO
ALTER TABLE [dbo].[Flocks]  WITH CHECK ADD  CONSTRAINT [FK_Flocks_Categories_CategoryId] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Flocks] CHECK CONSTRAINT [FK_Flocks_Categories_CategoryId]
GO
ALTER TABLE [dbo].[FlockStages]  WITH CHECK ADD  CONSTRAINT [FK_FlockStages_Flocks_FlockId] FOREIGN KEY([FlockId])
REFERENCES [dbo].[Flocks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FlockStages] CHECK CONSTRAINT [FK_FlockStages_Flocks_FlockId]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Orders_OrderId]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetails_Products_ProductId] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails] CHECK CONSTRAINT [FK_OrderDetails_Products_ProductId]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_Users_UserId]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Flocks_FlockId] FOREIGN KEY([FlockId])
REFERENCES [dbo].[Flocks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Flocks_FlockId]
GO
ALTER TABLE [dbo].[Trackings]  WITH CHECK ADD  CONSTRAINT [FK_Trackings_Flocks_FlockId] FOREIGN KEY([FlockId])
REFERENCES [dbo].[Flocks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Trackings] CHECK CONSTRAINT [FK_Trackings_Flocks_FlockId]
GO
USE [master]
GO
ALTER DATABASE [Testflock2] SET  READ_WRITE 
GO
