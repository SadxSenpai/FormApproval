namespace FormApproval.Domain;

// Simple workflow statuses for a request.
public enum FormStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected
}