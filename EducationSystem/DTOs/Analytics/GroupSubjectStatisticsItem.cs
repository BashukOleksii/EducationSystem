namespace EducationSystem.DTOs.Analytics
{
    public sealed class GroupSubjectStatisticsItem
    {
        public int GroupId { get; set; }

        public string GroupPrefix { get; set; }
            = string.Empty;

        public byte GroupNumber { get; set; }


        public int SubjectId { get; set; }

        public string SubjectName { get; set; }
            = string.Empty;


        public int StudentCount { get; set; }

        public int LessonCount { get; set; }

        public int ConductedHours { get; set; }


        public int GradeCount { get; set; }

        public decimal? AverageGrade { get; set; }


        public int PresentCount { get; set; }

        public int MissingCount { get; set; }

        public decimal? AttendancePercent { get; set; }


        public string GroupName =>
            $"{GroupPrefix}-{GroupNumber}";
    }
}