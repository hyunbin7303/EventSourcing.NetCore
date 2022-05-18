using Core.Commands;
using Core.Requests;
using MediatR;

namespace Orders.Payments.RequestingPayment;

public class RequestPayment: ICommand
{
    public Guid OrderId { get; }

    public decimal Amount { get; }

    private RequestPayment(Guid orderId, decimal amount)
    {
        OrderId = orderId;
        Amount = amount;
    }

    public static RequestPayment Create(Guid orderId, decimal amount)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        return new RequestPayment(orderId, amount);
    }
}

public class HandleRequestPayment:
    ICommandHandler<RequestPayment>
{
    private readonly ExternalServicesConfig externalServicesConfig;
    private readonly IHttpExternalCommandBus httpExternalCommandBus;

    public HandleRequestPayment(ExternalServicesConfig externalServicesConfig,
        IHttpExternalCommandBus httpExternalCommandBus)
    {
        this.externalServicesConfig = externalServicesConfig;
        this.httpExternalCommandBus = httpExternalCommandBus;
    }

    public async Task<Unit> Handle(RequestPayment command, CancellationToken cancellationToken)
    {
        await httpExternalCommandBus.Post(
            externalServicesConfig.PaymentsUrl!,
            "payments",
            command,
            cancellationToken);

        return Unit.Value;
    }
}