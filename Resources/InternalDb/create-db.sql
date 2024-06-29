create table PaymentUsers (
	Id					bigint			not null		primary key		identity(1,1),
	FirstName			varchar(254)	not null,
	LastName			varchar(254)	not null,
	Username			varchar(254)	not null,

	constraint PaymentUsers unique (Username)
);

create table Payments (
	Id					bigint			not null		primary key		identity(1,1),
	CreditorIdFk		bigint			not null,
	Price				decimal(5, 2)	not null,
	PaymentDate			datetime2		not null,
	Description			varchar(254),
	AuthorIdFk			bigint			not null,
	UpdateTime			datetime2		not null,

	constraint FK_Payment_CreditorIdFk foreign key (CreditorIdFk)
	references PaymentUsers(Id),
	
	constraint FK_Payment_AuthorIdFk foreign key (AuthorIdFk)
	references PaymentUsers(Id)
);

create table Payment2Debitor(
	Id					bigint			not null		primary key		identity(1,1),
	DebitorIdFk			bigint			not null,
	PaymentIdFk			bigint			not null,

	constraint FK_Payment2Debitor_DebitorIdFk foreign key (DebitorIdFk)
	references PaymentUsers(Id),

	constraint FK_Payment2Debitor_PaymentIdFk foreign key (PaymentIdFk)
	references Payments(Id)
);