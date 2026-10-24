using Microsoft.Extensions.Logging;
using PaymentBackend.Common.Model.Dto;
using PaymentBackend.Settings;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace PaymentBackend.Database.DatabaseServices
{
    public abstract class AbstractPaymentDatabaseService : AbstractDatabaseService
    {
        protected AbstractPaymentDatabaseService(
            IFunctionSettingsResolver functionSettingsResolver,
            ILogger logger
        ) 
            : base(functionSettingsResolver, logger)
        {
        }

        protected async Task<List<JoinedPayment2DebitorDto>> SelectAllPayment2DebitorsAsync(SqlConnection connection, long paymentContext, CancellationToken cancellationToken)
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
	w.Username as UAuthorUsername,
    ctx.IsClosed as CtxIsClosed
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
join PaymentContext ctx
    on ctx.Id = p.PaymentContextIdFk
where 
    p.IsDeleted = 0
	and p.PaymentContextIdFk = @PaymentContextIdFk
order by 
    p.PaymentDate asc
";

            await using SqlCommand cmd1 = new(sql, connection);
            cmd1.CommandType = CommandType.Text;

            cmd1.Parameters.AddWithValue("@PaymentContextIdFk", paymentContext);

            List<JoinedPayment2DebitorDto> result = [];

            await using SqlDataReader reader = await cmd1.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected async Task<List<JoinedPayment2DebitorDto>> SelectPaymentByIdAsync(SqlConnection connection, long paymentContext, long id, CancellationToken cancellationToken)
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
	w.Username as UAuthorUsername,
    ctx.IsClosed as CtxIsClosed
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
join PaymentContext ctx
    on ctx.Id = p.PaymentContextIdFk
where
    p.IsDeleted = 0
	and p.PaymentContextIdFk = @PaymentContextIdFk
order by 
    p.PaymentDate asc
";

            await using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@PaymentId", id);
            cmd.Parameters.AddWithValue("@PaymentContextIdFk", paymentContext);

            List<JoinedPayment2DebitorDto> result = [];

            await using SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected async Task<List<JoinedPayment2DebitorDto>> SelectPaymentsByCreditorAsync(SqlConnection connection, long paymentContext, string username, CancellationToken cancellationToken)
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
	a.Username as UAuthorUsername,
    ctx.IsClosed as CtxIsClosed
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
join PaymentContext ctx
    on ctx.Id = p.PaymentContextIdFk
where 
    p.IsDeleted = 0
	and p.PaymentContextIdFk = @PaymentContextIdFk
order by 
    p.PaymentDate asc
";

            await using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@PaymentContextIdFk", paymentContext);

            List<JoinedPayment2DebitorDto> result = [];

            await using SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected async Task<List<JoinedPayment2DebitorDto>> SelectPaymentsByDebitorAsync(SqlConnection connection, long paymentContext, string username, CancellationToken cancellationToken)
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
	a.Username as UAuthorUsername,
    ctx.IsClosed as CtxIsClosed
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
join PaymentContext ctx
    on ctx.Id = p.PaymentContextIdFk
where
    p.IsDeleted = 0
	and p.PaymentContextIdFk = @PaymentContextIdFk
order by 
    p.PaymentDate asc
";

            await using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@PaymentContextIdFk", paymentContext);

            List<JoinedPayment2DebitorDto> result = [];

            await using SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected async Task<List<JoinedPayment2DebitorDto>> SelectPaymentsByAuthorAsync(SqlConnection connection, long paymentContext, string username, CancellationToken cancellationToken)
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
	a.Username as UAuthorUsername,
    ctx.IsClosed as CtxIsClosed
from
	a
join Payments p 
	on p.AuthorIdFk = a.Id
join Payment2Debitor p2d
	on p2d.PaymentIdFk = p.Id
join PaymentUsers d
	on p2d.DebitorIdFk = d.Id 
join PaymentUsers c
	on p.CreditorIdFk = c.Id
join PaymentContext ctx
    on ctx.Id = p.PaymentContextIdFk
where
    p.IsDeleted = 0
	and p.PaymentContextIdFk = @PaymentContextIdFk
order by 
    p.PaymentDate asc
";

            await using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@PaymentContextIdFk", paymentContext);

            List<JoinedPayment2DebitorDto> result = [];

            await using SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                JoinedPayment2DebitorDto joinedPayment2Debitor = ConvertDbToObject(reader);
                result.Add(joinedPayment2Debitor);
            }

            return result;
        }

        protected async Task<long> MarkPaymentAsDeletedAsync(SqlConnection connection, long paymentContext, long paymentId, CancellationToken cancellationToken)
        {
            string sql = @"
update
    Payments
set
    IsDeleted = 1
where
    Id = @Id
	and PaymentContextIdFk = @PaymentContextIdFk
";

            await using SqlCommand cmd = new(sql, connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Id", paymentId);
            cmd.Parameters.AddWithValue("@PaymentContextIdFk", paymentContext);

            return await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        protected List<FullPaymentDto> MergeJoinedPayments(List<JoinedPayment2DebitorDto> joinedPayments)
        {
            List<FullPaymentDto> result = [];

            bool? firstIsClosedValue = null;
            foreach (var p2d in joinedPayments)
            {
                var match = result.Find(payment => payment.Id == p2d.PaymentId);

                if (match == null)
                {
                    FullPaymentDto newPaymentDto = new FullPaymentDto
                    {
                        Id = p2d.PaymentId,
                        Creditor = p2d.CreditorUsername,
                        Debitors = [p2d.DebitorUsername],
                        Author = p2d.AuthorUsername,
                        Price = p2d.Price,
                        PaymentDate = p2d.PaymentDate,
                        PaymentDescription = p2d.PaymentDescription,
                        UpdateTime = p2d.PaymentUpdateTime,

                        PaymentContextIsClosed = p2d.PaymentContextIsClosed
                    };

                    result.Add(newPaymentDto);
                }
                else
                {
                    if (match.PaymentContextIsClosed != p2d.PaymentContextIsClosed)
                    {
                        throw new UnreachableException("Payments with different context detected.");
                    }

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

            var contextIsClosed= reader.SafeGetInt16("CtxIsClosed");

            return new JoinedPayment2DebitorDto
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
                PaymentDescription = paymentDescription,

                PaymentContextIsClosed = contextIsClosed is 1
            };
        }
    }
}
