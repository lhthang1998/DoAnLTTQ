create database Chat
drop database Chat
drop table CHATUSER
create table CHATUSER
(
	Name nvarchar(100) primary key,
	Pass nvarchar(100)
)

select * from CHATUSER
DROP TABLE PRIVATECHAT
create table PRIVATECHAT
(
	ID int primary key identity,
	NameGui nvarchar(100) foreign key references CHATUSER(Name) ,
	NameNhan nvarchar(100) foreign key references CHATUSER(Name),
	NoiDung nvarchar(1000)  
	
)

delete from PRIVATECHAT where (NameGui='a' and NameNhan='c') or (NameGui='c' and NameNhan='a')

select * from PRIVATECHAT where (NameGui='a' and NameNhan='b') or (NameGui='b' and NameNhan='a')