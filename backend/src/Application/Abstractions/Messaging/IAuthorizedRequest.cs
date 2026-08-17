namespace Application.Abstractions.Messaging;

public interface IAuthorizedRequest
{
    string Permission { get; }
}
