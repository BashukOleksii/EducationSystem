namespace EducationSystem.DTOs.Lessons
{
    public sealed class LessonListItem
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public DateTime Date { get; set; }


        public int TeacherSubjectId { get; set; }

        public int TeacherId { get; set; }

        public int SubjectId { get; set; }

        public int GroupId { get; set; }


        public string TeacherFirstName { get; set; }
            = string.Empty;

        public string TeacherLastName { get; set; }
            = string.Empty;

        public string SubjectName { get; set; }
            = string.Empty;

        public byte? Subgroup { get; set; }


        public string GroupPrefix { get; set; }
            = string.Empty;

        public byte GroupNumber { get; set; }


        public string TeacherFullName =>
            $"{TeacherLastName} {TeacherFirstName}";


        public string GroupName =>
            $"{GroupPrefix}-{GroupNumber}";


        public string SubgroupText =>
            Subgroup.HasValue
                ? Subgroup.Value.ToString()
                : "—";

        public string JournalName =>
         $"{Date:dd.MM.yyyy} | {GroupName} | {SubjectName} | {TeacherFullName}";
    }
}