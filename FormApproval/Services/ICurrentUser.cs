namespace FormApproval.Services;
public interface ICurrentUser
{
    string UserId { get; }
    string Name { get; }
    string Email { get; }
    string Department { get; }
    bool IsApprover { get; }
}