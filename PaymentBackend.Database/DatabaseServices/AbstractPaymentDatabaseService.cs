using Microsoft.Extensions.Logging;
using PaymentBackend.Common.Model.Dto;
using PaymentBackend.Settings;
using System.Data.SqlClient;
using System.Data;

namespace PaymentBackend.Database.DatabaseServices
{
    public abstract class AbstractPaymentDatabaseService : AbstractDatabaseService
    {
        protected AbstractPaymentDatabaseService(
            ISqlExceptionHandler exceptionHandler,
            IFunctionSettingsResolver functionSettingsResolver,
            ILogger logger) : base(exceptionHandler,
            functionSettingsResolver,
            logger)
        {
        }

        protected List<JoinedPayment2DebitorDto> SelectAllPayment2Debitors(SqlConnection connection)
        {
            string sql = @"
select 
	p.Id as PId, 
	p.Price as PPrice,
	p.PaymentDate as PPaymentDate,
	p.UpdateTime as PUpdateTime,
    p.Description as PDescription,
	p2d.Id as P2DId,
	u.Id as UDebitorId,
	u.Username as UDebitorUsername,
    v.Id as UCreditorId,
	v.Username as UCreditorUsername,
    w.Id as UAuthorId,
	w.Username as UAuthorUsername
from 
	Payment2Debitor p2d
join Payments p 
	on p.Id = p2d.PaymentIdFk
join PaymentUsers u
	on p2d.DebitorIdFk = u.Id 
join PaymentUsers v 
	on p.CreditorIdFk = v.Id
join PaymentUsers w
	on p.AuthorIdFk = w.Id
order by 
    p.PaymentDate asc
";

            using SqlCommand cmd1 = new(sql, connection);
            cmd1.CommandType = CommandType.Text;

            List<JoinedPayment2DebitorDto> result = new();

            using SqlDataReader reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected List<JoinedPayment2DebitorDto> SelectPaymentById(SqlConnection connection, long id)
        {
            string sql = @"
with p2d as (
    select 
        *
    from
        Payment2Debitor
    where
        PaymentIdFk = @PaymentId
)
select 
	p.Id as PId, 
	p.Price as PPrice,
	p.PaymentDate as PPaymentDate,
	p.UpdateTime as PUpdateTime,
    p.Description as PDescription,
	p2d.Id as P2DId,
	u.Id as UDebitorId,
	u.Username as UDebitorUsername,
    v.Id as UCreditorId,
	v.Username as UCreditorUsername,
    w.Id as UAuthorId,
	w.Username as UAuthorUsername
from 
    p2d
join Payments p 
	on p.Id = p2d.PaymentIdFk
join PaymentUsers u
	on p2d.DebitorIdFk = u.Id 
join PaymentUsers v 
	on p.CreditorIdFk = v.Id
join PaymentUsers w
	on p.AuthorIdFk = w.Id
order by 
    p.PaymentDate asc
";

            using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@PaymentId", id);

            List<JoinedPayment2DebitorDto> result = new();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected List<JoinedPayment2DebitorDto> SelectPaymentsByCreditor(SqlConnection connection, string username)
        {
            string sql = @"
with c as (
    select 
        *
    from
        PaymentUsers
    where
        Username = @Username
)
select 
	p.Id as PId, 
	p.Price as PPrice,
	p.PaymentDate as PPaymentDate,
	p.UpdateTime as PUpdateTime,
    p.Description as PDescription,
	p2d.Id as P2DId,
	d.Id as UDebitorId,
	d.Username as UDebitorUsername,
    c.Id as UCreditorId,
	c.Username as UCreditorUsername,
    a.Id as UAuthorId,
	a.Username as UAuthorUsername
from
	c
join Payments p 
	on p.CreditorIdFk = c.Id
join Payment2Debitor p2d
	on p2d.PaymentIdFk = p.Id
join PaymentUsers d
	on p2d.DebitorIdFk = d.Id 
join PaymentUsers a
	on p.AuthorIdFk = a.Id
order by 
    p.PaymentDate asc
";

            using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Username", username);

            List<JoinedPayment2DebitorDto> result = new();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected List<JoinedPayment2DebitorDto> SelectPaymentsByDebitor(SqlConnection connection, string username)
        {
            string sql = @"
with d as (
    select 
        *
    from
        PaymentUsers
    where
        Username = @Username
),
paymentIds as (
	select 
		PaymentIdFk as PaymentIdFk
	from	
		d
	join Payment2Debitor p2d
		on p2d.DebitorIdFk = d.Id
)
select 
	p.Id as PId, 
	p.Price as PPrice,
	p.PaymentDate as PPaymentDate,
	p.UpdateTime as PUpdateTime,
    p.Description as PDescription,
	p2d.Id as P2DId,
	allDebitors.Id as UDebitorId,
	allDebitors.Username as UDebitorUsername,
    c.Id as UCreditorId,
	c.Username as UCreditorUsername,
    a.Id as UAuthorId,
	a.Username as UAuthorUsername
from 
	paymentIds
join Payments p
	on p.Id = paymentIds.PaymentIdFk
join Payment2Debitor p2d
	on p2d.PaymentIdFk = p.Id
join PaymentUsers c
	on p.CreditorIdFk = c.Id 
join PaymentUsers a
	on p.AuthorIdFk = a.Id
join PaymentUsers allDebitors
	on allDebitors.Id = p2d.DebitorIdFk
order by 
    p.PaymentDate asc
";

            using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Username", username);

            List<JoinedPayment2DebitorDto> result = new();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected List<JoinedPayment2DebitorDto> SelectPaymentsByAuthor(SqlConnection connection, string username)
        {
            string sql = @"
with a as (
    select 
        *
    from
        PaymentUsers
    where
        Username = @Username
)
select 
	p.Id as PId, 
	p.Price as PPrice,
	p.PaymentDate as PPaymentDate,
	p.UpdateTime as PUpdateTime,
    p.Description as PDescription,
	p2d.Id as P2DId,
	d.Id as UDebitorId,
	d.Username as UDebitorUsername,
    c.Id as UCreditorId,
	c.Username as UCreditorUsername,
    a.Id as UAuthorId,
	a.Username as UAuthorUsername
from
	a
join Payments p 
	on p.CreditorIdFk = a.Id
join Payment2Debitor p2d
	on p2d.PaymentIdFk = p.Id
join PaymentUsers d
	on p2d.DebitorIdFk = d.Id 
join PaymentUsers c
	on p.CreditorIdFk = c.Id
order by 
    p.PaymentDate asc
";

            using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Username", username);

            List<JoinedPayment2DebitorDto> result = new();

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected List<FullPaymentDto> MergeJoinedPayments(List<JoinedPayment2DebitorDto> joinedPayments)
        {
            List<FullPaymentDto> result = new();

            foreach (var p2d in joinedPayments)
            {
                var match = result.Find(payment => payment.Id == p2d.PaymentId);

                if (match == null)
                {
                    FullPaymentDto newPaymentDto = new FullPaymentDto()
                    {
                        Id = p2d.PaymentId,
                        Creditor = p2d.CreditorUsername,
                        Debitors = new List<string>() { p2d.DebitorUsername },
                        Author = p2d.AuthorUsername,
                        Price = p2d.Price,
                        PaymentDate = p2d.PaymentDate,
                        PaymentDescription = p2d.PaymentDescription,
                        UpdateTime = p2d.PaymentUpdateTime
                    };

                    result.Add(newPaymentDto);
                }
                else
                {
                    match.Debitors.Add(p2d.DebitorUsername);
                }
            }

            return result;
        }

        protected static JoinedPayment2DebitorDto ConvertDbToObject(SqlDataReader reader)
        {
            var paymentId = reader.SafeGetInt64("PId");

            var debitorId = reader.SafeGetInt64("UDebitorId");
            var debitorUsername = reader.SafeGetString("UDebitorUsername");

            var creditorId = reader.SafeGetInt64("UCreditorId");
            var creditorUsername = reader.SafeGetString("UCreditorUsername");

            var authorId = reader.SafeGetInt64("UAuthorId");
            var authorUsername = reader.SafeGetString("UAuthorUsername");

            var payment2DebitorIdFk = reader.SafeGetInt64("P2DId");

            var price = reader.SafeGetDecimal("PPrice");
            var paymentDate = reader.SafeGetDateTime("PPaymentDate");
            var paymentUpdateTime = reader.SafeGetDateTime("PUpdateTime");
            var paymentDescription = reader.SafeGetString("PDescription");

            return new JoinedPayment2DebitorDto()
            {
                PaymentId = paymentId!.Value,

                Payment2DebitorIdFk = payment2DebitorIdFk!.Value,

                DebitorId = debitorId!.Value,
                DebitorUsername = debitorUsername,

                CreditorId = creditorId!.Value,
                CreditorUsername = creditorUsername,

                AuthorId = authorId!.Value,
                AuthorUsername = authorUsername,

                Price = price!.Value,
                PaymentDate = paymentDate!.Value,
                PaymentUpdateTime = paymentUpdateTime!.Value,
                PaymentDescription = paymentDescription
            };
        }
    }
}
