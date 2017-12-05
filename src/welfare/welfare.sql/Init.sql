USE [Unemployment]
GO
/****** Object:  Table [dbo].[HandicapType]    Script Date: 5/12/2017 12:08:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HandicapType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HandicapPct] [int] NOT NULL,
 CONSTRAINT [PK_HandicapType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MaritalStatus]    Script Date: 5/12/2017 12:08:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaritalStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
 CONSTRAINT [PK_MaritalStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WelfareLogic]    Script Date: 5/12/2017 12:08:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WelfareLogic](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Month] [int] NOT NULL,
	[IdMaritalStatus] [int] NOT NULL,
	[idHandicap] [int] NOT NULL,
	[NumberOfChildren] [int] NOT NULL,
	[BrutoStart] [int] NOT NULL,
	[BrutoEnd] [int] NOT NULL,
	[Age] [int] NOT NULL,
	[Value] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_WelfareLogic] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[HandicapType] ON 
GO
INSERT [dbo].[HandicapType] ([Id], [HandicapPct]) VALUES (1, 0)
GO
INSERT [dbo].[HandicapType] ([Id], [HandicapPct]) VALUES (2, 10)
GO
INSERT [dbo].[HandicapType] ([Id], [HandicapPct]) VALUES (3, 20)
GO
INSERT [dbo].[HandicapType] ([Id], [HandicapPct]) VALUES (4, 30)
GO
INSERT [dbo].[HandicapType] ([Id], [HandicapPct]) VALUES (5, 40)
GO
INSERT [dbo].[HandicapType] ([Id], [HandicapPct]) VALUES (6, 50)
GO
SET IDENTITY_INSERT [dbo].[HandicapType] OFF
GO
SET IDENTITY_INSERT [dbo].[MaritalStatus] ON 
GO
INSERT [dbo].[MaritalStatus] ([Id], [Name]) VALUES (1, N'Single')
GO
INSERT [dbo].[MaritalStatus] ([Id], [Name]) VALUES (2, N'Married')
GO
INSERT [dbo].[MaritalStatus] ([Id], [Name]) VALUES (3, N'Married_Head_Of_Family')
GO
SET IDENTITY_INSERT [dbo].[MaritalStatus] OFF
GO
SET IDENTITY_INSERT [dbo].[WelfareLogic] ON 
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (1, 0, 1, 1, 0, 0, 1000, 18, CAST(953.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (2, 0, 1, 1, 0, 1000, 2000, 18, CAST(1300.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (3, 4, 1, 1, 0, 1000, 2000, 18, CAST(1200.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (4, 12, 1, 1, 0, 1000, 2000, 18, CAST(1100.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (5, 0, 2, 1, 0, 0, 1000, 18, CAST(715.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (6, 0, 2, 1, 0, 1000, 2000, 18, CAST(1300.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (7, 4, 2, 1, 0, 1000, 2000, 18, CAST(1200.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (8, 12, 2, 1, 0, 1000, 2000, 18, CAST(800.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (9, 0, 3, 1, 1, 0, 1000, 18, CAST(1135.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (10, 0, 3, 1, 1, 1000, 2000, 18, CAST(1300.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[WelfareLogic] ([Id], [Month], [IdMaritalStatus], [idHandicap], [NumberOfChildren], [BrutoStart], [BrutoEnd], [Age], [Value]) VALUES (11, 4, 3, 1, 1, 1000, 2000, 18, CAST(1200.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[WelfareLogic] OFF
GO
ALTER TABLE [dbo].[WelfareLogic]  WITH CHECK ADD  CONSTRAINT [FK_WelfareLogic_HandicapType] FOREIGN KEY([idHandicap])
REFERENCES [dbo].[HandicapType] ([Id])
GO
ALTER TABLE [dbo].[WelfareLogic] CHECK CONSTRAINT [FK_WelfareLogic_HandicapType]
GO
ALTER TABLE [dbo].[WelfareLogic]  WITH CHECK ADD  CONSTRAINT [FK_WelfareLogic_MaritalStatus] FOREIGN KEY([IdMaritalStatus])
REFERENCES [dbo].[MaritalStatus] ([Id])
GO
ALTER TABLE [dbo].[WelfareLogic] CHECK CONSTRAINT [FK_WelfareLogic_MaritalStatus]
GO
