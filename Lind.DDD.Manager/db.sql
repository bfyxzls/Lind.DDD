USE [Test_Code_BG]
GO
/****** Object:  Table [dbo].[WebManageUsers]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebManageUsers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LastModifyTime] [datetime] NOT NULL,
	[LastModifyUserId] [int] NOT NULL,
	[DataStatus] [int] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[LoginName] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[RealName] [nvarchar](max) NOT NULL,
	[Mobile] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Operator] [nvarchar](max) NULL,
	[WebSystemID] [int] NULL,
	[DataCreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.WebManageUsers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebDepartments]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebDepartments](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[About] [nvarchar](max) NULL,
	[SortNumber] [int] NOT NULL,
	[Operator] [nvarchar](max) NULL,
	[Level] [int] NOT NULL,
	[ParentID] [int] NULL,
	[IsDeleted] [bit] NOT NULL,
	[LastModifyTime] [datetime] NOT NULL,
	[LastModifyUserId] [int] NOT NULL,
	[DataStatus] [int] NOT NULL,
	[DataCreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.WebDepartments] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebDataCtrls]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebDataCtrls](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DataCtrlName] [nvarchar](max) NOT NULL,
	[DataCtrlType] [nvarchar](max) NOT NULL,
	[DataCtrlField] [nvarchar](max) NOT NULL,
	[DataCtrlApi] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DataCreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.WebDataCtrls] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebManageMenus]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebManageMenus](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[LastModifyTime] [datetime] NOT NULL,
	[LastModifyUserId] [int] NOT NULL,
	[DataStatus] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Level] [int] NOT NULL,
	[LinkUrl] [nvarchar](max) NULL,
	[About] [nvarchar](max) NULL,
	[SortNumber] [int] NOT NULL,
	[Operator] [nvarchar](max) NULL,
	[IsDisplayMenuTree] [bit] NOT NULL,
	[ParentID] [int] NULL,
	[DataCreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.WebManageMenus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebDepartments_WebManageUsers_R]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebDepartments_WebManageUsers_R](
	[DeptId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.WebDepartments_WebManageUsers_R] PRIMARY KEY CLUSTERED 
(
	[DeptId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebDataSettings]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebDataSettings](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[WebDataCtrlId] [int] NOT NULL,
	[DepartmentId] [int] NOT NULL,
	[ObjectIdArr] [nvarchar](max) NOT NULL,
	[DataCreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.WebDataSettings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebManageRoles]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebManageRoles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[LastModifyTime] [datetime] NOT NULL,
	[LastModifyUserId] [int] NOT NULL,
	[DataStatus] [int] NOT NULL,
	[RoleName] [nvarchar](max) NOT NULL,
	[About] [nvarchar](max) NULL,
	[SortNumber] [int] NOT NULL,
	[Operator] [nvarchar](max) NULL,
	[OperatorAuthority] [int] NOT NULL,
	[DepartmentID] [int] NOT NULL,
	[DataCreateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.WebManageRoles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebManageRoles_WebManageUsers_R]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebManageRoles_WebManageUsers_R](
	[RoleId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.WebManageRoles_WebManageUsers_R] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebManageRoles_WebManageMenus_R]    Script Date: 06/24/2016 10:01:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebManageRoles_WebManageMenus_R](
	[RoleId] [int] NOT NULL,
	[MenuId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.WebManageRoles_WebManageMenus_R] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[MenuId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_dbo.WebDataSettings_dbo.WebDataCtrls_WebDataCtrlId]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebDataSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebDataSettings_dbo.WebDataCtrls_WebDataCtrlId] FOREIGN KEY([WebDataCtrlId])
REFERENCES [dbo].[WebDataCtrls] ([ID])
GO
ALTER TABLE [dbo].[WebDataSettings] CHECK CONSTRAINT [FK_dbo.WebDataSettings_dbo.WebDataCtrls_WebDataCtrlId]
GO
/****** Object:  ForeignKey [FK_dbo.WebDataSettings_dbo.WebDepartments_DepartmentId]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebDataSettings]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebDataSettings_dbo.WebDepartments_DepartmentId] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[WebDepartments] ([ID])
GO
ALTER TABLE [dbo].[WebDataSettings] CHECK CONSTRAINT [FK_dbo.WebDataSettings_dbo.WebDepartments_DepartmentId]
GO
/****** Object:  ForeignKey [FK_dbo.WebDepartments_dbo.WebDepartments_ParentID]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebDepartments]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebDepartments_dbo.WebDepartments_ParentID] FOREIGN KEY([ParentID])
REFERENCES [dbo].[WebDepartments] ([ID])
GO
ALTER TABLE [dbo].[WebDepartments] CHECK CONSTRAINT [FK_dbo.WebDepartments_dbo.WebDepartments_ParentID]
GO
/****** Object:  ForeignKey [FK_dbo.WebDepartments_WebManageUsers_R_dbo.WebDepartments_DeptId]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebDepartments_WebManageUsers_R]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebDepartments_WebManageUsers_R_dbo.WebDepartments_DeptId] FOREIGN KEY([DeptId])
REFERENCES [dbo].[WebDepartments] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebDepartments_WebManageUsers_R] CHECK CONSTRAINT [FK_dbo.WebDepartments_WebManageUsers_R_dbo.WebDepartments_DeptId]
GO
/****** Object:  ForeignKey [FK_dbo.WebDepartments_WebManageUsers_R_dbo.WebManageUsers_UserId]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebDepartments_WebManageUsers_R]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebDepartments_WebManageUsers_R_dbo.WebManageUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[WebManageUsers] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebDepartments_WebManageUsers_R] CHECK CONSTRAINT [FK_dbo.WebDepartments_WebManageUsers_R_dbo.WebManageUsers_UserId]
GO
/****** Object:  ForeignKey [FK_dbo.WebManageMenus_dbo.WebManageMenus_ParentID]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebManageMenus]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebManageMenus_dbo.WebManageMenus_ParentID] FOREIGN KEY([ParentID])
REFERENCES [dbo].[WebManageMenus] ([ID])
GO
ALTER TABLE [dbo].[WebManageMenus] CHECK CONSTRAINT [FK_dbo.WebManageMenus_dbo.WebManageMenus_ParentID]
GO
/****** Object:  ForeignKey [FK_dbo.WebManageRoles_dbo.WebDepartments_DepartmentID]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebManageRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebManageRoles_dbo.WebDepartments_DepartmentID] FOREIGN KEY([DepartmentID])
REFERENCES [dbo].[WebDepartments] ([ID])
GO
ALTER TABLE [dbo].[WebManageRoles] CHECK CONSTRAINT [FK_dbo.WebManageRoles_dbo.WebDepartments_DepartmentID]
GO
/****** Object:  ForeignKey [FK_dbo.WebManageRoles_WebManageMenus_R_dbo.WebManageMenus_MenuId]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebManageRoles_WebManageMenus_R]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebManageRoles_WebManageMenus_R_dbo.WebManageMenus_MenuId] FOREIGN KEY([MenuId])
REFERENCES [dbo].[WebManageMenus] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebManageRoles_WebManageMenus_R] CHECK CONSTRAINT [FK_dbo.WebManageRoles_WebManageMenus_R_dbo.WebManageMenus_MenuId]
GO
/****** Object:  ForeignKey [FK_dbo.WebManageRoles_WebManageMenus_R_dbo.WebManageRoles_RoleId]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebManageRoles_WebManageMenus_R]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebManageRoles_WebManageMenus_R_dbo.WebManageRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[WebManageRoles] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebManageRoles_WebManageMenus_R] CHECK CONSTRAINT [FK_dbo.WebManageRoles_WebManageMenus_R_dbo.WebManageRoles_RoleId]
GO
/****** Object:  ForeignKey [FK_dbo.WebManageRoles_WebManageUsers_R_dbo.WebManageRoles_RoleId]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebManageRoles_WebManageUsers_R]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebManageRoles_WebManageUsers_R_dbo.WebManageRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[WebManageRoles] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebManageRoles_WebManageUsers_R] CHECK CONSTRAINT [FK_dbo.WebManageRoles_WebManageUsers_R_dbo.WebManageRoles_RoleId]
GO
/****** Object:  ForeignKey [FK_dbo.WebManageRoles_WebManageUsers_R_dbo.WebManageUsers_UserId]    Script Date: 06/24/2016 10:01:24 ******/
ALTER TABLE [dbo].[WebManageRoles_WebManageUsers_R]  WITH CHECK ADD  CONSTRAINT [FK_dbo.WebManageRoles_WebManageUsers_R_dbo.WebManageUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[WebManageUsers] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WebManageRoles_WebManageUsers_R] CHECK CONSTRAINT [FK_dbo.WebManageRoles_WebManageUsers_R_dbo.WebManageUsers_UserId]
GO
