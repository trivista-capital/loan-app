namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class ClientApiKeyConfiguration : BaseEntity<Guid>
{
    internal ClientApiKeyConfiguration() { }

    private ClientApiKeyConfiguration(string email, string apiKey)
    {
        Id = Guid.NewGuid();
        Email = email;
        ApiKey = apiKey;
        Created = DateTime.UtcNow;
    }

    public string Email { get; set; }

    public string ApiKey { get; set; }

    public class Factory
    {
        public static ClientApiKeyConfiguration Build(string email, string apiKey)
            => new ClientApiKeyConfiguration(email, apiKey);
    }

    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}