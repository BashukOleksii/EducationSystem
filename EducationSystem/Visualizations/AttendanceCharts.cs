using EducationSystem.DTOs.Analytics;
using ScottPlot;

namespace EducationSystem.Visualizations;

public static class AttendanceCharts
{
    public static void PopulateGroups(Plot plot, AttendanceReportData data)
    {
        plot.Clear();
        var groups = data.Groups.Where(x => x.AttendancePercent.HasValue).ToList();
        if (groups.Count == 0)
        {
            plot.Title("Немає заповнених відміток відвідування");
            return;
        }

        var bars = plot.Add.Bars(groups.Select(g => g.AttendancePercent!.Value).ToArray());
        foreach (var bar in bars.Bars)
            bar.Label = $"{bar.Value:0.0}%";
        bars.ValueLabelStyle.Bold = true;
        bars.ValueLabelStyle.FontSize = 11;
        plot.Axes.Bottom.SetTicks(
            Enumerable.Range(0, groups.Count).Select(i => (double)i).ToArray(),
            groups.Select(x => x.GroupName).ToArray());
        plot.Axes.SetLimitsY(0, 110);
        plot.Title("Відвідуваність за академічними групами");
        plot.YLabel("Присутність серед відмічених, %");
    }

    public static void PopulateMonthlyCoverage(Plot plot, AttendanceReportData data)
    {
        plot.Clear();
        var months = data.Months.Where(x => x.CoveragePercent.HasValue).ToList();
        if (months.Count == 0)
        {
            plot.Title("Немає занять за вибраними умовами");
            return;
        }

        var bars = plot.Add.Bars(months.Select(m => m.CoveragePercent!.Value).ToArray());
        foreach (var bar in bars.Bars)
            bar.Label = $"{bar.Value:0.0}%";
        bars.ValueLabelStyle.FontSize = 10;
        plot.Axes.Bottom.SetTicks(
            Enumerable.Range(0, months.Count).Select(i => (double)i).ToArray(),
            months.Select(m => m.MonthKey).ToArray());
        plot.Axes.SetLimitsY(0, 110);
        plot.Title("Заповненість журналу за місяцями");
        plot.YLabel("Заповнених відміток, %");
    }
}
