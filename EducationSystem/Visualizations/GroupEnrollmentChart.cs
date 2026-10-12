using EducationSystem.DTOs.Analytics;
using ScottPlot;

namespace EducationSystem.Visualizations;

public static class GroupEnrollmentChart
{
    public static void Populate(Plot plot, IReadOnlyList<GroupEnrollmentRow> rows)
    {
        plot.Clear();

        if (rows.Count == 0)
        {
            plot.Title("Немає даних для відображення");
            return;
        }

        var values = rows.Select(x => (double)x.StudentCount).ToArray();
        var bars = plot.Add.Bars(values);

        foreach (var bar in bars.Bars)
            bar.Label = bar.Value.ToString("0");

        bars.ValueLabelStyle.Bold = true;
        bars.ValueLabelStyle.FontSize = 12;

        var positions = Enumerable.Range(0, rows.Count).Select(i => (double)i).ToArray();
        var labels = rows.Select(x => x.GroupName).ToArray();
        plot.Axes.Bottom.SetTicks(positions, labels);
        plot.Axes.Margins(bottom: 0, top: .25);
        plot.Title("Наповненість академічних груп");
        plot.YLabel("Студентів");
    }
}
