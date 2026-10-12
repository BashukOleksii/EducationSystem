using ClosedXML.Excel;
using EducationSystem.DTOs.Analytics;
using EducationSystem.Visualizations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ScottPlot;
using System.IO;

namespace EducationSystem.Services;

public sealed class AttendanceReportExportService
{
    public void ExportExcel(string filePath, AttendanceReportData data)
    {
        using var workbook = new XLWorkbook();
        var overview = workbook.Worksheets.Add("Зведення");
        overview.Cell("A1").Value = "EDUCATION SYSTEM — ЗВІТ ПРО ВІДВІДУВАНІСТЬ";
        overview.Range("A1:G1").Merge();
        overview.Range("A1:G1").Style.Font.Bold = true;
        overview.Range("A1:G1").Style.Font.FontSize = 16;
        overview.Cell("A2").Value = $"{data.GroupLabel} | {data.SubjectLabel} | {data.PeriodLabel}";
        overview.Range("A2:G2").Merge();
        overview.Cell("A3").Value = "Сформовано";
        overview.Cell("B3").Value = DateTime.Now;
        overview.Cell("B3").Style.DateFormat.Format = "dd.mm.yyyy hh:mm";

        overview.Cell("A4").Value = "Студентів";
        overview.Cell("B4").Value = data.TotalStudents;
        overview.Cell("C4").Value = "Присутностей";
        overview.Cell("D4").Value = data.TotalPresent;
        overview.Cell("E4").Value = "Пропусків";
        overview.Cell("F4").Value = data.TotalMissing;
        overview.Cell("A5").Value = "Без відмітки";
        overview.Cell("B5").Value = data.TotalUnmarked;
        overview.Cell("C5").Value = "Відвідуваність";
        if (data.AttendancePercent.HasValue)
            overview.Cell("D5").Value = data.AttendancePercent.Value / 100.0;
        overview.Cell("D5").Style.NumberFormat.Format = "0.0%";
        overview.Cell("E5").Value = "Заповнення журналу";
        if (data.CoveragePercent.HasValue)
            overview.Cell("F5").Value = data.CoveragePercent.Value / 100.0;
        overview.Cell("F5").Style.NumberFormat.Format = "0.0%";
        WriteHeader(overview, 8, "Група", "Студентів", "Очікується", "Присутніх", "Пропусків", "Без відмітки", "Відвідуваність");
        int row = 9;
        foreach (var item in data.Groups)
        {
            overview.Cell(row, 1).Value = item.GroupName;
            overview.Cell(row, 2).Value = item.StudentCount;
            overview.Cell(row, 3).Value = item.ExpectedCount;
            overview.Cell(row, 4).Value = item.PresentCount;
            overview.Cell(row, 5).Value = item.MissingCount;
            overview.Cell(row, 6).Value = item.UnmarkedCount;
            overview.Cell(row, 7).FormulaA1 = $"=IF(D{row}+E{row}=0,\"\",D{row}/(D{row}+E{row}))";
            overview.Cell(row, 7).Style.NumberFormat.Format = "0.0%";
            row++;
        }
        overview.Cell(row, 1).Value = "РАЗОМ";
        overview.Range(row, 1, row, 7).Style.Font.Bold = true;
        for (int col = 2; col <= 6; col++)
        {
            string colName = ((char)('A' + col - 1)).ToString();
            overview.Cell(row, col).FormulaA1 = data.Groups.Count == 0
                ? "=0"
                : $"=SUM({colName}9:{colName}{row - 1})";
        }
        overview.Cell(row, 7).FormulaA1 = $"=IF(D{row}+E{row}=0,\"\",D{row}/(D{row}+E{row}))";
        overview.Cell(row, 7).Style.NumberFormat.Format = "0.0%";

        var students = workbook.Worksheets.Add("Студенти");
        students.Cell("A1").Value = $"Відвідуваність студентів — {data.GroupLabel}";
        students.Range("A1:G1").Merge();
        students.Range("A1:G1").Style.Font.Bold = true;
        WriteHeader(students, 3, "Група", "Студент", "Очікується", "Присутніх", "Пропусків", "Без відмітки", "Відвідуваність");
        int studentRow = 4;
        foreach (var item in data.Students)
        {
            students.Cell(studentRow, 1).Value = item.GroupName;
            students.Cell(studentRow, 2).Value = item.FullName;
            students.Cell(studentRow, 3).Value = item.ExpectedCount;
            students.Cell(studentRow, 4).Value = item.PresentCount;
            students.Cell(studentRow, 5).Value = item.MissingCount;
            students.Cell(studentRow, 6).Value = item.UnmarkedCount;
            students.Cell(studentRow, 7).FormulaA1 = $"=IF(D{studentRow}+E{studentRow}=0,\"\",D{studentRow}/(D{studentRow}+E{studentRow}))";
            students.Cell(studentRow, 7).Style.NumberFormat.Format = "0.0%";
            studentRow++;
        }

        var subjects = workbook.Worksheets.Add("Предмети");
        subjects.Cell("A1").Value = $"Відвідуваність за дисциплінами — {data.PeriodLabel}";
        subjects.Range("A1:F1").Merge();
        subjects.Range("A1:F1").Style.Font.Bold = true;
        WriteHeader(subjects, 3, "Предмет", "Очікується", "Присутніх", "Пропусків", "Без відмітки", "Відвідуваність");
        int subjectRow = 4;
        foreach (var item in data.Subjects)
        {
            subjects.Cell(subjectRow, 1).Value = item.SubjectName;
            subjects.Cell(subjectRow, 2).Value = item.ExpectedCount;
            subjects.Cell(subjectRow, 3).Value = item.PresentCount;
            subjects.Cell(subjectRow, 4).Value = item.MissingCount;
            subjects.Cell(subjectRow, 5).Value = item.UnmarkedCount;
            subjects.Cell(subjectRow, 6).FormulaA1 = $"=IF(C{subjectRow}+D{subjectRow}=0,\"\",C{subjectRow}/(C{subjectRow}+D{subjectRow}))";
            subjects.Cell(subjectRow, 6).Style.NumberFormat.Format = "0.0%";
            subjectRow++;
        }

        var months = workbook.Worksheets.Add("Динаміка");
        months.Cell("A1").Value = "Помісячна динаміка відміток журналу";
        months.Range("A1:F1").Merge();
        months.Range("A1:F1").Style.Font.Bold = true;
        WriteHeader(months, 3, "Місяць", "Очікується", "Присутніх", "Пропусків", "Без відмітки", "Заповнення журналу");
        int monthRow = 4;
        foreach (var item in data.Months)
        {
            months.Cell(monthRow, 1).Value = item.MonthKey;
            months.Cell(monthRow, 2).Value = item.ExpectedCount;
            months.Cell(monthRow, 3).Value = item.PresentCount;
            months.Cell(monthRow, 4).Value = item.MissingCount;
            months.Cell(monthRow, 5).Value = item.UnmarkedCount;
            months.Cell(monthRow, 6).FormulaA1 = $"=IF(B{monthRow}=0,\"\",(C{monthRow}+D{monthRow})/B{monthRow})";
            months.Cell(monthRow, 6).Style.NumberFormat.Format = "0.0%";
            monthRow++;
        }

        string? groupChart = null;
        string? monthChart = null;
        try
        {
            if (data.TotalMarked > 0)
            {
                groupChart = SaveChart(plot => AttendanceCharts.PopulateGroups(plot, data));
                overview.AddPicture(groupChart).MoveTo(overview.Cell("I8")).WithSize(720, 350);
            }
            if (data.Months.Count > 0)
            {
                monthChart = SaveChart(plot => AttendanceCharts.PopulateMonthlyCoverage(plot, data));
                months.AddPicture(monthChart).MoveTo(months.Cell("H3")).WithSize(720, 350);
            }

            foreach (var sheet in workbook.Worksheets)
            {
                sheet.ColumnsUsed().AdjustToContents();
                sheet.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            }
            overview.SheetView.FreezeRows(8);
            students.SheetView.FreezeRows(3);
            subjects.SheetView.FreezeRows(3);
            months.SheetView.FreezeRows(3);
            workbook.SaveAs(filePath);
        }
        finally
        {
            DeleteIfExists(groupChart);
            DeleteIfExists(monthChart);
        }
    }

    public void ExportPdf(string filePath, AttendanceReportData data)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        string? groupChart = null;
        string? monthChart = null;
        try
        {
            if (data.TotalMarked > 0)
                groupChart = SaveChart(plot => AttendanceCharts.PopulateGroups(plot, data));
            if (data.Months.Count > 0)
                monthChart = SaveChart(plot => AttendanceCharts.PopulateMonthlyCoverage(plot, data));

            Document.Create(document =>
            {
                document.Page(page =>
                {
                    ConfigurePage(page);
                    page.Content().PaddingVertical(12).Column(column =>
                    {
                        column.Spacing(10);
                        column.Item().Text("ЗВІТ ПРО ВІДВІДУВАНІСТЬ").Bold().FontSize(16);
                        column.Item().Text($"Група: {data.GroupLabel} | Предмет: {data.SubjectLabel}");
                        column.Item().Text($"Період: {data.PeriodLabel} | Сформовано: {DateTime.Now:dd.MM.yyyy HH:mm}");
                        column.Item().Text(
                            $"Студентів: {data.TotalStudents} | Присутностей: {data.TotalPresent} | " +
                            $"Пропусків: {data.TotalMissing} | Без відмітки: {data.TotalUnmarked}");
                        column.Item().Text(
                            $"Відвідуваність: {AttendanceReportData.FormatPercent(data.AttendancePercent)} | " +
                            $"Заповнення журналу: {AttendanceReportData.FormatPercent(data.CoveragePercent)}").Bold();
                        if (groupChart is not null)
                            column.Item().Height(220).Image(groupChart).FitArea();

                        column.Item().Text("1. Показники за групами").Bold().FontSize(12);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2.5f);
                                cols.RelativeColumn(1.4f);
                                cols.RelativeColumn(1.4f);
                                cols.RelativeColumn(1.4f);
                                cols.RelativeColumn(1.4f);
                                cols.RelativeColumn(1.8f);
                            });
                            table.Header(h =>
                            {
                                HeaderCell(h.Cell(), "Група");
                                HeaderCell(h.Cell(), "Очікується");
                                HeaderCell(h.Cell(), "Присутніх");
                                HeaderCell(h.Cell(), "Пропусків");
                                HeaderCell(h.Cell(), "Без відмітки");
                                HeaderCell(h.Cell(), "Відвідуваність");
                            });
                            foreach (var item in data.Groups)
                            {
                                BodyCell(table.Cell(), item.GroupName);
                                BodyCell(table.Cell(), item.ExpectedCount.ToString());
                                BodyCell(table.Cell(), item.PresentCount.ToString());
                                BodyCell(table.Cell(), item.MissingCount.ToString());
                                BodyCell(table.Cell(), item.UnmarkedCount.ToString());
                                BodyCell(table.Cell(), AttendanceReportData.FormatPercent(item.AttendancePercent));
                            }
                        });
                        if (monthChart is not null)
                            column.Item().Height(220).Image(monthChart).FitArea();
                        column.Item().Text("Методика: одна очікувана відмітка — один студент поточного " +
                            "складу групи на одному занятті. Статус present означає присутність, " +
                            "missing — пропуск, NULL — журнал ще не заповнено. " +
                            "Відвідуваність = присутності / (присутності + пропуски). " +
                            "Заповнення = (присутності + пропуски) / очікувані відмітки. " +
                            "За відсутності записів показник не обчислюється. " +
                            "Заняття підгруп виключені: у студентів немає номера підгрупи. " +
                            "Історія переведень між групами не реконструюється.")
                            .Italic().FontSize(8);
                    });
                });

                document.Page(page =>
                {
                    ConfigurePage(page);
                    page.Content().PaddingVertical(12).Column(column =>
                    {
                        column.Spacing(10);
                        column.Item().Text("2. Детальна відомість студентів").Bold().FontSize(13);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(4);
                                cols.RelativeColumn(1.3f);
                                cols.RelativeColumn(1.3f);
                                cols.RelativeColumn(1.3f);
                                cols.RelativeColumn(1.5f);
                            });
                            table.Header(h =>
                            {
                                HeaderCell(h.Cell(), "Група");
                                HeaderCell(h.Cell(), "Студент");
                                HeaderCell(h.Cell(), "+");
                                HeaderCell(h.Cell(), "−");
                                HeaderCell(h.Cell(), "Не заповн.");
                                HeaderCell(h.Cell(), "Відвід., %");
                            });
                            foreach (var item in data.Students)
                            {
                                BodyCell(table.Cell(), item.GroupName);
                                BodyCell(table.Cell(), item.FullName);
                                BodyCell(table.Cell(), item.PresentCount.ToString());
                                BodyCell(table.Cell(), item.MissingCount.ToString());
                                BodyCell(table.Cell(), item.UnmarkedCount.ToString());
                                BodyCell(table.Cell(), AttendanceReportData.FormatPercent(item.AttendancePercent));
                            }
                        });
                        column.Item().Text("3. Дисципліни").Bold().FontSize(13);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(4);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                            });
                            table.Header(h =>
                            {
                                HeaderCell(h.Cell(), "Предмет");
                                HeaderCell(h.Cell(), "Присутніх");
                                HeaderCell(h.Cell(), "Пропусків");
                                HeaderCell(h.Cell(), "Без відмітки");
                                HeaderCell(h.Cell(), "Відвід., %");
                            });
                            foreach (var item in data.Subjects)
                            {
                                BodyCell(table.Cell(), item.SubjectName);
                                BodyCell(table.Cell(), item.PresentCount.ToString());
                                BodyCell(table.Cell(), item.MissingCount.ToString());
                                BodyCell(table.Cell(), item.UnmarkedCount.ToString());
                                BodyCell(table.Cell(), AttendanceReportData.FormatPercent(item.AttendancePercent));
                            }
                        });
                    });
                });
            }).GeneratePdf(filePath);
        }
        finally
        {
            DeleteIfExists(groupChart);
            DeleteIfExists(monthChart);
        }
    }

    private static void ConfigurePage(PageDescriptor page)
    {
        page.Size(PageSizes.A4);
        page.Margin(34);
        page.DefaultTextStyle(x => x.FontFamily("Lato").FontSize(9));
        page.Header().Column(h =>
        {
            h.Item().Text("EDUCATION SYSTEM").Bold().FontSize(13);
            h.Item().Text("Звітність навчального процесу").FontSize(8);
            h.Item().LineHorizontal(1);
        });
        page.Footer().AlignRight().Text(t =>
        {
            t.Span("Сторінка ");
            t.CurrentPageNumber();
            t.Span(" з ");
            t.TotalPages();
        });
    }

    private static void HeaderCell(IContainer cell, string text) =>
        cell.Background(QuestPDF.Helpers.Colors.Grey.Lighten3).Padding(3).Text(text).Bold().FontSize(8);

    private static void BodyCell(IContainer cell, string text) =>
        cell.BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(3).Text(text).FontSize(8);

    private static void WriteHeader(IXLWorksheet sheet, int row, params string[] labels)
    {
        for (int i = 0; i < labels.Length; i++)
            sheet.Cell(row, i + 1).Value = labels[i];
        var range = sheet.Range(row, 1, row, labels.Length);
        range.Style.Fill.BackgroundColor = XLColor.FromHtml("#173B5E");
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.Bold = true;
    }

    private static string SaveChart(Action<Plot> populate)
    {
        string file = Path.Combine(Path.GetTempPath(), $"attendance-chart-{Guid.NewGuid():N}.png");
        var plot = new Plot();
        populate(plot);
        plot.SavePng(file, 1100, 470);
        return file;
    }

    private static void DeleteIfExists(string? file)
    {
        if (file is not null && File.Exists(file))
            File.Delete(file);
    }
}
