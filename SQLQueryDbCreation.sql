USE [CrewRedDb]
GO

/****** Object:  Table [dbo].[TripData]    Script Date: 3/14/2026 10:23:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TripData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PickupDatetime] [datetime2](7) NULL,
	[DropoffDatetime] [datetime2](7) NULL,
	[PassengerCount] [tinyint] NULL,
	[TripDistance] [float] NULL,
	[StoreAndFwdFlag] [nvarchar](3) NULL,
	[PULocationId] [smallint] NULL,
	[DOLocationId] [smallint] NULL,
	[FareAmount] [decimal](10, 2) NULL,
	[TipAmount] [decimal](10, 2) NULL,
	[travel_time_seconds]  AS (case when [PickupDatetime] IS NULL OR [DropoffDatetime] IS NULL then NULL else datediff(second,[PickupDatetime],[DropoffDatetime]) end),
 CONSTRAINT [PK_TripData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


