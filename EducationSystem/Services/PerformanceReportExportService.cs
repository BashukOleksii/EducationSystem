using ClosedXML.Excel;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Visualizations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ScottPlot;
using System.IO;

namespace EducationSystem.Services;

public sealed class PerformanceReportExportService
{
    public void ExportExcel(string filePath, PerformanceReportData data)
    {
        using var workbook = new XLWorkbook();
        var summary = workbook.Worksheets.Add("Зведення");

        summary.Cell("A1").Value = "EDUCATION SYSTEM — ЗВІТ ПРО УСПІШНІСТЬ";
        summary.Range("A1:G1").Merge();
        summary.Range("A1:G1").Style.Font.Bold = true;
        summary.Range("A1:G1").Style.Font.FontSize = 16;
        summary.Cell("A2").Value = $"Група: {data.GroupLabel}; період: {data.PeriodLabel}";
        summary.Range("A2:G2").Merge();
        summary.Cell("A3").Value = "Сформовано";
        summary.Cell("B3").Value = DateTime.Now;
        summary.Cell("B3").Style.DateFormat.Format = "dd.mm.yyyy hh:mm";

        summary.Cell("A4").Value = "Груп";
        summary.Cell("B4").Value = data.Groups.Count;
        summary.Cell("C4").Value = "Студентів";
        summary.Cell("D4").Value = data.TotalStudents;
        summary.Cell("E4").Value = "Оцінок";
        summary.Cell("F4").Value = data.TotalGrades;
        summary.Cell("A5").Value = "Мають оцінки";
        summary.Cell("B5").Value = data.GradedStudents;
        summary.Cell("C5").Value = "Середній бал";
        if (data.AverageGrade.HasValue)
            summary.Cell("D5").Value = data.AverageGrade.Value;
        summary.Cell("D5").Style.NumberFormat.Format = "0.00";
        summary.Cell("E5").Value = "Низьких (1–3)";
        summary.Cell("F5").Value = data.LowGradeCount;

        WriteHeader(summary, 8,
            "Група", "Студентів", "З оцінками", "Без оцінок", "Оцінок", "Середній бал", "Низьких (1–3)");

        int row = 9;
        foreach (var item in data.Groups)
        {
            summary.Cell(row, 1).Value = item.GroupName;
            summary.Cell(row, 2).Value = item.StudentCount;
            summary.Cell(row, 3).Value = item.GradedStudents;
            summary.Cell(row, 4).Value = item.WithoutGrades;
            summary.Cell(row, 5).Value = item.GradeCount;
            if (item.AverageGrade.HasValue)
                summary.Cell(row, 6).Value = item.AverageGrade.Value;
            summary.Cell(row, 6).Style.NumberFormat.Format = "0.00";
            summary.Cell(row, 7).Value = item.LowGradeCount;
            row++;
        }

        summary.Cell(row, 1).Value = "РАЗОМ";
        summary.Range(row, 1, row, 7).Style.Font.Bold = true;
        for (int col = 2; col <= 5; col++)
        {
            string letter = ((char)('A' + col - 1)).ToString();
            summary.Cell(row, col).FormulaA1 = data.Groups.Count == 0
                ? "=0"
                : $"=SUM({letter}9:{letter}{row - 1})";
        }
        if (data.TotalGrades > 0)
            summary.Cell(row, 6).FormulaA1 = $"=IFERROR(SUMPRODUCT(E9:E{row - 1},F9:F{row - 1})/E{row},0)";
        for (int col = 7; col <= 7; col++)
            summary.Cell(row, col).FormulaA1 = data.Groups.Count == 0 ? "=0" : $"=SUM(G9:G{row - 1})";
        summary.Cell(row, 6).Style.NumberFormat.Format = "0.00";

        var studentSheet = workbook.Worksheets.Add("Студенти");
        studentSheet.Cell("A1").Value = $"Успішність студентів — {data.GroupLabel}";
        studentSheet.Range("A1:F1").Merge();
        studentSheet.Range("A1:F1").Style.Font.Bold = true;
        WriteHeader(studentSheet, 3,
            "Група", "Студент", "Оцінок", "Середній бал", "Низьких (1–3)", "Високих (10–12)");

        int studentRow = 4;
        foreach (var student in data.Students)
        {
            studentSheet.Cell(studentRow, 1).Value = student.GroupName;
            studentSheet.Cell(studentRow, 2).Value = student.FullName;
            studentSheet.Cell(studentRow, 3).Value = student.GradeCount;
            if (student.AverageGrade.HasValue)
                studentSheet.Cell(studentRow, 4).Value = student.AverageGrade.Value;
            studentSheet.Cell(studentRow, 4).Style.NumberFormat.Format = "0.00";
            studentSheet.Cell(studentRow, 5).Value = student.LowGradeCount;
            studentSheet.Cell(studentRow, 6).Value = student.HighGradeCount;
            studentRow++;
        }

        var distribution = workbook.Worksheets.Add("Розподіл оцінок");
        distribution.Cell("A1").Value = "Розподіл оцінок за 12-бальною шкалою";
        distribution.Range("A1:C1").Merge();
        distribution.Range("A1:C1").Style.Font.Bold = true;
        WriteHeader(distribution, 3, "Бал", "Кількість", "Частка");
        int distRow = 4;
        foreach (var grade in data.Distribution)
        {
            distribution.Cell(distRow, 1).Value = grade.Grade;
            distribution.Cell(distRow, 2).Value = grade.Count;
            distribution.Cell(distRow, 3).FormulaA1 = $"=IFERROR(B{distRow}/{data.TotalGrades},0)";
            distribution.Cell(distRow, 3).Style.NumberFormat.Format = "0.0%";
            distRow++;
        }

        string? groupChart = null;
        string? gradesChart = null;
        try
        {
            if (data.TotalGrades > 0)
            {
                groupChart = SaveChart(plot => PerformanceCharts.PopulateGroupAverages(plot, data));
                gradesChart = SaveChart(plot => PerformanceCharts.PopulateGradeDistribution(plot, data));
                summary.AddPicture(groupChart).MoveTo(summary.Cell("I8")).WithSize(700, 330);
                distribution.AddPicture(gradesChart).MoveTo(distribution.Cell("E3")).WithSize(700, 330);
            }

            summary.SheetView.FreezeRows(8);
            studentSheet.SheetView.FreezeRows(3);
            distribution.SheetView.FreezeRows(3);
            summary.Columns("A:G").Width = 18;
            summary.Column("A").Width = 22;
            studentSheet.Columns("A:F").Width = 19;
            studentSheet.Column("B").Width = 30;
            distribution.Columns("A:C").Width = 18;
            summary.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            studentSheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;

            workbook.SaveAs(filePath);
        }
        finally
        {
            if (groupChart is not null) File.Delete(groupChart);
            if (gradesChart is not null) File.Delete(gradesChart);
        }
    }

    public void ExportPdf(string filePath, PerformanceReportData data)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        string? groupChart = null;
        string? gradesChart = null;
        try
        {
            if (data.TotalGrades > 0)
            {
                groupChart = SaveChart(plot => PerformanceCharts.PopulateGroupAverages(plot, data));
                gradesChart = SaveChart(plot => PerformanceCharts.PopulateGradeDistribution(plot, data));
            }

            Document.Create(document =>
            {
                document.Page(page =>
                {
                    ConfigurePage(page);
                    page.Content().PaddingVertical(12).Column(column =>
                    {
                        column.Spacing(10);
                        column.Item().Text("ЗВІТ ПРО УСПІШНІСТЬ АКАДЕМІЧНИХ ГРУП")
                            .Bold().FontSize(15);
                        column.Item().Text($"Група: {data.GroupLabel} | Період: {data.PeriodLabel}");
                        column.Item().Text($"Груп: {data.Groups.Count}; студентів: {data.TotalStudents}; " +
                            $"із оцінками: {data.GradedStudents}; оцінок: {data.TotalGrades}; " +
                            $"середній бал: {FormatScore(data.AverageGrade)}");
                        if (groupChart is not null)
                            column.Item().Height(230).Image(groupChart).FitArea();
                        column.Item().Text("Показники за групами").Bold().FontSize(12);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });
                            table.Header(h =>
                            {
                                HeaderCell(h.Cell(), "Група");
                                HeaderCell(h.Cell(), "Студентів");
                                HeaderCell(h.Cell(), "З оцінками");
                                HeaderCell(h.Cell(), "Оцінок");
                                HeaderCell(h.Cell(), "Сер. бал");
                                HeaderCell(h.Cell(), "Низьких");
                            });
                            foreach (var g in data.Groups)
                            {
                                BodyCell(table.Cell(), g.GroupName);
                                BodyCell(table.Cell(), g.StudentCount.ToString());
                                BodyCell(table.Cell(), g.GradedStudents.ToString());
                                BodyCell(table.Cell(), g.GradeCount.ToString());
                                BodyCell(table.Cell(), FormatScore(g.AverageGrade));
                                BodyCell(table.Cell(), g.LowGradeCount.ToString());
                            }
                        });
                        if (gradesChart is not null)
                            column.Item().Height(230).Image(gradesChart).FitArea();
                        column.Item().Text(
                            "Методика: середній бал обчислено за виставленими оцінками 1–12; " +
                            "відсутня оцінка не є нулем. Низькі оцінки: 1–3; високі: 10–12. " +
                            "Дату визначає заняття. Дані згруповано за поточною групою студента; " +
                            "історію переведень не враховано.").FontSize(9).Italic();
                    });
                });

                document.Page(page =>
                {
                    ConfigurePage(page);
                    page.Content().PaddingVertical(12).Column(column =>
                    {
                        column.Spacing(10);
                        column.Item().Text("ДЕТАЛІЗАЦІЯ УСПІШНОСТІ СТУДЕНТІВ")
                            .Bold().FontSize(14);
                        column.Item().Text($"Група: {data.GroupLabel} | Період: {data.PeriodLabel}");
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2);
                                c.RelativeColumn(4);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });
                            table.Header(h =>
                            {
                                HeaderCell(h.Cell(), "Група");
                                HeaderCell(h.Cell(), "Студент");
                                HeaderCell(h.Cell(), "Оцінок");
                                HeaderCell(h.Cell(), "Сер. бал");
                                HeaderCell(h.Cell(), "Низьких");
                                HeaderCell(h.Cell(), "Високих");
                            });
                            foreach (var s in data.Students)
                            {
                                BodyCell(table.Cell(), s.GroupName);
                                BodyCell(table.Cell(), s.FullName);
                                BodyCell(table.Cell(), s.GradeCount.ToString());
                                BodyCell(table.Cell(), FormatScore(s.AverageGrade));
                                BodyCell(table.Cell(), s.LowGradeCount.ToString());
                                BodyCell(table.Cell(), s.HighGradeCount.ToString());
                            }
                        });
                    });
                });
            }).GeneratePdf(filePath);
        }
        finally
        {
            if (groupChart is not null) File.Delete(groupChart);
            if (gradesChart is not null) File.Delete(gradesChart);
        }
    }

    private static string FormatScore(double? score) =>
        score.HasValue ? score.Value.ToString("0.00") : "—";

    private static void ConfigurePage(PageDescriptor page)
    {
        page.Size(PageSizes.A4.Landscape());
        page.Margin(30);
        page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(9));
        page.Header().Column(h =>
        {
            h.Item().Text("EDUCATION SYSTEM — АНАЛІТИЧНИЙ ЗВІТ").Bold().FontSize(11);
            h.Item().Text($"Сформовано: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(8);
            h.Item().LineHorizontal(1);
        });
        page.Footer().AlignRight().Text(t =>
        {
            t.Span("Сторінка ");
            t.CurrentPageNumber();
            t.Span(" із ");
            t.TotalPages();
        });
    }

    private static void HeaderCell(IContainer cell, string text) =>
        cell.Padding(4).Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Text(text).Bold();

    private static void BodyCell(IContainer cell, string text) =>
        cell.PaddingVertical(4).PaddingHorizontal(3).Text(text);

    private static string SaveChart(Action<Plot> populate)
    {
        string path = Path.Combine(Path.GetTempPath(), $"edu-performance-{Guid.NewGuid():N}.png");
        var plot = new Plot();
        populate(plot);
        plot.SavePng(path, 1050, 460);
        return path;
    }

    private static void WriteHeader(IXLWorksheet sheet, int row, params string[] headers)
    {
        for (int i = 0; i < headers.Length; i++)
            sheet.Cell(row, i + 1).Value = headers[i];
        var range = sheet.Range(row, 1, row, headers.Length);
        range.Style.Fill.BackgroundColor = XLColor.FromHtml("#173B5E");
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.Bold = true;
    }
}
