using EducationSystem.DTOs.Analytics;
using ScottPlot;

namespace EducationSystem.Visualizations;

public static class PerformanceCharts
{
    public static void PopulateGroupAverages(Plot plot, PerformanceReportData data)
    {
        plot.Clear();

        var groups = data.Groups.Where(g => g.AverageGrade.HasValue).ToList();
        if (groups.Count == 0)
        {
            plot.Title("Немає оцінок за вибраними умовами");
            return;
        }

        var bars = plot.Add.Bars(groups.Select(g => g.AverageGrade!.Value).ToArray());
        foreach (var bar in bars.Bars)
            bar.Label = bar.Value.ToString("0.00");

        bars.ValueLabelStyle.FontSize = 11;
        bars.ValueLabelStyle.Bold = true;
        plot.Axes.Bottom.SetTicks(
            Enumerable.Range(0, groups.Count).Select(i => (double)i).ToArray(),
            groups.Select(g => g.GroupName).ToArray());
        plot.Axes.SetLimitsY(0, 13);
        plot.Title("Середній бал за групами");
        plot.YLabel("Бал (1–12)");
    }

    public static void PopulateGradeDistribution(Plot plot, PerformanceReportData data)
    {
        plot.Clear();

        if (data.TotalGrades == 0)
        {
            plot.Title("Немає оцінок за вибраними умовами");
            return;
        }

        var bins = data.Distribution.OrderBy(x => x.Grade).ToList();
        var bars = plot.Add.Bars(bins.Select(g => (double)g.Count).ToArray());
        foreach (var bar in bars.Bars)
            bar.Label = bar.Value.ToString("0");

        bars.ValueLabelStyle.FontSize = 10;
        plot.Axes.Bottom.SetTicks(
            Enumerable.Range(0, bins.Count).Select(i => (double)i).ToArray(),
            bins.Select(b => b.Grade.ToString()).ToArray());
        plot.Axes.Margins(bottom: 0, top: 0.25);
        plot.Title("Розподіл виставлених оцінок");
        plot.YLabel("Кількість оцінок");
        plot.XLabel("Оцінка");
    }
}
