using FormApproval.Domain;

namespace FormApproval.Services
{
    public class FormServices
    {
        private readonly List<FormInstance> forms = new();

        public IEnumerable<FormInstance> GetAll() => forms;

        public void Add(FormInstance form)
        {
            forms.Add(form);
        }
        public FormInstance? GetById(Guid id) => forms.FirstOrDefault(f => f.Id == id);
    }
}
