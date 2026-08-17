namespace SharedKernel.Primitives;

public readonly record struct StronglyTypedId<TValue>(TValue Value) where TValue : notnull
{
    public override readonly string ToString() => Value.ToString() ?? string.Empty;
}
