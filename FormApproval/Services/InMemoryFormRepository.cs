using FormApproval.Domain;

namespace FormApproval.Services;

// Simple in-memory repository for demo purposes.
// - Stores form instances in a List for the lifetime of the app.
// - GetMine and GetApproverQueue provide filtered views for dashboard screens.
public class InMemoryFormRepository : IFormRepository
{
    private readonly List<FormInstance> _forms = new();
    private readonly FormTemplate _template;

    public InMemoryFormRepository()
    {
        // A static template describing expected fields (metadata only in this demo).
        _template = new FormTemplate
        {
            Name = "Leave Request",
            Fields = new() {
                new FormField { Key="FullName", Label="Full Name", Type="text", Order=0 },
                new FormField { Key="Email", Label="Email", Type="text", Order=1 },
                new FormField { Key="Department", Label="Department", Type="text", Order=2 },
                new FormField { Key="FromDate", Label="From", Type="date", Order=3 },
                new FormField { Key="ToDate", Label="To", Type="date", Order=4 },
                new FormField { Key="Reason", Label="Reason", Type="textarea", Order=5 },
            }
        };
    }

    // Return the single default template.
    public FormTemplate GetDefaultTemplate() => _template;

    // Add a new instance (used for both Draft and Submitted in this demo).
    public FormInstance CreateDraft(FormInstance instance) { _forms.Add(instance); return instance; }

    // Find a form instance by Id.
    public FormInstance? Get(Guid id) => _forms.FirstOrDefault(f => f.Id == id);

    // All forms owned by a user, newest first (any status).
    public IEnumerable<FormInstance> GetMine(string ownerUserId) =>
        _forms.Where(f => f.OwnerUserId == ownerUserId).OrderByDescending(f => f.CreatedAt);

    // Approver queue: only Submitted items are actionable.
    public IEnumerable<FormInstance> GetApproverQueue(bool isApprover) =>
        isApprover ? _forms.Where(f => f.Status == FormStatus.Submitted) : Enumerable.Empty<FormInstance>();

    // No persistence layer here; instances are mutated in-place in memory.
    public void Save(FormInstance instance) { /* noop */ }
}