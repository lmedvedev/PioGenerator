/****** Object:  Table [dbo].[SysAudit]    Script Date: 12/05/2013 12:18:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysAudit](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Srv] [varchar](50) NOT NULL,
	[Db] [varchar](50) NOT NULL,
	[Tbl] [varchar](50) NOT NULL,
	[Operation] [int] NOT NULL,
	[OpContent] [xml] NULL,
	[ClientHost] [varchar](50) NOT NULL,
	[ClientApplication] [varchar](50) NOT NULL,
	[dt] [datetime] NOT NULL,
	[usr] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SysAudit] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_Trancount]    Script Date: 12/05/2013 12:18:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[sp_Trancount]
AS
	return @@trancount

grant execute on dbo.sp_Trancount to public
GO
/****** Object:  UserDefinedFunction [dbo].[fn_TopCodeSN]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fn_TopCodeSN]
	(
    @fPath varchar(896)
	)
RETURNS varchar(896)
AS
 BEGIN
    if @fPath is null 
        SET @fPath = ''
    else
	BEGIN
	DECLARE @I int
	  SET @I = CHARINDEX(':',@fPath)
	  IF @I > 0 
		SET @fPath = LEFT(@fPath,@I-1)
	END
    RETURN @fPath 
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_TopCodeS]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fn_TopCodeS]
	(
    @fPath varchar(100)
	)
RETURNS varchar(100)
AS
 BEGIN
    if @fPath is null 
        SET @fPath = ''
    else
	BEGIN
	DECLARE @I int
	  SET @I = CHARINDEX('.',@fPath)
	  IF @I > 0 
		SET @fPath = LEFT(@fPath,@I-1)
	END
    RETURN @fPath 
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_TimeCut]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_TimeCut]
	(
	@dt smalldatetime
	)
RETURNS smalldatetime
AS
	BEGIN
	return convert(smalldatetime,convert(varchar(8),@dt,112),112)
	END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_RootCodeSN]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fn_RootCodeSN]
	(
    @fp varchar(896)
	)
RETURNS varchar(896)
AS
 BEGIN
	DECLARE @s varchar(896)
	DECLARE @I int
    
	if @fp is not null 
	BEGIN
		  SET @I = CHARINDEX(':', @fp)
		  IF @I > 1	
			SET @s = LEFT(@fp,@I-1)
		  else	  
			SET @s = @fp
	END
    RETURN @s
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_RootCodeS]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_RootCodeS]
	(
    @fp varchar(100)
	)
RETURNS varchar(100)
AS
 BEGIN
	DECLARE @s varchar(100)
	DECLARE @I int
    
	if @fp is not null 
	BEGIN
		  SET @I = CHARINDEX('.', @fp)
		  IF @I > 1	
			SET @s = LEFT(@fp,@I-1)
		  else	  
			SET @s = @fp
	END
    RETURN @s
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_ParCodeSN]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fn_ParCodeSN]
	(
    @fp varchar(896)
	)
RETURNS varchar(896)
AS
 BEGIN
 DECLARE @s varchar(896)
    if @fp is null 
        SET @s = null
    else
	BEGIN
	DECLARE @I int
	  SET @s=reverse(@fp)
	  SET @I = CHARINDEX(':',@s)
	  IF @I > 0 
		SET @s = LEFT(@fp,LEN(@s) - @I)
	  ELSE
		SET @s = null
	END
    RETURN @s
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_ParCodeS]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_ParCodeS]
	(
    @fp varchar(100)
	)
RETURNS varchar(100)
AS
 BEGIN
 DECLARE @s varchar(100)
    if @fp is null 
        SET @s = null
    else
	BEGIN
	DECLARE @I int
	  SET @s=reverse(@fp)
	  SET @I = CHARINDEX('.',@s)
	  IF @I > 0 
		SET @s = LEFT(@fp,LEN(@s) - @I)
	  ELSE
		SET @s = null
	END
    RETURN @s
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_LCodeSN]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  UserDefinedFunction [dbo].[fn_LCodeS]    Script Date: 11/12/2013 12:17:47 ******/
create FUNCTION [dbo].[fn_LCodeSN]
	(
    @fPath varchar(896)
	)
RETURNS varchar(896)
AS
 BEGIN
    DECLARE @LCODES varchar(896)
    if @fPath is null 
        SET @LCODES = ''
    else
	BEGIN
	DECLARE @I int
	  SET @LCODES=reverse(@fPath)
	  SET @I = CHARINDEX(':',@LCODES)
	  IF @I > 0 SET @LCODES = LEFT(@LCODES,@I-1)
	  SET @LCODES = reverse(@LCODES)
	END
    RETURN @LCODES 
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_LCodeS]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_LCodeS]
	(
    @fPath varchar(100)
	)
RETURNS varchar(100)
AS
 BEGIN
    DECLARE @LCODES varchar(100)
    if @fPath is null 
        SET @LCODES = ''
    else
	BEGIN
	DECLARE @I int
	  SET @LCODES=reverse(@fPath)
	  SET @I = CHARINDEX('.',@LCODES)
	  IF @I > 0 SET @LCODES = LEFT(@LCODES,@I-1)
	  SET @LCODES = reverse(@LCODES)
	END
    RETURN @LCODES 
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_GetSubstring]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_GetSubstring](@origin varchar(4096), @divider char, @entryNum int)
RETURNS varchar(4096)
AS
BEGIN

declare @left int = 1;
declare @right int = 0;
declare @cycle int = 0;
declare @ret varchar(4096);

while (@cycle < @entryNum)
begin
	set @left = CHARINDEX(@divider, @origin, @left) + 1;
	set @right = CHARINDEX(@divider, @origin, @left);
	set @ret = substring(@origin, @left, case when (@right - @left > 0) then @right - @left else 255 end);
	set @cycle = @cycle + 1;
	if CHARINDEX(@divider, @ret, 1) > 0
	begin
		set @ret = '';
		BREAK
	end
end

return @ret
END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_FullPathN]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fn_FullPathN]
	(
	@Parent_FP varchar(896),
	@Code varchar(896)
	)
RETURNS varchar(896)
AS
 BEGIN
    RETURN @Parent_FP + ':' + @Code 
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_FullPath]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fn_FullPath]
	(
	@Parent_FP varchar(100),
	@Code int
	)
RETURNS varchar(100)
AS
 BEGIN
    RETURN @Parent_FP+'.'+Cast(@Code as varchar(15)) 
 END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_filterInt]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[fn_filterInt] (
	@LocalVar int,
	@FieldVar int
)
RETURNS bit
AS
	BEGIN
	    IF @LocalVar is NULL RETURN 1
	    IF @LocalVar = @FieldVar RETURN 1
	    RETURN 0 
	END
GO
/****** Object:  UserDefinedFunction [dbo].[fn_CompareXML]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_CompareXML] (@ColumnName varchar(max), @newValue varchar(max), @oldValue varchar(max))
RETURNS varchar(max) 
AS 
begin 

declare @result varchar(max) 
select @result = ''
if isnull(@oldValue, '') <> isnull(@newValue, '')
begin 
	select @result = '<field column="' + @ColumnName + '">'

	if isnull(@oldValue, '') <> '' 
		select @result = @result + '<o>' + convert(varchar(max), isnull(@oldValue, '')) + '</o>'
	
	if isnull(@newValue, '') <> '' 
		select @result = @result + '<n>' + convert(varchar(max), isnull(@newValue, '')) + '</n>'
	
	select @result = @result + '</field>' 
	set @result = replace(@result, '&', '&amp;')

end 
return @result 
end
GO
/****** Object:  UserDefinedFunction [dbo].[fn_Compare]    Script Date: 12/05/2013 12:18:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[fn_Compare] (@ColumnName varchar(max), @newValue varchar(max), @oldValue varchar(max))
RETURNS varchar(max) 
AS 
begin 
declare @result varchar(max) 
select @result = ''
if isnull(@oldValue, '') <> isnull(@newValue, '')
begin 
	select @result = '<field column="' + @ColumnName + '">'

	if isnull(@oldValue, '') <> '' 
		select @result = @result + '<o>' + convert(varchar(max), isnull(@oldValue, '')) + '</o>'
	
	if isnull(@newValue, '') <> '' 
		select @result = @result + '<n>' + convert(varchar(max), isnull(@newValue, '')) + '</n>'
	
	select @result = @result + '</field>' 
	set @result = replace(@result, '&', '&amp;')
end 
return @result 
end
GO

CREATE TABLE [dbo].[BaseUser](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[SCode] [varchar](255) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Phone] [varchar](255) NULL,
	[Workstation] [varchar](255) NULL,
	[EMail] [varchar](255) NULL,
	[IsSupervisor] [bit] NOT NULL,
	[SysUser] [nvarchar](255) NOT NULL,
	[SysDate] [datetime] NOT NULL,
 CONSTRAINT [PK_BaseUser] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Пользователи' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'BaseUser'
GO

ALTER TABLE [dbo].[BaseUser] ADD  CONSTRAINT [DF_BaseUser_IsSupervisor_1]  DEFAULT ((0)) FOR [IsSupervisor]
GO

ALTER TABLE [dbo].[BaseUser] ADD  CONSTRAINT [DF_BaseUser_SysUser]  DEFAULT (suser_sname()) FOR [SysUser]
GO

ALTER TABLE [dbo].[BaseUser] ADD  CONSTRAINT [DF_BaseUser_SysDate]  DEFAULT (getdate()) FOR [SysDate]
GO
