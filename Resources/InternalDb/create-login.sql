-- in master

IF NOT EXISTS 
    (SELECT name  
     FROM master.sys.server_principals
     WHERE name = 'PaymentDbAdminLogin')
BEGIN
    CREATE LOGIN [PaymentDbAdminLogin] WITH PASSWORD = N'1231!#ASDF!a'
END

-- in PaymentDb

CREATE USER PaymentDbAdminUser for login PaymentDbAdminLogin

grant delete, insert, select, update on [dbo].[PaymentUsers] to PaymentDbAdminUser;
grant delete, insert, select, update on [dbo].[Payments] to PaymentDbAdminUser;
grant delete, insert, select, update on [dbo].[Payment2Debitor] to PaymentDbAdminUser;
grant delete, insert, select, update on [dbo].[payments_tscheschien_24] to PaymentDbAdminUser;
grant delete, insert, select, update on [dbo].[payment2debitor_tscheschien_24] to PaymentDbAdminUser;
grant delete, insert, select, update on [dbo].[PaymentContext] to PaymentDbAdminUser;