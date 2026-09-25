namespace EducationSystem.DTOs.TeacherSubjects
{
    public sealed class TeacherSubjectListItem
    {
        public int Id { get; set; }

        public int TeacherId { get; set; }

        public int SubjectId { get; set; }

        public byte? Subgroup { get; set; }


        public string TeacherFirstName { get; set; }
            = string.Empty;

        public string TeacherLastName { get; set; }
            = string.Empty;

        public string TeacherEmail { get; set; }
            = string.Empty;


        public string SubjectName { get; set; }
            = string.Empty;


        public string TeacherFullName =>
            $"{TeacherLastName} {TeacherFirstName}";


        public string SubgroupText =>
            Subgroup.HasValue
                ? Subgroup.Value.ToString()
                : "Без підгрупи";
    }
}