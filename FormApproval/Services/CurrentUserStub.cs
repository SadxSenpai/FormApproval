namespace FormApproval.Services;

public class CurrentUserStub : ICurrentUser
{
    public bool IsApprover { get; set; } = false; // flip for testing
    public string UserId => IsApprover ? "approver-1" : "user-1";
    public string Name => IsApprover ? "Alex Approver" : "Sam Submitter";
    public string Email => IsApprover ? "alex.approver@demo" : "sam.submitter@demo";
    public string Department => IsApprover ? "Management" : "Engineering";
}