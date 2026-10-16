using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace EducationSystem.DTOs.Journal
{
    /// <summary>
    /// Рядок електронного журналу. GradeText використовується для введення з DataGrid,
    /// Grade — для передавання числового значення до БД.
    /// </summary>
    public sealed class JournalStudentItem : ObservableObject
    {
        private int? _grade;
        private string _gradeText = string.Empty;
        private bool _isMissing;

        public int StudentId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        /// <summary>Nullable-оцінка, що передається до SQL. NULL означає видалення оцінки.</summary>
        public int? Grade
        {
            get => _grade;
            set
            {
                if (!SetProperty(ref _grade, value))
                    return;

                SetProperty(
                    ref _gradeText,
                    value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
                    nameof(GradeText));

                // Оцінка означає, що позначка пропуску має бути знята.
                if (value.HasValue && _isMissing)
                    IsMissing = false;
            }
        }

        /// <summary>
        /// Текст для DataGrid: порожнє поле однозначно стає Grade = null.
        /// Некоректні значення залишаються видимими, але не зберігаються.
        /// </summary>
        public string GradeText
        {
            get => _gradeText;
            set
            {
                string text = value ?? string.Empty;

                if (!SetProperty(ref _gradeText, text))
                    return;

                if (string.IsNullOrWhiteSpace(text))
                {
                    SetProperty(ref _grade, (int?)null, nameof(Grade));
                    return;
                }

                if (int.TryParse(text, NumberStyles.None,
                        CultureInfo.InvariantCulture, out int parsed)
                    && parsed >= 1 && parsed <= 12)
                {
                    SetProperty(ref _grade, (int?)parsed, nameof(Grade));

                    if (_isMissing)
                        IsMissing = false;
                }
                else
                {
                    // Зберігаємо введений текст, щоб користувач міг його виправити.
                    // Сервіс заблокує збереження некоректного GradeText.
                    SetProperty(ref _grade, (int?)null, nameof(Grade));
                }
            }
        }

        public string? AttendanceStatusValue { get; set; }

        public bool IsMissing
        {
            get => _isMissing;
            set
            {
                if (!SetProperty(ref _isMissing, value))
                    return;

                if (value)
                {
                    // НБ означає, що оцінки бути не може.
                    Grade = null;

                    // У разі, коли Grade вже був null, але GradeText був некоректним,
                    // потрібно також очистити текстове поле.
                    SetProperty(ref _gradeText, string.Empty, nameof(GradeText));
                }
            }
        }

        public string FullName => $"{LastName} {FirstName}";
    }
}
