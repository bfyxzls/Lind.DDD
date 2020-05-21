--数据迁移
--部门表
--角色表
--用户表
--菜单表
--角色用户菜单授权表

--数据库
DECLARE @oldDbName VARCHAR(128)
DECLARE @newDbName VARCHAR(128)
SET @oldDbName = '[Tsingda.Cloud]'
SET @newDbName = '[Tsingda.CloudNew]'
--部门
DECLARE @deptId INT 
DECLARE @oldDeptId INT 
DECLARE @deptLevel INT 
DECLARE @deptParentId INT 
DECLARE @deptName VARCHAR(128)
DECLARE @deptAbout VARCHAR(128)	
DECLARE @deptParentName VARCHAR(128)
--角色
DECLARE @roleId INT 
DECLARE @oldRoleId INT 
DECLARE @roleName VARCHAR(128)
DECLARE @roleAbout VARCHAR(128)
--用户
DECLARE @userId INT 
DECLARE @oldUserId INT 
DECLARE @loginName VARCHAR(128)
DECLARE @password VARCHAR(128)
DECLARE @realName VARCHAR(128)
DECLARE @mobile VARCHAR(128)
DECLARE @email VARCHAR(128)
DECLARE @description VARCHAR(128)
--菜单
DECLARE @menuId INT 
DECLARE @oldMenuId INT 
DECLARE @menuLevel INT 
DECLARE @menuParentId INT 
DECLARE @menuParentName VARCHAR(256)
DECLARE @menuName VARCHAR(128)
DECLARE @linkUrl VARCHAR(128)
DECLARE @menuAbout VARCHAR(128)
DECLARE @isDisplayMenuTree BIT
--权限
DECLARE @authority BIGINT
--数据集
DECLARE @CtrlName VARCHAR(128) 
DECLARE @objectIdArr VARCHAR(1024)
											 
DECLARE @provinceId INT 
DECLARE @cityId INT 
DECLARE @district INT 
SELECT  @provinceId = ID
FROM    [Tsingda.Cloud].dbo.WebDataCtrls
WHERE   DataCtrlType = 'CommonArea'
        AND DataCtrlField = 'AreaProvince'
IF @@ROWCOUNT = 0 
    BEGIN
        INSERT  INTO [Tsingda.Cloud].dbo.WebDataCtrls
                ( DataCtrlName ,
                  DataCtrlType ,
                  DataCtrlField ,
                  DataCtrlApi ,
                  Description ,
                  DataCreateDateTime ,
                  DataUpdateDateTime ,
                  DataStatus
                )
        VALUES  ( '角色的区域控制' , -- DataCtrlName - nvarchar(max)
                  'CommonArea' , -- DataCtrlType - nvarchar(max)
                  'AreaProvince' , -- DataCtrlField - nvarchar(max)
                  '#' , -- DataCtrlApi - nvarchar(max)
                  '#' , -- Description - nvarchar(max)
                  '2016-09-30 05:33:50' , -- DataCreateDateTime - datetime
                  '2016-09-30 05:33:50' , -- DataUpdateDateTime - datetime
                  1  -- DataStatus - int
                )
     
        SET @provinceId = @@IDENTITY
    END
SELECT  @cityId = ID
FROM    [Tsingda.Cloud].dbo.WebDataCtrls
WHERE   DataCtrlType = 'CommonArea'
        AND DataCtrlField = 'AreaCity'
IF @@ROWCOUNT = 0 
    BEGIN
        INSERT  INTO [Tsingda.Cloud].dbo.WebDataCtrls
                ( DataCtrlName ,
                  DataCtrlType ,
                  DataCtrlField ,
                  DataCtrlApi ,
                  Description ,
                  DataCreateDateTime ,
                  DataUpdateDateTime ,
                  DataStatus
                )
        VALUES  ( '角色的区域控制' , -- DataCtrlName - nvarchar(max)
                  'CommonArea' , -- DataCtrlType - nvarchar(max)
                  'AreaCity' , -- DataCtrlField - nvarchar(max)
                  '#' , -- DataCtrlApi - nvarchar(max)
                  '#' , -- Description - nvarchar(max)
                  '2016-09-30 05:33:50' , -- DataCreateDateTime - datetime
                  '2016-09-30 05:33:50' , -- DataUpdateDateTime - datetime
                  1  -- DataStatus - int
                )
   
        SET @cityId = @@IDENTITY
    END
SELECT  @district = ID
FROM    [Tsingda.Cloud].dbo.WebDataCtrls
WHERE   DataCtrlType = 'CommonArea'
        AND DataCtrlField = 'AreaDistrict'
IF @@ROWCOUNT = 0 
    BEGIN
        INSERT  INTO [Tsingda.Cloud].dbo.WebDataCtrls
                ( DataCtrlName ,
                  DataCtrlType ,
                  DataCtrlField ,
                  DataCtrlApi ,
                  Description ,
                  DataCreateDateTime ,
                  DataUpdateDateTime ,
                  DataStatus
                )
        VALUES  ( '角色的区域控制' , -- DataCtrlName - nvarchar(max)
                  'CommonArea' , -- DataCtrlType - nvarchar(max)
                  'AreaDistrict' , -- DataCtrlField - nvarchar(max)
                  '#' , -- DataCtrlApi - nvarchar(max)
                  '#' , -- Description - nvarchar(max)
                  '2016-09-30 05:33:50' , -- DataCreateDateTime - datetime
                  '2016-09-30 05:33:50' , -- DataUpdateDateTime - datetime
                  1  -- DataStatus - int
                )
      
        SET @district = @@IDENTITY
    END    
--程序正文开始
DECLARE cursorMenu CURSOR
FOR
    SELECT  a.name ,
            a.level ,
            a.LinkUrl ,
            a.About ,
            a.IsDisplayMenuTree ,
            b.Name AS Father
    FROM    [Tsingda.CloudOld].dbo.WebManageMenus a
            LEFT JOIN [Tsingda.CloudOld].dbo.WebManageMenus b ON a.ParentID = b.id
    WHERE   a.DataStatus = 1
            AND a.name NOT IN ( SELECT  name
                                FROM    [Tsingda.Cloud].dbo.WebManageMenus )
    ORDER BY a.Level

OPEN cursorMenu
FETCH NEXT FROM cursorMenu INTO @menuName, @menuLevel, @linkUrl, @menuAbout,
    @isDisplayMenuTree, @menuParentName
 
WHILE @@fetch_status = 0 
    BEGIN
  
        SELECT  @menuParentId = ID
        FROM    [Tsingda.Cloud].dbo.WebManageMenus
        WHERE   Name = @menuParentName
   
        INSERT  INTO [Tsingda.Cloud].dbo.WebManageMenus
                ( Name ,
                  Level ,
                  LinkUrl ,
                  About ,
                  SortNumber ,
                  Operator ,
                  IsDisplayMenuTree ,
                  ParentID ,
                  DataCreateDateTime ,
                  DataUpdateDateTime ,
                  DataStatus
	           
                )
        VALUES  ( @menuName , -- Name - nvarchar(max)
                  @menuLevel , -- Level - int
                  @linkUrl , -- LinkUrl - nvarchar(max)
                  @menuAbout , -- About - nvarchar(max)
                  0 , -- SortNumber - int
                  N'' , -- Operator - nvarchar(max)
                  @isDisplayMenuTree , -- IsDisplayMenuTree - bit
                  @menuParentId , -- ParentID - int
                  '2016-09-29 08:43:11' , -- DataCreateDateTime - datetime
                  '2016-09-29 08:43:11' , -- DataUpdateDateTime - datetime
                  1  -- DataStatus - int
	           
                )
     
        FETCH NEXT FROM cursorMenu INTO @menuName, @menuLevel, @linkUrl,
            @menuAbout, @isDisplayMenuTree, @menuParentName
    END
CLOSE cursorMenu
DEALLOCATE cursorMenu

DECLARE cursor1 CURSOR
FOR
    SELECT  a.id ,
            a.Level ,
            a.name ,
            a.About ,
            b.NAME AS Father
    FROM    [Tsingda.CloudOld].dbo.WebDepartments a
            LEFT JOIN [Tsingda.CloudOld].dbo.WebDepartments b ON a.ParentID = b.id
    WHERE   a.DataStatus = 1
            AND a.name NOT IN ( SELECT  name
                                FROM    [Tsingda.Cloud].dbo.WebDepartments )
    ORDER BY a.Level

OPEN cursor1
FETCH NEXT FROM cursor1 INTO @oldDeptId, @deptLevel, @deptName, @deptAbout,
    @deptParentName
 
WHILE @@fetch_status = 0 
    BEGIN
        SELECT  @deptParentId = ID
        FROM    [Tsingda.Cloud].dbo.WebDepartments
        WHERE   Name = @deptParentName

        INSERT  [Tsingda.Cloud].dbo.WebDepartments
                ( Name ,
                  About ,
                  SortNumber ,
                  Operator ,
                  Level ,
                  ParentID ,
                  DataCreateDateTime ,
                  DataUpdateDateTime ,
                  DataStatus
                )
        VALUES  ( @deptName , -- Name - nvarchar(max)
                  @deptAbout , -- About - nvarchar(max)
                  0 , -- SortNumber - int
                  N'' , -- Operator - nvarchar(max)
                  @deptLevel , -- Level - int
                  @deptParentId , -- ParentID - int
                  '2016-09-27 08:39:34' , -- DataCreateDateTime - datetime
                  '2016-09-27 08:39:34' , -- DataUpdateDateTime - datetime
                  1  -- DataStatus - int
                )
           
        SET @deptId = @@IDENTITY

     --部门角色
        DECLARE cursorRole CURSOR
        FOR
            SELECT  id ,
                    RoleName ,
                    About
            FROM    [Tsingda.CloudOld].dbo.WebManageRoles
            WHERE   DepartmentID = @oldDeptId 
        OPEN cursorRole
        FETCH NEXT FROM cursorRole INTO @oldRoleId, @roleName, @roleAbout
 
        WHILE @@fetch_status = 0 
            BEGIN
                INSERT  INTO [Tsingda.Cloud].dbo.WebManageRoles
                        ( RoleName ,
                          About ,
                          SortNumber ,
                          Operator ,
                          DepartmentID ,
                          DataCreateDateTime ,
                          DataUpdateDateTime ,
                          DataStatus
	                  )
                VALUES  ( @roleName , -- RoleName - nvarchar(max)
                          @roleAbout , -- About - nvarchar(max)
                          0 , -- SortNumber - int
                          N'' , -- Operator - nvarchar(max)
                          @deptId , -- DepartmentID - int
                          '2016-09-27 08:45:59' , -- DataCreateDateTime - datetime
                          '2016-09-27 08:45:59' , -- DataUpdateDateTime - datetime
                          1  -- DataStatus - int
	                  )
                SET @roleId = @@IDENTITY
		 
		 
--部门用户		 
                DECLARE cursorUser CURSOR
                FOR
                    SELECT  id ,
                            LoginName ,
                            RealName ,
                            Password ,
                            Mobile ,
                            email ,
                            Description
                    FROM    [Tsingda.CloudOld].dbo.WebManageUsers
                    WHERE   DataStatus = 1
                            AND LoginName NOT IN (
                            SELECT  LoginName
                            FROM    [Tsingda.Cloud].dbo.WebManageUsers )
                            AND id IN (
                            SELECT  userid
                            FROM    [Tsingda.CloudOld].dbo.WebDepartments_WebManageUsers_R
                            WHERE   DeptId = @oldDeptId )

                OPEN cursorUser
                FETCH NEXT FROM cursorUser INTO @oldUserId, @loginName,
                    @realName, @password, @mobile, @email, @description
 
                WHILE @@fetch_status = 0 
                    BEGIN
   
                        INSERT  INTO [Tsingda.Cloud].dbo.WebManageUsers
                                ( LoginName ,
                                  Password ,
                                  RealName ,
                                  Mobile ,
                                  Email ,
                                  Description ,
                                  Operator ,
                                  WebSystemID ,
                                  ThridUserId ,
                                  DataCreateDateTime ,
                                  DataUpdateDateTime ,
                                  DataStatus
	                          )
                        VALUES  ( @loginName , -- LoginName - nvarchar(max)
                                  @password , -- Password - nvarchar(max)
                                  @realName , -- RealName - nvarchar(max)
                                  @mobile , -- Mobile - nvarchar(max)
                                  @email , -- Email - nvarchar(max)
                                  @description , -- Description - nvarchar(max)
                                  N'' , -- Operator - nvarchar(max)
                                  0 , -- WebSystemID - int
                                  N'' , -- ThridUserId - nvarchar(max)
                                  '2016-09-27 08:56:46' , -- DataCreateDateTime - datetime
                                  '2016-09-27 08:56:46' , -- DataUpdateDateTime - datetime
                                  1  -- DataStatus - int
	                          )
                        SET @userId = @@IDENTITY
	          --部门-用户-关系
                        INSERT  INTO [Tsingda.Cloud].dbo.WebDepartments_WebManageUsers_R
                                ( DeptId, UserId )
                        VALUES  ( @deptId, -- DeptId - int
                                  @userId  -- UserId - int
                                  )
	          --授权用户－角色
                        SELECT  *
                        FROM    [Tsingda.CloudOld].dbo.WebManageRoles_WebManageUsers_R
                        WHERE   RoleId = @oldRoleId
                                AND UserId = @oldUserId
                       
                        IF @@ROWCOUNT > 0 
                            BEGIN
                                PRINT 'roleid=' + STR(@oldRoleId) + ',userid='
                                    + STR(@oldUserId)
                                INSERT  INTO [Tsingda.Cloud].dbo.WebManageRoles_WebManageUsers_R
                                        ( RoleId, UserId )
                                VALUES  ( @roleId, -- RoleId - int
                                          @userId  -- UserId - int
                                          )
                            END
					  
                        FETCH NEXT FROM cursorUser INTO @oldUserId, @loginName,
                            @realName, @password, @mobile, @email,
                            @description
                    END
                CLOSE cursorUser
                DEALLOCATE cursorUser

			   --菜单-权限-权限
                DECLARE cursorMenu CURSOR
                FOR
                    SELECT  a.MenuId ,
                            c.ID ,
                            a.Authority
                    FROM    [Tsingda.CloudOld].dbo.WebManageRoles_WebManageMenus_Authority_R a
                            INNER JOIN [Tsingda.CloudOld].dbo.WebManageMenus b ON a.MenuId = b.ID
                            INNER JOIN [Tsingda.Cloud].dbo.WebManageMenus c ON b.Name = c.Name
                    WHERE   RoleId = @oldRoleId

                OPEN cursorMenu
                FETCH NEXT FROM cursorMenu INTO @oldMenuId, @menuId,
                    @authority
  
                WHILE @@fetch_status = 0 
                    BEGIN
				 --添加到新库的权限表
                        INSERT  INTO [Tsingda.Cloud].dbo.WebManageRoles_WebManageMenus_Authority_R
                                ( RoleId ,
                                  MenuId ,
                                  Authority ,
                                  DataCreateDateTime ,
                                  DataUpdateDateTime ,
                                  DataStatus
				         
                                )
                        VALUES  ( @roleId , -- RoleId - int
                                  @menuId , -- MenuId - int
                                  @authority , -- Authority - int
                                  '2016-09-29 09:28:26' , -- DataCreateDateTime - datetime
                                  '2016-09-29 09:28:26' , -- DataUpdateDateTime - datetime
                                  1  -- DataStatus - int
				         
                                )
     
                        FETCH NEXT FROM cursorMenu INTO @oldMenuId, @menuId,
                            @authority
                    END
                CLOSE cursorMenu
                DEALLOCATE cursorMenu
		 
--角色－数据集
                DECLARE cursorData CURSOR
                FOR
                    SELECT  b.DataCtrlField ,
                            a.ObjectIdArr
                    FROM    [Tsingda.CloudOld].dbo.WebDataSettings a
                            INNER JOIN [Tsingda.CloudOld].dbo.WebDataCtrls b ON a.WebDataCtrlId = b.ID
                    WHERE   a.WebManageRolesId = @roleId
					 
                OPEN cursorData
                FETCH NEXT FROM cursorData INTO @CtrlName, @objectIdArr
                WHILE @@fetch_status = 0 
                    BEGIN
                        IF @CtrlName = 'AreaProvince' 
                            BEGIN
                                INSERT  [Tsingda.Cloud].dbo.WebDataSettings
                                        ( WebDataCtrlId ,
                                          WebManageRolesId ,
                                          ObjectIdArr ,
                                          DataCreateDateTime ,
                                          DataUpdateDateTime ,
                                          DataStatus ,
                                          WebDepartments_Id
						           
                                        )
                                VALUES  ( @provinceId , -- WebDataCtrlId - int
                                          @roleId , -- WebManageRolesId - int
                                          @objectIdArr , -- ObjectIdArr - nvarchar(max)
                                          '2016-09-30 05:51:56' , -- DataCreateDateTime - datetime
                                          '2016-09-30 05:51:56' , -- DataUpdateDateTime - datetime
                                          1 , -- DataStatus - int
                                          @deptId  -- WebDepartments_Id - int
						           
                                        )
                            END
						   
                        IF @CtrlName = 'AreaCity' 
                            BEGIN
                                INSERT  [Tsingda.Cloud].dbo.WebDataSettings
                                        ( WebDataCtrlId ,
                                          WebManageRolesId ,
                                          ObjectIdArr ,
                                          DataCreateDateTime ,
                                          DataUpdateDateTime ,
                                          DataStatus ,
                                          WebDepartments_Id
						           
                                        )
                                VALUES  ( @cityId , -- WebDataCtrlId - int
                                          @roleId , -- WebManageRolesId - int
                                          @objectIdArr , -- ObjectIdArr - nvarchar(max)
                                          '2016-09-30 05:51:56' , -- DataCreateDateTime - datetime
                                          '2016-09-30 05:51:56' , -- DataUpdateDateTime - datetime
                                          1 , -- DataStatus - int
                                          @deptId  -- WebDepartments_Id - int
						           
                                        )
                            END
						   
                        IF @CtrlName = 'AreaDistrict' 
                            BEGIN
                                INSERT  [Tsingda.Cloud].dbo.WebDataSettings
                                        ( WebDataCtrlId ,
                                          WebManageRolesId ,
                                          ObjectIdArr ,
                                          DataCreateDateTime ,
                                          DataUpdateDateTime ,
                                          DataStatus ,
                                          WebDepartments_Id
						           
                                        )
                                VALUES  ( @district , -- WebDataCtrlId - int
                                          @roleId , -- WebManageRolesId - int
                                          @objectIdArr , -- ObjectIdArr - nvarchar(max)
                                          '2016-09-30 05:51:56' , -- DataCreateDateTime - datetime
                                          '2016-09-30 05:51:56' , -- DataUpdateDateTime - datetime
                                          1 , -- DataStatus - int
                                          @deptId  -- WebDepartments_Id - int
						           
                                        )
                            END
								 
 
                        FETCH NEXT FROM cursorData INTO @CtrlName,
                            @objectIdArr
                         
                    END
                CLOSE cursorData
                DEALLOCATE cursorData
                
                
                FETCH NEXT FROM cursorRole INTO @oldRoleId, @roleName,
                    @roleAbout    
            END
        CLOSE cursorRole
        DEALLOCATE cursorRole		  
     
     
        FETCH NEXT FROM cursor1 INTO @oldDeptId, @deptLevel, @deptName,
            @deptAbout, @deptParentName
    END
CLOSE cursor1
DEALLOCATE cursor1