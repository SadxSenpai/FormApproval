using FormApproval.Domain;

namespace FormApproval.Services;

public interface IPdfService
{
    // Generates a PTO PDF for the given instance and saves it under wwwroot/pdfs/{Id}.pdf
    // Returns the relative URL (e.g. /pdfs/{Id}.pdf) to be shown in the UI.
    string SavePtoPdf(FormInstance instance);
}