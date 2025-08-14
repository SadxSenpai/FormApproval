using FormApproval.Domain;

namespace FormApproval.Services;

// Abstraction for form persistence and queries used by pages/components.
public interface IFormRepository
{
    // Template metadata for the default form.
    FormTemplate GetDefaultTemplate();

    // Create and store a new instance (Draft or Submitted).
    FormInstance CreateDraft(FormInstance instance);

    // Fetch a specific instance by Id.
    FormInstance? Get(Guid id);

    // All forms for a specific owner (used by "My Submissions").
    IEnumerable<FormInstance> GetMine(string ownerUserId);

    // Items the approver can act on (used by "My Approvals").
    IEnumerable<FormInstance> GetApproverQueue(bool isApprover);

    // Persist changes to an existing instance (noop in-memory).
    void Save(FormInstance instance);
}