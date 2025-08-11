using FormApproval.Domain;

namespace FormApproval.Services;

public class InMemoryFormRepository : IFormRepository
{
    private readonly List<FormInstance> _forms = new();
    private readonly FormTemplate _template;

    public InMemoryFormRepository()
    {
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

    public FormTemplate GetDefaultTemplate() => _template;
    public FormInstance CreateDraft(FormInstance instance) { _forms.Add(instance); return instance; }
    public FormInstance? Get(Guid id) => _forms.FirstOrDefault(f => f.Id == id);
    public IEnumerable<FormInstance> GetMine(string ownerUserId) => _forms.Where(f => f.OwnerUserId == ownerUserId).OrderByDescending(f => f.CreatedAt);
    public IEnumerable<FormInstance> GetApproverQueue(bool isApprover) => isApprover ? _forms.Where(f => f.Status == FormStatus.Submitted) : Enumerable.Empty<FormInstance>();
    public void Save(FormInstance instance) { /* noop */ }
}