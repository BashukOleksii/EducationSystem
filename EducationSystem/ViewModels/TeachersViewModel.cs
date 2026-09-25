using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Models.Enums;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels
{
    public partial class TeachersViewModel : ObservableObject
    {
        private readonly TeacherService _teacherService;

        private int? _editingId;


        public TeachersViewModel(
            TeacherService teacherService)
        {
            _teacherService =
                teacherService;
        }


        public ObservableCollection<Teacher> Teachers { get; }
            = new();


        public IReadOnlyList<TeacherCategory> Categories { get; }
            = Enum
                .GetValues<TeacherCategory>()
                .ToList();


        [ObservableProperty]
        private Teacher? selectedTeacher;


        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isCategoryFilterEnabled;


        [ObservableProperty]
        private TeacherCategory filterCategory =
            TeacherCategory.I;


        [ObservableProperty]
        private string firstName = string.Empty;


        [ObservableProperty]
        private string lastName = string.Empty;


        [ObservableProperty]
        private string email = string.Empty;


        [ObservableProperty]
        private TeacherCategory selectedCategory =
            TeacherCategory.I;

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


        partial void OnSelectedTeacherChanged(
            Teacher? value)
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


            SelectedCategory =
                value.Category;


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


                TeacherCategory? category = null;


                if (IsCategoryFilterEnabled)
                {
                    category =
                        FilterCategory;
                }


                PagedResult<Teacher> result =
                    await _teacherService
                        .GetPagedAsync(
                            SearchText,
                            category,
                            CurrentPage,
                            PageSize
                        );


                Teachers.Clear();


                foreach (Teacher teacher in result.Items)
                {
                    Teachers.Add(teacher);
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


            IsCategoryFilterEnabled =
                false;


            FilterCategory =
                TeacherCategory.I;


            CurrentPage =
                1;


            Message =
                string.Empty;


            await LoadAsync();
        }


        [RelayCommand]
        private void New()
        {
            SelectedTeacher =
                null;


            _editingId =
                null;


            FirstName =
                string.Empty;


            LastName =
                string.Empty;


            Email =
                string.Empty;


            SelectedCategory =
                TeacherCategory.I;


            Message =
                string.Empty;


            OnPropertyChanged(
                nameof(IsEditing)
            );
        }


        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                Message =
                    string.Empty;


                if (_editingId.HasValue)
                {
                    await _teacherService.UpdateAsync(
                        _editingId.Value,
                        FirstName,
                        LastName,
                        Email,
                        SelectedCategory
                    );


                    Message =
                        "Викладача успішно оновлено.";
                }
                else
                {
                    await _teacherService.CreateAsync(
                        FirstName,
                        LastName,
                        Email,
                        SelectedCategory
                    );


                    Message =
                        "Викладача успішно створено.";
                }


                SelectedTeacher =
                    null;


                _editingId =
                    null;


                FirstName =
                    string.Empty;


                LastName =
                    string.Empty;


                Email =
                    string.Empty;


                SelectedCategory =
                    TeacherCategory.I;


                OnPropertyChanged(
                    nameof(IsEditing)
                );


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
            if (SelectedTeacher is null)
            {
                Message =
                    "Спочатку виберіть викладача.";

                return;
            }


            try
            {
                int id =
                    SelectedTeacher.Id;


                await _teacherService
                    .DeleteAsync(id);


                SelectedTeacher =
                    null;


                _editingId =
                    null;


                FirstName =
                    string.Empty;


                LastName =
                    string.Empty;


                Email =
                    string.Empty;


                SelectedCategory =
                    TeacherCategory.I;


                Message =
                    "Викладача успішно видалено.";


                OnPropertyChanged(
                    nameof(IsEditing)
                );


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
    }
}