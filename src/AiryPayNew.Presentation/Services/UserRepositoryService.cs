using Discord;

namespace AiryPayNew.Presentation.Services;

public class UserRepositoryService
{
    private readonly Dictionary<ulong, IUser> _socketUsers = new();
    
    public void SetUser(IUser user)
    {
        _socketUsers[user.Id] = user;
    }
    
    public IUser? GetUser(ulong id)
    {
        _socketUsers.TryGetValue(id, out var user);
        return user;
    }
}