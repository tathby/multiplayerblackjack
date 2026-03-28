public static class BlackjackAuthority
{
    public static bool IsHost()
    {
        return UserManager.GetUserID() == BCConfig.GetCurrentHostID();
    }
}
