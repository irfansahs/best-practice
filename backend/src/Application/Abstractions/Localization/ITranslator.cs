namespace Application.Abstractions.Localization;

public interface ITranslator
{
    string this[string key] { get; }
    string Translate(string key, params object[] args);
}
