namespace EducationSystem.DTOs.Journal
{
    public sealed class JournalStudentItem
    {
        public int StudentId { get; set; }

        public string FirstName { get; set; }
            = string.Empty;

        public string LastName { get; set; }
            = string.Empty;

        public string Email { get; set; }
            = string.Empty;

        public int? Grade { get; set; }

        public string? AttendanceStatusValue { get; set; }

        public bool? IsMissing { get; set; }


        public string FullName =>
            $"{LastName} {FirstName}";
    }
}