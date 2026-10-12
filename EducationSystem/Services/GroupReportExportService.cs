using ClosedXML.Excel;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Visualizations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ScottPlot;
using System.IO;

namespace EducationSystem.Services;

public sealed class GroupReportExportService
{
    public void ExportExcel(string filePath, IReadOnlyList<GroupEnrollmentRow> rows)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Контингент груп");
        int total = rows.Sum(x => x.StudentCount);

        ws.Cell("A1").Value = "EDUCATION SYSTEM — КОНТИНГЕНТ СТУДЕНТІВ";
        ws.Range("A1:D1").Merge();
        ws.Range("A1:D1").Style.Font.Bold = true;
        ws.Range("A1:D1").Style.Font.FontSize = 16;
        ws.Cell("A2").Value = "Статистика за активними групами";
        ws.Cell("A3").Value = "Сформовано";
        ws.Cell("B3").Value = DateTime.Now;
        ws.Cell("B3").Style.DateFormat.Format = "dd.mm.yyyy hh:mm";
        ws.Cell("A4").Value = "Активних груп";
        ws.Cell("B4").Value = rows.Count;
        ws.Cell("A5").Value = "Студентів";
        ws.Cell("B5").Value = total;
        ws.Cell("A6").Value = "Середня наповненість";
        ws.Cell("B6").FormulaA1 = "=IFERROR(B5/B4,0)";
        ws.Cell("B6").Style.NumberFormat.Format = "0.00";

        string[] headers = { "Група", "Студентів", "Частка, %", "ID групи" };
        for (int j = 0; j < headers.Length; j++)
            ws.Cell(8, j + 1).Value = headers[j];
        var header = ws.Range("A8:D8");
        header.Style.Fill.BackgroundColor = XLColor.FromHtml("#173B5E");
        header.Style.Font.FontColor = XLColor.White;
        header.Style.Font.Bold = true;

        int line = 9;
        foreach (var row in rows)
        {
            ws.Cell(line, 1).Value = row.GroupName;
            ws.Cell(line, 2).Value = row.StudentCount;
            ws.Cell(line, 3).FormulaA1 = $"=IFERROR(B{line}/$B$5,0)";
            ws.Cell(line, 3).Style.NumberFormat.Format = "0.0%";
            ws.Cell(line, 4).Value = row.Id;
            line++;
        }

        ws.Cell(line, 1).Value = "РАЗОМ";
        ws.Cell(line, 1).Style.Font.Bold = true;
        if (rows.Count > 0)
        {
            ws.Cell(line, 2).FormulaA1 = $"=SUM(B9:B{line - 1})";
            ws.Cell(line, 3).FormulaA1 = $"=SUM(C9:C{line - 1})";
            ws.Cell(line, 3).Style.NumberFormat.Format = "0.0%";
        }
        else
        {
            ws.Cell(line, 2).Value = 0;
            ws.Cell(line, 3).Value = 0;
        }
        ws.Range($"A{line}:D{line}").Style.Font.Bold = true;

        ws.Cell(line + 2, 1).Value =
            "Методика: враховано студентів, прикріплених до активних груп; порожні групи також показано.";
        ws.Range(line + 2, 1, line + 2, 4).Merge();
        ws.Range(line + 2, 1, line + 2, 4).Style.Alignment.WrapText = true;
        ws.Row(line + 2).Height = 32;
        ws.SheetView.FreezeRows(8);
        ws.Column(1).Width = 24;
        ws.Column(2).Width = 19;
        ws.Column(3).Width = 19;
        ws.Column(4).Width = 15;
        ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;

        if (rows.Count > 0)
        {
            string chartFile = CreateTemporaryChart(rows);
            try
            {
                ws.AddPicture(chartFile).MoveTo(ws.Cell("F8")).WithSize(780, 360);
                workbook.SaveAs(filePath);
            }
            finally { File.Delete(chartFile); }
        }
        else workbook.SaveAs(filePath);
    }

    public void ExportPdf(string filePath, IReadOnlyList<GroupEnrollmentRow> rows)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        string? chartFile = rows.Count == 0 ? null : CreateTemporaryChart(rows);
        try
        {
            int total = rows.Sum(x => x.StudentCount);
            Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(35);
                    page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(10));

                    page.Header().Column(header =>
                    {
                        header.Item().Text("EDUCATION SYSTEM").Bold().FontSize(15);
                        header.Item().Text("Аналітична звітність • Контингент студентів").FontSize(9);
                        header.Item().LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(15).Column(column =>
                    {
                        column.Spacing(12);
                        column.Item().Text("ЗВІТ ПРО НАПОВНЕНІСТЬ АКАДЕМІЧНИХ ГРУП")
                            .Bold().FontSize(15);
                        column.Item().Text($"Сформовано: {DateTime.Now:dd.MM.yyyy HH:mm}");
                        column.Item().Text($"Активних груп: {rows.Count}; студентів: {total}; " +
                            $"середня наповненість: {(rows.Count == 0 ? 0 : (double)total / rows.Count):0.00}");
                        if (chartFile is not null)
                            column.Item().Image(chartFile);

                        column.Item().Text("Деталізація за групами").Bold().FontSize(12);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Text("Група").Bold();
                                h.Cell().Text("Студентів").Bold();
                                h.Cell().Text("Частка").Bold();
                            });
                            foreach (var row in rows)
                            {
                                table.Cell().PaddingVertical(4).Text(row.GroupName);
                                table.Cell().PaddingVertical(4).Text(row.StudentCount.ToString());
                                table.Cell().PaddingVertical(4).Text($"{row.SharePercent:0.0}%");
                            }
                        });
                        column.Item().Text(
                            "Методика: враховано студентів, які зараз належать до активних груп. " +
                            "Історичні переведення між групами не реконструюються.")
                            .Italic().FontSize(9);
                    });

                    page.Footer().AlignRight().Text(t =>
                    {
                        t.Span("Сторінка ");
                        t.CurrentPageNumber();
                        t.Span(" із ");
                        t.TotalPages();
                    });
                });
            }).GeneratePdf(filePath);
        }
        finally { if (chartFile is not null) File.Delete(chartFile); }
    }

    private static string CreateTemporaryChart(IReadOnlyList<GroupEnrollmentRow> rows)
    {
        string path = Path.Combine(Path.GetTempPath(), $"edu-chart-{Guid.NewGuid():N}.png");
        var plot = new Plot();
        GroupEnrollmentChart.Populate(plot, rows);
        plot.SavePng(path, 1000, 460);
        return path;
    }
}
