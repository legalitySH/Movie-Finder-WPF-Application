USE [master]
GO
/****** Object:  Database [moviefinder_db]    Script Date: 21.05.2024 02:46:51 ******/
CREATE DATABASE [moviefinder_db]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'moviefinder_db', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\moviefinder_db.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'moviefinder_db_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\moviefinder_db_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [moviefinder_db] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [moviefinder_db].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [moviefinder_db] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [moviefinder_db] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [moviefinder_db] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [moviefinder_db] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [moviefinder_db] SET ARITHABORT OFF 
GO
ALTER DATABASE [moviefinder_db] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [moviefinder_db] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [moviefinder_db] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [moviefinder_db] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [moviefinder_db] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [moviefinder_db] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [moviefinder_db] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [moviefinder_db] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [moviefinder_db] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [moviefinder_db] SET  ENABLE_BROKER 
GO
ALTER DATABASE [moviefinder_db] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [moviefinder_db] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [moviefinder_db] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [moviefinder_db] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [moviefinder_db] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [moviefinder_db] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [moviefinder_db] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [moviefinder_db] SET RECOVERY FULL 
GO
ALTER DATABASE [moviefinder_db] SET  MULTI_USER 
GO
ALTER DATABASE [moviefinder_db] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [moviefinder_db] SET DB_CHAINING OFF 
GO
ALTER DATABASE [moviefinder_db] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [moviefinder_db] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [moviefinder_db] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [moviefinder_db] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'moviefinder_db', N'ON'
GO
ALTER DATABASE [moviefinder_db] SET QUERY_STORE = ON
GO
ALTER DATABASE [moviefinder_db] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [moviefinder_db]
GO
/****** Object:  UserDefinedFunction [dbo].[GetProductionIdFromTable]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetProductionIdFromTable] (@type VARCHAR(50), @production_id INT)
RETURNS INT
AS
BEGIN
    DECLARE @result INT;
    
    IF @type = 'movie'
        SET @result = (SELECT id FROM movies_table WHERE id = @production_id);
    ELSE IF @type = 'serial'
        SET @result = (SELECT id FROM serials_table WHERE id = @production_id);
    
    RETURN @result;
END;
GO
/****** Object:  Table [dbo].[black_list]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[black_list](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[favourites]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[favourites](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [varchar](50) NULL,
	[production_id] [int] NULL,
	[user_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[history]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[history](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [varchar](50) NULL,
	[production_id] [int] NULL,
	[user_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[movies_table]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[movies_table](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[russian_name] [varchar](150) NULL,
	[original_name] [varchar](150) NOT NULL,
	[image_url] [varchar](800) NULL,
	[rating] [float] NULL,
	[year] [int] NULL,
	[duration] [int] NULL,
	[description] [varchar](1500) NULL,
	[short_description] [varchar](750) NULL,
	[age_limit] [int] NULL,
	[countries] [varchar](200) NULL,
	[genres] [varchar](200) NULL,
	[director] [varchar](200) NULL,
	[type] [varchar](50) NULL,
	[votes] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[reviews]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[reviews](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [varchar](50) NULL,
	[production_id] [int] NULL,
	[user_id] [int] NULL,
	[review_text] [varchar](1600) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[serials_table]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[serials_table](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](255) NULL,
	[rating] [float] NULL,
	[year] [int] NULL,
	[image_url] [varchar](1000) NULL,
	[age_limit] [int] NULL,
	[short_description] [varchar](2000) NULL,
	[description] [varchar](1600) NULL,
	[director] [varchar](80) NULL,
	[genres] [varchar](80) NULL,
	[countries] [varchar](80) NULL,
	[type] [varchar](50) NULL,
	[votes] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[users]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[users](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[login] [varchar](255) NOT NULL,
	[email] [varchar](255) NOT NULL,
	[password] [varchar](512) NOT NULL,
	[role] [varchar](20) NOT NULL,
	[registration_date] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[votes]    Script Date: 21.05.2024 02:46:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[votes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type] [varchar](50) NULL,
	[production_id] [int] NULL,
	[user_id] [int] NULL,
	[rating_value] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[movies_table] ADD  DEFAULT ('movie') FOR [type]
GO
ALTER TABLE [dbo].[serials_table] ADD  DEFAULT ('serial') FOR [type]
GO
ALTER TABLE [dbo].[serials_table] ADD  DEFAULT ((0)) FOR [votes]
GO
ALTER TABLE [dbo].[users] ADD  DEFAULT (getdate()) FOR [registration_date]
GO
ALTER TABLE [dbo].[black_list]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[favourites]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[history]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[reviews]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[votes]  WITH CHECK ADD FOREIGN KEY([user_id])
REFERENCES [dbo].[users] ([id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[users]  WITH CHECK ADD CHECK  (([role]='user' OR [role]='admin'))
GO
USE [master]
GO
ALTER DATABASE [moviefinder_db] SET  READ_WRITE 
GO
