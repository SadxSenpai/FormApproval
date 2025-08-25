using System.Globalization;
using FormApproval.Domain;
using Microsoft.AspNetCore.Hosting;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FormApproval.Services;

public class PdfService : IPdfService
{
    private readonly IWebHostEnvironment _env;

    public PdfService(IWebHostEnvironment env) => _env = env;

    public string SavePtoPdf(FormInstance instance)
    {
        var bytes = BuildPtoPdf(instance);

        var webRoot = _env.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
        var folder = Path.Combine(webRoot, "pdfs");
        Directory.CreateDirectory(folder);

        var fileName = $"{instance.Id}.pdf";
        var fullPath = Path.Combine(folder, fileName);
        File.WriteAllBytes(fullPath, bytes);

        return $"/pdfs/{fileName}";
    }

    private static byte[] BuildPtoPdf(FormInstance inst)
    {
        var a = inst.Answers;

        // Deutsch als Format
        var de = CultureInfo.GetCultureInfo("de-DE");

        // kleine Helfer
        string D(DateTime? d) => d.HasValue ? d.Value.ToString("dd.MM.yyyy", de) : "";
        string N(int? n) => n.HasValue ? n.Value.ToString(de) : "";

        DateTime? ParseDate(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateTime.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d1))
                return d1;
            if (DateTime.TryParse(s, de, DateTimeStyles.None, out var d2))
                return d2;
            return null;
        }

        int? ParseInt(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (int.TryParse(s, NumberStyles.Integer, de, out var n)) return n;
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) return n;
            return null;
        }

        // Map inputs from Answers
        var isUrlaub = string.Equals(a.GetValueOrDefault("Antragstyp"), "urlaub", StringComparison.OrdinalIgnoreCase);

        var name = a.GetValueOrDefault("Nachname");
        var vorname = a.GetValueOrDefault("Vorname");
        var abteilung = a.GetValueOrDefault("Abteilung");
        var geburtsdatum = ParseDate(a.GetValueOrDefault("Geburtsdatum"));

        var anspruchAktuellesJahr = ParseInt(a.GetValueOrDefault("AnspruchAktuellesJahr"));
        var anspruchZusatz = ParseInt(a.GetValueOrDefault("AnspruchZusatz"));
        var anspruchGesamt = ParseInt(a.GetValueOrDefault("AnspruchGesamt"));

        var tageGesamt = ParseInt(a.GetValueOrDefault("TageGesamt"));
        var restanspruch = ParseInt(a.GetValueOrDefault("Restanspruch"));

        var periods = new[]
        {
            new { Days = ParseInt(a.GetValueOrDefault("Tage1")), From = ParseDate(a.GetValueOrDefault("Von1")), To = ParseDate(a.GetValueOrDefault("Bis1")) },
            new { Days = ParseInt(a.GetValueOrDefault("Tage2")), From = ParseDate(a.GetValueOrDefault("Von2")), To = ParseDate(a.GetValueOrDefault("Bis2")) },
            new { Days = ParseInt(a.GetValueOrDefault("Tage3")), From = ParseDate(a.GetValueOrDefault("Von3")), To = ParseDate(a.GetValueOrDefault("Bis3")) }
        };

        // Styles
        var label = TextStyle.Default.FontSize(9).Bold().FontColor(Colors.Grey.Darken2);
        var value = TextStyle.Default.FontSize(11);
        var title = TextStyle.Default.FontSize(18).Bold();
        var sectionTitle = TextStyle.Default.FontSize(11).Bold().FontColor(Colors.Grey.Darken2);

        byte[] pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2.2f, Unit.Centimetre);
                page.DefaultTextStyle(TextStyle.Default.FontFamily(Fonts.Arial));

                page.Content().Column(col =>
                {
                    // Titel
                    col.Item().Text("Antrag auf Urlaub / Arbeitsbefreiung").Style(title);
                    col.Item().PaddingBottom(8).Text("Bitte ausfüllen. Felder mit * sind Pflichtfeldern.")
                        .FontSize(9).FontColor(Colors.Grey.Darken2);

                    // Antragstyp
                    col.Item().Element(e => SectionBox(e, "Antragstyp *", inner =>
                    {
                        inner.Item().Row(r =>
                        {
                            r.RelativeItem().Element(c => CheckboxLine(c, "Antrag auf Urlaub", isUrlaub));
                            r.RelativeItem().Element(c => CheckboxLine(c, "Antrag auf Arbeitsbefreiung / Freistellung", !isUrlaub));
                        });
                    }));

                    // Person
                    col.Item().Element(e => SectionBox(e, "Person", inner =>
                    {
                        inner.Item().Row(r =>
                        {
                            r.RelativeItem().Element(c => LabeledInput(c, "Name *", name));
                            r.RelativeItem().Element(c => LabeledInput(c, "Vorname *", vorname));
                        });
                        inner.Item().Height(6);
                        inner.Item().Row(r =>
                        {
                            r.RelativeItem().Element(c => LabeledInput(c, "Abteilung *", abteilung));
                            r.RelativeItem().Element(c => LabeledInput(c, "Geburtsdatum", D(geburtsdatum)));
                        });
                    }));

                    // Anspruch
                    col.Item().Element(e => SectionBox(e, "ANSPRUCH", inner =>
                    {
                        inner.Item().Row(r =>
                        {
                            r.RelativeItem().Element(c =>
                                LabeledInput(c, "Urlaubsanspruch (verbleibender Anspruch lfd. Jahr) — Tage",
                                    N(anspruchAktuellesJahr)));

                            r.RelativeItem().Element(c =>
                                LabeledInput(c, "Zusätzlicher Urlaub (Azubi mit GdB ≥ 50) — Tage",
                                    N(anspruchZusatz)));

                            r.RelativeItem().Element(c =>
                                LabeledInput(c, "Gesamtanspruch — Tage",
                                    N(anspruchGesamt)));
                        });
                    }));

                    // Zeiträume (bis zu 3)
                    col.Item().Element(e => SectionBox(e, "Urlaubszeiträume", inner =>
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            var p = periods[i];
                            inner.Item().Row(r =>
                            {
                                r.RelativeItem().Element(c => LabeledInput(c, "Tage Urlaub", N(p.Days)));
                                r.RelativeItem().Element(c => LabeledInput(c, "vom", D(p.From)));
                                r.RelativeItem().Element(c => LabeledInput(c, "bis", D(p.To)));
                            });
                            inner.Item().Height(6);
                        }
                    }));

                    // Summen
                    col.Item().Element(e => SectionBox(e, "Summen", inner =>
                    {
                        inner.Item().Row(r =>
                        {
                            r.RelativeItem().Element(c => LabeledInput(c, "Tage gesamt", N(tageGesamt)));
                            r.RelativeItem().Element(c => LabeledInput(c, "verbleibender Anspruch =", N(restanspruch)));
                        });
                    }));

                    // Hinweis (wie im Formular, ohne Unterschriften-Teil)
                    col.Item().PaddingTop(4).Text("Der restliche Urlaubsanspruch verfällt zum 31.12. des laufenden Jahres.")
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                });

                // keine Fußzeile nötig
            });
        }).GeneratePdf();

        return pdf;

        // ----- lokale UI-Helfer -----

        // NEW signature to work with ColumnDescriptor correctly
        void SectionBox(IContainer parent, string caption, Action<ColumnDescriptor> content)
        {
            parent.Element(e =>
            {
                e.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(c =>
                {
                    // put padding on the item (not after Element)
                    c.Item().PaddingBottom(6).Text(caption).Style(sectionTitle);

                    // let the caller add rows/items inside this section
                    content(c);
                });
            });
        }

        void LabeledInput(IContainer parent, string caption, string? val)
        {
            parent.Column(c =>
            {
                c.Item().Text(caption).Style(label);
                c.Item().PaddingTop(2).BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                    .PaddingBottom(2).Text(val ?? "").Style(value);
            });
        }

        void CheckboxLine(IContainer parent, string text, bool isChecked)
        {
            parent.Row(r =>
            {
                r.ConstantItem(12).Height(12).Border(1).BorderColor(Colors.Grey.Darken1)
                    .AlignCenter().AlignMiddle().Text(isChecked ? "X" : "");
                r.ConstantItem(6);
                r.RelativeItem().AlignMiddle().Text(text).Style(value);
            });
        }
    }
}