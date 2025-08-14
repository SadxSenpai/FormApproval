namespace FormApproval.Domain;

// A single form submission instance (Draft/Submitted/Approved/Rejected).
public class FormInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Links the instance to a form template (metadata).
    public Guid TemplateId { get; set; }

    // Owner information (used to filter and display in UI).
    public string OwnerUserId { get; set; } = default!;
    public string OwnerName { get; set; } = default!;
    public string OwnerEmail { get; set; } = default!;
    public string? Department { get; set; }

    // Workflow status and timestamps.
    public FormStatus Status { get; set; } = FormStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }

    // Answers keyed by field keys (for flexible display).
    public Dictionary<string, string?> Answers { get; set; } = new();

    // Audit trail of actions taken on this instance.
    public List<AuditEntry> Audit { get; set; } = new();
}

// A single audit event (Create/Submit/Approve/Reject/etc).
public class AuditEntry
{
    public DateTime At { get; set; } = DateTime.UtcNow;
    public string Actor { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string? Comment { get; set; }
}