namespace FormApproval.Domain;

public class FormInstance
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TemplateId { get; set; }
    public string OwnerUserId { get; set; } = default!;
    public string OwnerName { get; set; } = default!;
    public string OwnerEmail { get; set; } = default!;
    public string? Department { get; set; }
    public FormStatus Status { get; set; } = FormStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public Dictionary<string, string?> Answers { get; set; } = new();
    public List<AuditEntry> Audit { get; set; } = new();
}

public class AuditEntry
{
    public DateTime At { get; set; } = DateTime.UtcNow;
    public string Actor { get; set; } = default!;
    public string Action { get; set; } = default!;
    public string? Comment { get; set; }
}