using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Linq;

namespace EducationSystem.ViewModels
{
    public partial class SubjectsViewModel : ObservableObject
    {
        private readonly SubjectService _subjectService;

        private int? _editingId;

        public SubjectsViewModel(
            SubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        public ObservableCollection<Subject> Subjects { get; }
            = new();

        [ObservableProperty]
        private Subject? selectedSubject;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private int duration = 1;

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

        partial void OnSelectedSubjectChanged(
            Subject? value)
        {
            if (value is null)
            {
                return;
            }

            _editingId = value.Id;

            Name = value.Name;
            Duration = value.Duration;

            Message = string.Empty;

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
                Message = string.Empty;

                PagedResult<Subject> result =
                    await _subjectService.GetPagedAsync(
                        SearchText,
                        CurrentPage,
                        PageSize
                    );

                Subjects.Clear();

                foreach (Subject subject in result.Items)
                {
                    Subjects.Add(subject);
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
            CurrentPage = 1;

            await LoadAsync();
        }

        [RelayCommand]
        private async Task ClearSearchAsync()
        {
            SearchText =
                string.Empty;

            CurrentPage = 1;

            await LoadAsync();
        }

        [RelayCommand]
        private void New()
        {
            SelectedSubject = null;

            _editingId = null;

            Name = string.Empty;

            Duration = 1;

            Message = string.Empty;

            OnPropertyChanged(
                nameof(IsEditing)
            );
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                Message = string.Empty;

                if (_editingId.HasValue)
                {
                    await _subjectService.UpdateAsync(
                        _editingId.Value,
                        Name,
                        Duration
                    );

                    Message =
                        "Предмет успішно оновлено.";
                }
                else
                {
                    await _subjectService.CreateAsync(
                        Name,
                        Duration
                    );

                    Message =
                        "Предмет успішно створено.";
                }

                SelectedSubject = null;

                _editingId = null;

                Name = string.Empty;
                Duration = 1;

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
            if (SelectedSubject is null)
            {
                Message =
                    "Спочатку виберіть предмет.";

                return;
            }

            try
            {
                int id =
                    SelectedSubject.Id;

                await _subjectService
                    .DeleteAsync(id);

                SelectedSubject = null;

                _editingId = null;

                Name = string.Empty;
                Duration = 1;

                Message =
                    "Предмет успішно видалено.";

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