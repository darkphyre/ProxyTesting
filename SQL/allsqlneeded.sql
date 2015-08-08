use [master]
go
create database ProxyWorld;
go
use [ProxyWorld]
go
CREATE TABLE [dbo].[tblProxy](
	[IP] [nvarchar](50) NOT NULL,
	[Port] [int] NOT NULL,
	[LastTestDate] [datetime] NOT NULL,
	[LastTestBy] [nvarchar](50) NOT NULL,
	[Active] [bit] NOT NULL CONSTRAINT [DF_tblProxy_Active]  DEFAULT ((1))
) ON [PRIMARY]
go
CREATE TABLE [dbo].[tblProxyTestResults](
	[UniqueId] [int] IDENTITY(1,1) NOT NULL,
	[IP] [nvarchar](50) NOT NULL,
	[Port] [int] NOT NULL,
	[TestDate] [datetime] NOT NULL,
	[TestBy] [nvarchar](50) NOT NULL,
	[PingTimeResults] [time](2) NOT NULL,
	[PingTestGrade] [int] NOT NULL,
 CONSTRAINT [PK_tblProxyTestResults] PRIMARY KEY CLUSTERED 
(
	[UniqueId] ASC,
	[IP] ASC,
	[Port] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
go
CREATE view [dbo].[ProxyTestResults]
as
select
   IP,
   Port,
   case           
      when avg(PingTestGrade) < 1.2 then 'Premium'           
      when avg(PingTestGrade) < 2.2 then 'Good'            
      when avg(PingTestGrade) < 3.4 then 'Average'             
      when avg(PingTestGrade) > 3.4 then 'Below Average'      
   end as 'Grade',
   avg(PingTestGrade) as 'AverageResult'   
from
   tblProxyTestResults     
where
   exists (
      select
         'x' 
      from
         tblProxyTestResults a      
      where
         a.IP = tblProxyTestResults.IP 
         and a.Port = tblProxyTestResults.Port 
         and a.TestDate > Dateadd(hh,-24,GetDate())
   )  
group by
   IP,
   Port
GO
create procedure [dbo].[sp_DeleteFailedProxyTests]   as   begin    delete   
from
   tblProxy     
where
   not exists (
      select
         'x'         
      from
         tblProxyTestResults a         
      where
         a.Ip = tblProxy.IP            
         and a.Port = tblProxy.Port     
   )       
end
go
create procedure [dbo].[sp_GrabAllActiveProxies]
as
begin
select * from tblProxy where Active = 1;
end
GO

create  procedure [dbo].[sp_insertProxyResult](@IP nvarchar(50),@Port int,@TestDate datetime,@TestBy nvarchar(50),@PingTimeResults time(2),@PingTestGrade int)
as
begin
insert into tblProxyTestResults(IP, Port, TestDate, TestBy, PingTimeResults, PingTestGrade)
values(@IP, @Port, @TestDate, @TestBy, @PingTimeResults, @PingTestGrade);


end
GO

USE [master]
GO
CREATE LOGIN [ProxyWorld] WITH PASSWORD=N'proxyworld', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
USE [ProxyWorld]
GO
CREATE USER [ProxyWorld] FOR LOGIN [ProxyWorld]
GO
USE [ProxyWorld]
GO
ALTER ROLE [db_datareader] ADD MEMBER [ProxyWorld]
GO
USE [ProxyWorld]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [ProxyWorld]
GO
USE [ProxyWorld]
GO
ALTER ROLE [db_ddladmin] ADD MEMBER [ProxyWorld]
GO
USE [ProxyWorld]
go
Grant execute to [ProxyWorld];

go