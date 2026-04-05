public static class BlackjackAuthority
{
    public static bool IsHost()
    {
        if (!UserManager.TryGetUserID(out string userId))
        {
            return true;
        }

        if (!BCConfig.TryGetCurrentHostID(out string hostId))
        {
            return true;
        }

        return userId == hostId;
    }
}
