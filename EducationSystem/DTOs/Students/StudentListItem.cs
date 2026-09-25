namespace EducationSystem.DTOs.Students
{
    public sealed class StudentListItem
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int GroupId { get; set; }

        public string GroupPrefix { get; set; } = string.Empty;

        public byte GroupNumber { get; set; }

        public string FullName =>
            $"{LastName} {FirstName}";

        public string GroupName =>
            $"{GroupPrefix}-{GroupNumber}";
    }
}