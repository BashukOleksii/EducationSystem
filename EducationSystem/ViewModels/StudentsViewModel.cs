using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Students;
using EducationSystem.Models;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels
{
    public partial class StudentsViewModel : ObservableObject
    {
        private readonly StudentService _studentService;

        private readonly GroupService _groupService;

        private int? _editingId;


        public StudentsViewModel(
            StudentService studentService,
            GroupService groupService)
        {
            _studentService =
                studentService;

            _groupService =
                groupService;
        }


        public ObservableCollection<StudentListItem> Students { get; }
            = new();


        public ObservableCollection<Group> Groups { get; }
            = new();


        [ObservableProperty]
        private StudentListItem? selectedStudent;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isGroupFilterEnabled;


        [ObservableProperty]
        private Group? filterGroup;

        [ObservableProperty]
        private string firstName = string.Empty;


        [ObservableProperty]
        private string lastName = string.Empty;


        [ObservableProperty]
        private string email = string.Empty;


        [ObservableProperty]
        private Group? selectedGroup;

        [ObservableProperty]
        private int currentPage = 1;


        [ObservableProperty]
        private int totalPages = 1;


        [ObservableProperty]
        private int totalCount;

        [ObservableProperty]
        private string message = string.Empty;


        [ObservableProperty]
        private bool isLoading;


        public int PageSize { get; } = 10;


        public bool IsEditing =>
            _editingId.HasValue;


        partial void OnSelectedStudentChanged(
            StudentListItem? value)
        {
            if (value is null)
            {
                return;
            }


            _editingId =
                value.Id;


            FirstName =
                value.FirstName;


            LastName =
                value.LastName;


            Email =
                value.Email;


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
                IsLoading = true;


                if (Groups.Count == 0)
                {
                    await LoadGroupsAsync();
                }


                int? groupId = null;


                if (IsGroupFilterEnabled &&
                    FilterGroup is not null)
                {
                    groupId =
                        FilterGroup.Id;
                }


                PagedResult<StudentListItem> result =
                    await _studentService
                        .GetPagedAsync(
                            SearchText,
                            groupId,
                            CurrentPage,
                            PageSize
                        );


                Students.Clear();


                foreach (StudentListItem student in result.Items)
                {
                    Students.Add(student);
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


        private async Task LoadGroupsAsync()
        {
            IReadOnlyList<Group> groups =
                await _groupService
                    .GetAllAsync();


            Groups.Clear();


            foreach (Group group in groups)
            {
                Groups.Add(group);
            }


            if (Groups.Count > 0)
            {
                FilterGroup ??=
                    Groups[0];


                if (!_editingId.HasValue)
                {
                    SelectedGroup ??=
                        Groups[0];
                }
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


            IsGroupFilterEnabled =
                false;


            FilterGroup =
                Groups.FirstOrDefault();


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
                    await _studentService.UpdateAsync(
                        _editingId.Value,
                        FirstName,
                        LastName,
                        Email,
                        SelectedGroup.Id
                    );


                    ResetForm(
                        clearMessage: false
                    );


                    Message =
                        "Студента успішно оновлено.";
                }
                else
                {
                    await _studentService.CreateAsync(
                        FirstName,
                        LastName,
                        Email,
                        SelectedGroup.Id
                    );


                    ResetForm(
                        clearMessage: false
                    );


                    Message =
                        "Студента успішно створено.";
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
            if (SelectedStudent is null)
            {
                Message =
                    "Спочатку виберіть студента.";

                return;
            }


            try
            {
                int id =
                    SelectedStudent.Id;


                await _studentService
                    .DeleteAsync(id);


                ResetForm(
                    clearMessage: false
                );


                Message =
                    "Студента успішно видалено.";


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
            SelectedStudent =
                null;


            _editingId =
                null;


            FirstName =
                string.Empty;


            LastName =
                string.Empty;


            Email =
                string.Empty;


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