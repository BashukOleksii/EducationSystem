using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.TeacherSubjects;
using EducationSystem.Models;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels
{
    public partial class TeacherSubjectsViewModel
        : ObservableObject
    {
        private readonly TeacherSubjectService
            _teacherSubjectService;

        private readonly TeacherService
            _teacherService;

        private readonly SubjectService
            _subjectService;

        private int? _editingId;


        public TeacherSubjectsViewModel(
            TeacherSubjectService teacherSubjectService,
            TeacherService teacherService,
            SubjectService subjectService)
        {
            _teacherSubjectService =
                teacherSubjectService;

            _teacherService =
                teacherService;

            _subjectService =
                subjectService;
        }


        public ObservableCollection<TeacherSubjectListItem>
            TeacherSubjects
        { get; } = new();


        public ObservableCollection<Teacher>
            Teachers
        { get; } = new();


        public ObservableCollection<Subject>
            Subjects
        { get; } = new();


        [ObservableProperty]
        private TeacherSubjectListItem?
            selectedTeacherSubject;


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
        private Teacher? selectedTeacher;


        [ObservableProperty]
        private Subject? selectedSubject;


        [ObservableProperty]
        private bool hasSubgroup;


        [ObservableProperty]
        private int subgroup = 1;


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


        partial void OnSelectedTeacherSubjectChanged(
            TeacherSubjectListItem? value)
        {
            if (value is null)
            {
                return;
            }


            _editingId =
                value.Id;


            SelectedTeacher =
                Teachers.FirstOrDefault(
                    teacher =>
                        teacher.Id == value.TeacherId
                );


            SelectedSubject =
                Subjects.FirstOrDefault(
                    subject =>
                        subject.Id == value.SubjectId
                );


            HasSubgroup =
                value.Subgroup.HasValue;


            Subgroup =
                value.Subgroup ?? 1;


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
                IsLoading = true;


                if (Teachers.Count == 0 ||
                    Subjects.Count == 0)
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


                PagedResult<TeacherSubjectListItem> result =
                    await _teacherSubjectService
                        .GetPagedAsync(
                            SearchText,
                            teacherId,
                            subjectId,
                            CurrentPage,
                            PageSize
                        );


                TeacherSubjects.Clear();


                foreach (
                    TeacherSubjectListItem item
                    in result.Items)
                {
                    TeacherSubjects.Add(
                        item
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
                IsLoading = false;
            }
        }


        private async Task LoadLookupsAsync()
        {
            IReadOnlyList<Teacher> teachers =
                await _teacherService
                    .GetAllAsync();


            IReadOnlyList<Subject> subjects =
                await _subjectService
                    .GetAllAsync();


            Teachers.Clear();

            Subjects.Clear();


            foreach (Teacher teacher in teachers)
            {
                Teachers.Add(
                    teacher
                );
            }


            foreach (Subject subject in subjects)
            {
                Subjects.Add(
                    subject
                );
            }


            FilterTeacher ??=
                Teachers.FirstOrDefault();


            FilterSubject ??=
                Subjects.FirstOrDefault();


            if (!_editingId.HasValue)
            {
                SelectedTeacher ??=
                    Teachers.FirstOrDefault();


                SelectedSubject ??=
                    Subjects.FirstOrDefault();
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


            FilterTeacher =
                Teachers.FirstOrDefault();


            FilterSubject =
                Subjects.FirstOrDefault();


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
            if (SelectedTeacher is null)
            {
                Message =
                    "Необхідно вибрати викладача.";

                return;
            }


            if (SelectedSubject is null)
            {
                Message =
                    "Необхідно вибрати предмет.";

                return;
            }


            int? subgroupValue =
                HasSubgroup
                    ? Subgroup
                    : null;


            try
            {
                Message =
                    string.Empty;


                if (_editingId.HasValue)
                {
                    await _teacherSubjectService
                        .UpdateAsync(
                            _editingId.Value,
                            SelectedTeacher.Id,
                            SelectedSubject.Id,
                            subgroupValue
                        );


                    ResetForm(
                        clearMessage: false
                    );


                    Message =
                        "Призначення успішно оновлено.";
                }
                else
                {
                    await _teacherSubjectService
                        .CreateAsync(
                            SelectedTeacher.Id,
                            SelectedSubject.Id,
                            subgroupValue
                        );


                    ResetForm(
                        clearMessage: false
                    );


                    Message =
                        "Предмет успішно призначено викладачу.";
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
            if (SelectedTeacherSubject is null)
            {
                Message =
                    "Спочатку виберіть запис.";

                return;
            }


            try
            {
                int id =
                    SelectedTeacherSubject.Id;


                await _teacherSubjectService
                    .DeleteAsync(id);


                ResetForm(
                    clearMessage: false
                );


                Message =
                    "Призначення успішно видалено.";


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
            SelectedTeacherSubject =
                null;


            _editingId =
                null;


            SelectedTeacher =
                Teachers.FirstOrDefault();


            SelectedSubject =
                Subjects.FirstOrDefault();


            HasSubgroup =
                false;


            Subgroup =
                1;


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