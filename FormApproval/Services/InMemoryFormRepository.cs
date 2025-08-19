using FormApproval.Domain;

namespace FormApproval.Services;

// Simple in-memory repository for demo purposes.
public class InMemoryFormRepository : IFormRepository
{
    private readonly List<FormInstance> _forms = new();

    // Multiple templates supported
    private readonly List<FormTemplate> _templates;

    public InMemoryFormRepository()
    {
        // Seed several templates. You can refine fields per type as needed.
        var leave = new FormTemplate
        {
            Name = "Paid Time Off (PTO)",
            Fields = new() {
                new FormField { Key="FullName", Label="Full Name", Type="text", Order=0 },
                new FormField { Key="Email", Label="Email", Type="text", Order=1 },
                new FormField { Key="Department", Label="Department", Type="text", Order=2 },
                new FormField { Key="FromDate", Label="From", Type="date", Order=3 },
                new FormField { Key="ToDate", Label="To", Type="date", Order=4 },
                new FormField { Key="Reason", Label="Reason", Type="textarea", Order=5 },
            }
        };

        var earlyLeave = new FormTemplate
        {
            Name = "Overtime Reduction (Early Leave)",
            Fields = new() {
                new FormField { Key="FullName", Label="Full Name", Type="text", Order=0 },
                new FormField { Key="Email", Label="Email", Type="text", Order=1 },
                new FormField { Key="Department", Label="Department", Type="text", Order=2 },
                new FormField { Key="FromDate", Label="Date", Type="date", Order=3 },
                new FormField { Key="ToDate", Label="(Optional) Second Day", Type="date", Required=false, Order=4 },
                new FormField { Key="Reason", Label="Reason", Type="textarea", Order=5 },
            }
        };

        _templates = new() { leave, earlyLeave };
    }

    // Template APIs
    public FormTemplate GetDefaultTemplate() => _templates.First();
    public IEnumerable<FormTemplate> GetTemplates() => _templates;
    public FormTemplate? GetTemplate(Guid id) => _templates.FirstOrDefault(t => t.Id == id);

    // Instance APIs
    public FormInstance CreateDraft(FormInstance instance) { _forms.Add(instance); return instance; }
    public FormInstance? Get(Guid id) => _forms.FirstOrDefault(f => f.Id == id);
    public IEnumerable<FormInstance> GetMine(string ownerUserId) =>
        _forms.Where(f => f.OwnerUserId == ownerUserId).OrderByDescending(f => f.CreatedAt);
    public IEnumerable<FormInstance> GetApproverQueue(bool isApprover) =>
        isApprover ? _forms.Where(f => f.Status == FormStatus.Submitted) : Enumerable.Empty<FormInstance>();
    public void Save(FormInstance instance) { /* noop */ }
}