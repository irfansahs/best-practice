namespace Domain.Identity;

public enum RefreshTokenRevokeReason
{
    Rotated = 0,
    Logout = 1,
    ReuseDetected = 2,
    SecurityStampChanged = 3,
    Admin = 4
}
