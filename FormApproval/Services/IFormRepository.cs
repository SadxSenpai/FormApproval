using FormApproval.Domain;

namespace FormApproval.Services;

// Abstraction for form persistence and queries used by pages/components.
public interface IFormRepository
{
    // Template metadata
    FormTemplate GetDefaultTemplate();
    IEnumerable<FormTemplate> GetTemplates();
    FormTemplate? GetTemplate(Guid id);

    // Instances
    FormInstance CreateDraft(FormInstance instance);
    FormInstance? Get(Guid id);
    IEnumerable<FormInstance> GetMine(string ownerUserId);
    IEnumerable<FormInstance> GetApproverQueue(bool isApprover);
    void Save(FormInstance instance);
}