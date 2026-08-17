namespace SharedKernel.Guards;

public static class Guard
{
    public static T AgainstNull<T>(T? value, string parameterName) where T : class =>
        value ?? throw new ArgumentNullException(parameterName);

    public static string AgainstNullOrWhiteSpace(string? value, string parameterName) =>
        !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Value cannot be null or whitespace.", parameterName);

    public static IEnumerable<T> AgainstNullOrEmpty<T>(IEnumerable<T>? value, string parameterName)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);

        var materialized = value as ICollection<T> ?? value.ToList();
        if (materialized.Count == 0)
            throw new ArgumentException("Collection cannot be empty.", parameterName);

        return materialized;
    }

    public static decimal AgainstNegative(decimal value, string parameterName) =>
        value >= 0 ? value : throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");

    public static int AgainstNegative(int value, string parameterName) =>
        value >= 0 ? value : throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");

    public static int AgainstNegativeOrZero(int value, string parameterName) =>
        value > 0 ? value : throw new ArgumentOutOfRangeException(parameterName, value, "Value must be greater than zero.");
}
