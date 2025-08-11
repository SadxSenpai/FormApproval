using FormApproval.Domain;

namespace FormApproval.Services;
public interface IFormRepository
{
    FormTemplate GetDefaultTemplate();
    FormInstance CreateDraft(FormInstance instance);
    FormInstance? Get(Guid id);
    IEnumerable<FormInstance> GetMine(string ownerUserId);
    IEnumerable<FormInstance> GetApproverQueue(bool isApprover);
    void Save(FormInstance instance);
}