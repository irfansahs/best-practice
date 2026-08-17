using Domain.Localization.ValueObjects;

namespace Application.Abstractions.Localization;

public interface ICultureContext
{
    CultureCode Current { get; }
}
