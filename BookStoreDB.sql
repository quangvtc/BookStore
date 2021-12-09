drop database if exists BookStoreDB;

create database BookStoreDB;

use BookStoreDB;

create table Users(
	user_id int auto_increment primary key,
    staff_name varchar(200),
    user_name varchar(100) not null unique,
    user_pass varchar(200) not null,
    address varchar (200),
    telephone varchar(20),
    email varchar(100) unique,
    role int not null default 1
);

create table Books(
	book_id int auto_increment primary key,
    book_name varchar(200) not null,
    unit_price decimal(20,2) default 0,
    amount int not null default 0,
    book_status tinyint not null, 
    book_description varchar(500)
);

create table Customers(
	customer_id int auto_increment primary key,
    customer_name varchar(200),
    customer_address varchar(200),
    customer_phone varchar(200)
);

create table Invoices(
	invoice_id int auto_increment primary key,
    invoice_date datetime default now() not null,
    invoice_status int,
    customer_id int not null,
    constraint fk_Invoice_Customers foreign key (customer_id) references Customers(customer_id),
    user_id int not null,
    constraint fk_Invoice_Users foreign key (user_id) references Users(user_id)
);

create table InvoiceDetails(
	invoice_id int not null,
    constraint fk_InvoiceDetails_Invoices foreign key (invoice_id) references Invoices(invoice_id),
    book_id int not null,
    constraint fk_InvoiceDetails_Books foreign key (book_id) references Books(book_id),
    constraint pk_InvoiceDetails primary key(invoice_id, book_id),
    unit_price decimal(20,2) not null,
    quantity int not null default 1
);

delimiter $$
create trigger tg_before_insert before insert
	on Books for each row
    begin
		if new.amount < 0 then
            signal sqlstate '45001' set message_text = 'tg_before_insert: amount must > 0';
        end if;
    end $$
delimiter ;

delimiter $$
create trigger tg_CheckAmount
	before update on Books
	for each row
	begin
		if new.amount < 0 then
            signal sqlstate '45001' set message_text = 'tg_CheckAmount: amount must > 0';
        end if;
    end $$
delimiter ;

delimiter $$
create procedure sp_createCustomer(IN customerName varchar(100), IN customerAddress varchar(200), IN customerPhone varchar(200),OUT customerId int)
begin
	insert into Customers(customer_name, customer_address, customer_phone) values (customerName, customerAddress, customerPhone); 
    select max(customer_id) into customerId from Customers;
end $$
delimiter ;

call sp_createCustomer('no name','any where','any number', @cusId);
select @cusId;

/* INSERT DATA */
insert into Users(user_name, user_pass, role) values
					 -- PF15VTCAcademy
	( 'pf15', '827ccb0eea8a706c4c34a16891f84e7b', 1),
    ( 'QuangNguyen', '827ccb0eea8a706c4c34a16891f84e7b', 1);
select * from Users;

insert into Customers(customer_name, customer_address, customer_phone) values
	('Nguyen Thi X','Hai Duong','0904282103'),
    ('Nguyen Van N','Hanoi','0376054910'),
    ('Nguyen Van B','Ho Chi Minh','0909222333'),
    ('Nguyen Van A','Hanoi','0376054921');
select * from Customers;

insert into Books(book_name, unit_price, amount, book_status) values
	('Item 1', 12.5, 8, 1),
    ('Item 2', 62.5, 6, 1),
    ('Item 3', 31.0, 10, 1),
    ('Item 4', 24.5, 5, 1),
    ('Item 5', 7.5, 9, 1);
select * from Books;

insert into Invoices(customer_id, user_id, invoice_status) values
	(1, 1, 1), (2, 1, 1), (1, 1, 1);
select * from Invoices;

insert into InvoiceDetails(invoice_id, book_id, unit_price, quantity) values
	(1, 1, 12.5, 5), (1, 3, 31.0, 1), (1, 4, 24.5, 2),
    (2, 2, 62.5, 1), (2, 3, 31.0, 2), (2, 5, 7.5, 4);
select * from InvoiceDetails;

/* CREATE & GRANT USER */
create user if not exists 'vtca'@'localhost' identified by 'nguyenquang94';
grant all on BookStoreDB.* to 'vtca'@'localhost';
-- grant all on Books to 'vtca'@'localhost';
-- grant all on Customers to 'vtca'@'localhost';
-- grant all on Invoices to 'vtca'@'localhost';
-- grant all on InvoiceDetails to 'vtca'@'localhost';

select book_id from Books order by book_id desc limit 1;

select customer_id, customer_name, customer_phone,
    ifnull(customer_address, '') as customer_address
from Customers where customer_id=1;

select invoice_id from Invoices order by invoice_id desc limit 1;

select LAST_INSERT_ID();
select customer_id from Customers order by customer_id desc limit 1;

select user_id from Users order by user_id desc limit 1;

update Books set amount=10 where book_id=3;
-- lock table Invoices write;
-- unlock tables;

