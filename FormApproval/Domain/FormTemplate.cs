namespace FormApproval.Domain;

public class FormTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Leave Request";
    public List<FormField> Fields { get; set; } = new();
}

public class FormField
{
    public string Key { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Type { get; set; } = "text"; // text/date/textarea
    public bool Required { get; set; } = true;
    public int Order { get; set; }
}