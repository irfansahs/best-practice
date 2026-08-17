using Application.Abstractions.Localization;
using Domain.Localization.ValueObjects;

namespace Application.UnitTests.Helpers;

public sealed class FakeCultureContext(CultureCode? culture = null) : ICultureContext
{
    public CultureCode Current { get; } = culture ?? CultureCode.Default;
}
