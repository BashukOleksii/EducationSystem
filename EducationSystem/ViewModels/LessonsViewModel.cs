using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Lessons;
using EducationSystem.DTOs.TeacherSubjects;
using EducationSystem.Models;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels
{
    public partial class LessonsViewModel
        : ObservableObject
    {
        private readonly LessonService
            _lessonService;

        private readonly TeacherSubjectService
            _teacherSubjectService;

        private readonly TeacherService
            _teacherService;

        private readonly SubjectService
            _subjectService;

        private readonly GroupService
            _groupService;

        private int? _editingId;


        public LessonsViewModel(
            LessonService lessonService,
            TeacherSubjectService teacherSubjectService,
            TeacherService teacherService,
            SubjectService subjectService,
            GroupService groupService)
        {
            _lessonService =
                lessonService;

            _teacherSubjectService =
                teacherSubjectService;

            _teacherService =
                teacherService;

            _subjectService =
                subjectService;

            _groupService =
                groupService;
        }


        public ObservableCollection<LessonListItem>
            Lessons
        { get; } = new();


        public ObservableCollection<TeacherSubjectListItem>
            TeacherSubjects
        { get; } = new();


        public ObservableCollection<Teacher>
            Teachers
        { get; } = new();


        public ObservableCollection<Subject>
            Subjects
        { get; } = new();


        public ObservableCollection<Group>
            Groups
        { get; } = new();


        [ObservableProperty]
        private LessonListItem? selectedLesson;

        [ObservableProperty]
        private string searchText =
            string.Empty;
        [ObservableProperty]
        private bool isTeacherFilterEnabled;


        [ObservableProperty]
        private Teacher? filterTeacher;

        [ObservableProperty]
        private bool isSubjectFilterEnabled;


        [ObservableProperty]
        private Subject? filterSubject;


        [ObservableProperty]
        private bool isGroupFilterEnabled;


        [ObservableProperty]
        private Group? filterGroup;

        [ObservableProperty]
        private bool isDateFilterEnabled;


        [ObservableProperty]
        private DateTime? fromDate =
            DateTime.Today.AddMonths(-1);


        [ObservableProperty]
        private DateTime? toDate =
            DateTime.Today;

        [ObservableProperty]
        private string title =
            string.Empty;


        [ObservableProperty]
        private TeacherSubjectListItem?
            selectedTeacherSubject;


        [ObservableProperty]
        private Group? selectedGroup;


        [ObservableProperty]
        private DateTime lessonDate =
            DateTime.Today;

        [ObservableProperty]
        private int currentPage = 1;


        [ObservableProperty]
        private int totalPages = 1;


        [ObservableProperty]
        private int totalCount;


        [ObservableProperty]
        private string message =
            string.Empty;


        [ObservableProperty]
        private bool isLoading;


        public int PageSize { get; } = 10;


        public bool IsEditing =>
            _editingId.HasValue;


        partial void OnSelectedLessonChanged(
            LessonListItem? value)
        {
            if (value is null)
            {
                return;
            }


            _editingId =
                value.Id;


            Title =
                value.Title ?? string.Empty;


            LessonDate =
                value.Date;


            SelectedTeacherSubject =
                TeacherSubjects.FirstOrDefault(
                    item =>
                        item.Id == value.TeacherSubjectId
                );


            SelectedGroup =
                Groups.FirstOrDefault(
                    group =>
                        group.Id == value.GroupId
                );


            Message =
                string.Empty;


            OnPropertyChanged(
                nameof(IsEditing)
            );
        }


        [RelayCommand]
        public async Task LoadAsync()
        {
            try
            {
                IsLoading =
                    true;


                if (TeacherSubjects.Count == 0 ||
                    Teachers.Count == 0 ||
                    Subjects.Count == 0 ||
                    Groups.Count == 0)
                {
                    await LoadLookupsAsync();
                }


                int? teacherId =
                    IsTeacherFilterEnabled &&
                    FilterTeacher is not null
                        ? FilterTeacher.Id
                        : null;


                int? subjectId =
                    IsSubjectFilterEnabled &&
                    FilterSubject is not null
                        ? FilterSubject.Id
                        : null;


                int? groupId =
                    IsGroupFilterEnabled &&
                    FilterGroup is not null
                        ? FilterGroup.Id
                        : null;


                DateTime? actualFromDate =
                    IsDateFilterEnabled
                        ? FromDate
                        : null;


                DateTime? actualToDate =
                    IsDateFilterEnabled
                        ? ToDate
                        : null;


                PagedResult<LessonListItem> result =
                    await _lessonService
                        .GetPagedAsync(
                            SearchText,
                            teacherId,
                            subjectId,
                            groupId,
                            actualFromDate,
                            actualToDate,
                            CurrentPage,
                            PageSize
                        );


                Lessons.Clear();


                foreach (
                    LessonListItem lesson
                    in result.Items)
                {
                    Lessons.Add(
                        lesson
                    );
                }


                TotalCount =
                    result.TotalCount;


                TotalPages =
                    Math.Max(
                        1,
                        result.TotalPages
                    );


                if (CurrentPage > TotalPages)
                {
                    CurrentPage =
                        TotalPages;


                    await LoadAsync();


                    return;
                }
            }
            catch (Exception exception)
            {
                Message =
                    exception.Message;
            }
            finally
            {
                IsLoading =
                    false;
            }
        }


        private async Task LoadLookupsAsync()
        {
            IReadOnlyList<TeacherSubjectListItem>
                teacherSubjects =
                    await _teacherSubjectService
                        .GetAllAsync();


            IReadOnlyList<Teacher> teachers =
                await _teacherService
                    .GetAllAsync();


            IReadOnlyList<Subject> subjects =
                await _subjectService
                    .GetAllAsync();


            IReadOnlyList<Group> groups =
                await _groupService
                    .GetAllAsync();


            TeacherSubjects.Clear();
            Teachers.Clear();
            Subjects.Clear();
            Groups.Clear();


            foreach (
                TeacherSubjectListItem item
                in teacherSubjects)
            {
                TeacherSubjects.Add(item);
            }


            foreach (Teacher teacher in teachers)
            {
                Teachers.Add(teacher);
            }


            foreach (Subject subject in subjects)
            {
                Subjects.Add(subject);
            }


            foreach (Group group in groups)
            {
                Groups.Add(group);
            }


            FilterTeacher ??=
                Teachers.FirstOrDefault();


            FilterSubject ??=
                Subjects.FirstOrDefault();


            FilterGroup ??=
                Groups.FirstOrDefault();


            if (!_editingId.HasValue)
            {
                SelectedTeacherSubject ??=
                    TeacherSubjects.FirstOrDefault();


                SelectedGroup ??=
                    Groups.FirstOrDefault();
            }
        }


        [RelayCommand]
        private async Task SearchAsync()
        {
            Message =
                string.Empty;


            CurrentPage =
                1;


            await LoadAsync();
        }


        [RelayCommand]
        private async Task ClearSearchAsync()
        {
            SearchText =
                string.Empty;


            IsTeacherFilterEnabled =
                false;


            IsSubjectFilterEnabled =
                false;


            IsGroupFilterEnabled =
                false;


            IsDateFilterEnabled =
                false;


            FilterTeacher =
                Teachers.FirstOrDefault();


            FilterSubject =
                Subjects.FirstOrDefault();


            FilterGroup =
                Groups.FirstOrDefault();


            FromDate =
                DateTime.Today.AddMonths(-1);


            ToDate =
                DateTime.Today;


            CurrentPage =
                1;


            Message =
                string.Empty;


            await LoadAsync();
        }


        [RelayCommand]
        private void New()
        {
            ResetForm(
                clearMessage: true
            );
        }


        [RelayCommand]
        private async Task SaveAsync()
        {
            if (SelectedTeacherSubject is null)
            {
                Message =
                    "Необхідно вибрати викладача та предмет.";

                return;
            }


            if (SelectedGroup is null)
            {
                Message =
                    "Необхідно вибрати групу.";

                return;
            }


            try
            {
                Message =
                    string.Empty;


                if (_editingId.HasValue)
                {
                    await _lessonService
                        .UpdateAsync(
                            _editingId.Value,
                            Title,
                            SelectedTeacherSubject.Id,
                            SelectedGroup.Id,
                            LessonDate
                        );


                    ResetForm(
                        clearMessage: false
                    );


                    Message =
                        "Заняття успішно оновлено.";
                }
                else
                {
                    await _lessonService
                        .CreateAsync(
                            Title,
                            SelectedTeacherSubject.Id,
                            SelectedGroup.Id,
                            LessonDate
                        );


                    ResetForm(
                        clearMessage: false
                    );


                    Message =
                        "Заняття успішно створено.";
                }


                await LoadAsync();
            }
            catch (Exception exception)
            {
                Message =
                    exception.Message;
            }
        }


        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (SelectedLesson is null)
            {
                Message =
                    "Спочатку виберіть заняття.";

                return;
            }


            try
            {
                int id =
                    SelectedLesson.Id;


                await _lessonService
                    .DeleteAsync(id);


                ResetForm(
                    clearMessage: false
                );


                Message =
                    "Заняття успішно видалено.";


                await LoadAsync();
            }
            catch (Exception exception)
            {
                Message =
                    exception.Message;
            }
        }


        [RelayCommand]
        private async Task PreviousPageAsync()
        {
            if (CurrentPage <= 1)
            {
                return;
            }


            CurrentPage--;


            await LoadAsync();
        }


        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (CurrentPage >= TotalPages)
            {
                return;
            }


            CurrentPage++;


            await LoadAsync();
        }


        private void ResetForm(
            bool clearMessage)
        {
            SelectedLesson =
                null;


            _editingId =
                null;


            Title =
                string.Empty;


            LessonDate =
                DateTime.Today;


            SelectedTeacherSubject =
                TeacherSubjects.FirstOrDefault();


            SelectedGroup =
                Groups.FirstOrDefault();


            if (clearMessage)
            {
                Message =
                    string.Empty;
            }


            OnPropertyChanged(
                nameof(IsEditing)
            );
        }
    }
}