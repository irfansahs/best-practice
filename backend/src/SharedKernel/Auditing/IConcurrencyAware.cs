namespace SharedKernel.Auditing;

public interface IConcurrencyAware
{
    byte[] RowVersion { get; set; }
}
