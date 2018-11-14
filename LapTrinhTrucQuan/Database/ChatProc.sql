-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create proc ThemUser
	@Name nvarchar(100),
	@Pass nvarchar(100)
as
	begin
	insert into CHATUSER(Name,Pass)
	values(@Name,@Pass)
	end
go

create proc CheckLogin
	@Name nvarchar(100),
	@Pass nvarchar(100)
as
	select * from CHATUSER where Name=@Name and Pass=@Pass
go

create proc CheckThemUser
	@Name nvarchar(100)
as
	begin
	select * from CHATUSER where Name=@Name
	end
go


create proc History
	@From nvarchar(100),
	@To nvarchar(100)
as
	begin
	select * from PRIVATECHAT where (NameGui=@From and NameNhan=@To) or (NameGui=@To and NameNhan=@From)
	end
go

create proc DeleteHistory
	@From nvarchar(100),
	@To nvarchar(100)
as
	begin
	delete  from PRIVATECHAT where (NameGui=@From and NameNhan=@To) or (NameGui=@To and NameNhan=@From)
	end
go

create proc InsertPrivateChat
	@From nvarchar(100),
	@To nvarchar(100),
	@Mess nvarchar(1000)
as
	begin
	insert into PRIVATECHAT
	values(@From,@To,@Mess)
	end
go

--create proc InsertDetail
--	@From nvarchar(100),
--	@To nvarchar(100),
--	@Mess nvarchar(1000)
--as
--	begin
--	insert into DETAILPRIVATE
--	values('',@Mess)
--	update DETAILPRIVATE
--	set IDPRI= (select ID from PRIVATECHAT where PRIVATECHAT.NameGui=@From and PRIVATECHAT.NameNhan=@To)
--	end
--go


