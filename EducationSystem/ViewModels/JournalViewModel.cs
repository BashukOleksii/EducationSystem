using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Common;
using EducationSystem.DTOs.Journal;
using EducationSystem.DTOs.Lessons;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels
{
    public partial class JournalViewModel
        : ObservableObject
    {
        private readonly JournalService
            _journalService;

        private readonly LessonService
            _lessonService;


        public JournalViewModel(
            JournalService journalService,
            LessonService lessonService)
        {
            _journalService =
                journalService;

            _lessonService =
                lessonService;
        }


        public ObservableCollection<LessonListItem>
            Lessons
        { get; } = new();


        public ObservableCollection<JournalStudentItem>
            Students
        { get; } = new();


        [ObservableProperty]
        private LessonListItem? selectedLesson;


        [ObservableProperty]
        private string searchText =
            string.Empty;


        /*
         * Додаткові фільтри.
         */
        [ObservableProperty]
        private bool onlyMissing;


        [ObservableProperty]
        private bool onlyWithoutGrade;


        [ObservableProperty]
        private int currentPage = 1;


        [ObservableProperty]
        private int totalPages = 1;


        [ObservableProperty]
        private int totalCount;


        [ObservableProperty]
        private bool isLoading;


        [ObservableProperty]
        private string message =
            string.Empty;


        public int PageSize { get; } = 10;


        [RelayCommand]
        public async Task LoadAsync()
        {
            try
            {
                IsLoading =
                    true;


                IReadOnlyList<LessonListItem> lessons =
                    await _lessonService
                        .GetAllAsync();


                Lessons.Clear();


                foreach (LessonListItem lesson in lessons)
                {
                    Lessons.Add(
                        lesson
                    );
                }


                if (SelectedLesson is null)
                {
                    SelectedLesson =
                        Lessons.FirstOrDefault();
                }


                if (SelectedLesson is not null)
                {
                    await LoadJournalAsync();
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


        [RelayCommand]
        private async Task OpenLessonAsync()
        {
            CurrentPage =
                1;


            await LoadJournalAsync();
        }


        [RelayCommand]
        private async Task SearchAsync()
        {
            CurrentPage =
                1;


            await LoadJournalAsync();
        }


        [RelayCommand]
        private async Task ClearSearchAsync()
        {
            SearchText =
                string.Empty;


            OnlyMissing =
                false;


            OnlyWithoutGrade =
                false;


            CurrentPage =
                1;


            await LoadJournalAsync();
        }


        private async Task LoadJournalAsync()
        {
            if (SelectedLesson is null)
            {
                Students.Clear();

                TotalCount = 0;
                TotalPages = 1;

                return;
            }


            try
            {
                IsLoading =
                    true;


                Message =
                    string.Empty;


                PagedResult<JournalStudentItem> result =
                    await _journalService
                        .GetPagedAsync(
                            SelectedLesson.Id,
                            SearchText,
                            OnlyMissing,
                            OnlyWithoutGrade,
                            CurrentPage,
                            PageSize
                        );


                Students.Clear();


                foreach (
                    JournalStudentItem student
                    in result.Items)
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


                    await LoadJournalAsync();
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


        [RelayCommand]
        private async Task SaveEntryAsync(
            JournalStudentItem? item)
        {
            if (SelectedLesson is null ||
                item is null)
            {
                return;
            }


            try
            {
                Message =
                    string.Empty;


                await _journalService
                    .SaveEntryAsync(
                        SelectedLesson.Id,
                        item
                    );


                Message =
                    $"Дані для студента {item.FullName} збережено.";
            }
            catch (Exception exception)
            {
                Message =
                    exception.Message;
            }
        }


        [RelayCommand]
        private async Task SavePageAsync()
        {
            if (SelectedLesson is null)
            {
                return;
            }


            try
            {
                Message =
                    string.Empty;


                foreach (
                    JournalStudentItem item
                    in Students)
                {
                    await _journalService
                        .SaveEntryAsync(
                            SelectedLesson.Id,
                            item
                        );
                }


                Message =
                    "Поточну сторінку журналу збережено.";


                await LoadJournalAsync();
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


            await LoadJournalAsync();
        }


        [RelayCommand]
        private async Task NextPageAsync()
        {
            if (CurrentPage >= TotalPages)
            {
                return;
            }


            CurrentPage++;


            await LoadJournalAsync();
        }
    }
}