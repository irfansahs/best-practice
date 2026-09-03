namespace Domain.Identity;

public enum ClientType
{
    Web = 1,
    Mobile = 2
}

[Flags]
public enum ClientTypes
{
    None = 0,
    Web = 1,
    Mobile = 2,
    All = Web | Mobile
}
