namespace EducationSystem.DTOs.Analytics
{
    public sealed class SubjectStatisticsItem
    {
        public int SubjectId { get; set; }

        public string SubjectName { get; set; }
            = string.Empty;


        public int PlannedHours { get; set; }


        public int TeacherCount { get; set; }

        public int GroupCount { get; set; }

        public int StudentCount { get; set; }


        public int LessonCount { get; set; }

        public int ConductedHours { get; set; }


        public int GradeCount { get; set; }

        public decimal? AverageGrade { get; set; }


        public int PresentCount { get; set; }

        public int MissingCount { get; set; }

        public decimal? AttendancePercent { get; set; }


        public bool SubjectIsActive { get; set; }


        public int RemainingHours =>
            Math.Max(
                0,
                PlannedHours - ConductedHours
            );
    }
}