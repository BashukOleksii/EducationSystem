using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Analytics;
using EducationSystem.DTOs.Students;
using EducationSystem.Models;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels
{
    public partial class AnalyticsViewModel
        : ObservableObject
    {
        private readonly AnalyticsService _analyticsService;

        private readonly TeacherService _teacherService;

        private readonly SubjectService _subjectService;

        private readonly StudentService _studentService;

        private readonly GroupService _groupService;


        public AnalyticsViewModel(
            AnalyticsService analyticsService,
            TeacherService teacherService,
            SubjectService subjectService,
            StudentService studentService,
            GroupService groupService)
        {
            _analyticsService =
                analyticsService;

            _teacherService =
                teacherService;

            _subjectService =
                subjectService;

            _studentService =
                studentService;

            _groupService =
                groupService;
        }

        public ObservableCollection<Teacher> Teachers { get; }
            = new();


        public ObservableCollection<Subject> Subjects { get; }
            = new();


        public ObservableCollection<StudentListItem> Students { get; }
            = new();


        public ObservableCollection<Group> Groups { get; }
            = new();


        public ObservableCollection<TeacherSubjectStatisticsItem>
            TeacherSubjectStatistics
        { get; }
                = new();


        public ObservableCollection<TeacherWorkloadItem>
            TeacherWorkloads
        { get; }
                = new();


        public ObservableCollection<LessonDetailsItem>
            TeacherLessons
        { get; }
                = new();


        public ObservableCollection<TeacherWorkloadItem>
            TeachersWithoutLessons
        { get; }
                = new();


        public ObservableCollection<SubjectStatisticsItem>
            SubjectStatistics
        { get; }
                = new();


        public ObservableCollection<LessonDetailsItem>
            SubjectLessons
        { get; }
                = new();

        public ObservableCollection<StudentSubjectStatisticsItem>
            StudentSubjectStatistics
        { get; }
                = new();


        public ObservableCollection<StudentLessonResultItem>
            StudentSubjectDetails
        { get; }
                = new();


        public ObservableCollection<StudentOverallStatisticsItem>
            StudentOverallStatistics
        { get; }
                = new();


        public ObservableCollection<StudentOverallStatisticsItem>
            AtRiskStudents
        { get; }
                = new();


        public ObservableCollection<StudentSubjectStatisticsItem>
            TopStudents
        { get; }
                = new();


        public ObservableCollection<GroupSubjectStatisticsItem>
            GroupSubjectStatistics
        { get; }
                = new();


        [ObservableProperty]
        private Teacher? selectedTeacherFilter;


        [ObservableProperty]
        private Subject? selectedSubjectFilter;


        [ObservableProperty]
        private StudentListItem? selectedStudentFilter;


        [ObservableProperty]
        private Group? selectedGroupFilter;


        [ObservableProperty]
        private decimal maxAverageGrade = 7;


        [ObservableProperty]
        private int minMissingCount = 3;


        [ObservableProperty]
        private int topCount = 5;


        [ObservableProperty]
        private bool isLoading;


        [ObservableProperty]
        private string message =
            string.Empty;


        [ObservableProperty]
        private string detailsTitle =
            string.Empty;

        [RelayCommand]
        public async Task LoadAsync()
        {
            await ExecuteAsync(
                async () =>
                {

                    IReadOnlyList<Teacher> teachers =
                        await _teacherService
                            .GetAllAsync();


                    IReadOnlyList<Subject> subjects =
                        await _subjectService
                            .GetAllAsync();


                    IReadOnlyList<StudentListItem> students =
                        await _studentService
                            .GetAllAsync();


                    IReadOnlyList<Group> groups =
                        await _groupService
                            .GetAllAsync();


                    ReplaceCollection(
                        Teachers,
                        teachers
                    );


                    ReplaceCollection(
                        Subjects,
                        subjects
                    );


                    ReplaceCollection(
                        Students,
                        students
                    );


                    ReplaceCollection(
                        Groups,
                        groups
                    );



                    IReadOnlyList<TeacherWorkloadItem>
                        teacherWorkloads =
                            await _analyticsService
                                .GetTeacherWorkloadAsync();


                    IReadOnlyList<SubjectStatisticsItem>
                        subjectStatistics =
                            await _analyticsService
                                .GetSubjectStatisticsAsync();


                    IReadOnlyList<StudentOverallStatisticsItem>
                        studentStatistics =
                            await _analyticsService
                                .GetStudentOverallStatisticsAsync();


                    IReadOnlyList<GroupSubjectStatisticsItem>
                        groupStatistics =
                            await _analyticsService
                                .GetGroupSubjectStatisticsAsync();


                    ReplaceCollection(
                        TeacherWorkloads,
                        teacherWorkloads
                    );


                    ReplaceCollection(
                        SubjectStatistics,
                        subjectStatistics
                    );


                    ReplaceCollection(
                        StudentOverallStatistics,
                        studentStatistics
                    );


                    ReplaceCollection(
                        GroupSubjectStatistics,
                        groupStatistics
                    );
                }
            );
        }

        [RelayCommand]
        private async Task LoadTeacherSubjectStatisticsAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    int? teacherId =
                        SelectedTeacherFilter?.Id;


                    int? subjectId =
                        SelectedSubjectFilter?.Id;


                    IReadOnlyList<TeacherSubjectStatisticsItem> result =
                        await _analyticsService
                            .GetTeacherSubjectStatisticsAsync(
                                teacherId,
                                subjectId
                            );


                    ReplaceCollection(
                        TeacherSubjectStatistics,
                        result
                    );
                }
            );
        }


        

        [RelayCommand]
        private async Task LoadTeacherWorkloadAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<TeacherWorkloadItem> result =
                        await _analyticsService
                            .GetTeacherWorkloadAsync();


                    ReplaceCollection(
                        TeacherWorkloads,
                        result
                    );
                }
            );
        }


        [RelayCommand]
        private async Task ShowTeacherLessonsAsync(
            TeacherWorkloadItem? item)
        {
            if (item is null)
            {
                Message =
                    "Необхідно вибрати викладача.";

                return;
            }


            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<LessonDetailsItem> result =
                        await _analyticsService
                            .GetTeacherLessonsAsync(
                                item.TeacherId
                            );


                    ReplaceCollection(
                        TeacherLessons,
                        result
                    );


                    DetailsTitle =
                        $"Заняття викладача: {item.TeacherFullName}";
                }
            );
        }



        [RelayCommand]
        private async Task LoadSubjectStatisticsAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<SubjectStatisticsItem> result =
                        await _analyticsService
                            .GetSubjectStatisticsAsync();


                    ReplaceCollection(
                        SubjectStatistics,
                        result
                    );
                }
            );
        }



        [RelayCommand]
        private async Task ShowSubjectLessonsAsync(
            SubjectStatisticsItem? item)
        {
            if (item is null)
            {
                Message =
                    "Необхідно вибрати предмет.";

                return;
            }


            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<LessonDetailsItem> result =
                        await _analyticsService
                            .GetSubjectLessonsAsync(
                                item.SubjectId
                            );


                    ReplaceCollection(
                        SubjectLessons,
                        result
                    );


                    DetailsTitle =
                        $"Заняття з предмета: {item.SubjectName}";
                }
            );
        }


        /*
         * =====================================================
         * 6. СТАТИСТИКА СТУДЕНТІВ ЗА ПРЕДМЕТАМИ
         * =====================================================
         */

        [RelayCommand]
        private async Task LoadStudentSubjectStatisticsAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    int? studentId =
                        SelectedStudentFilter?.Id;


                    int? subjectId =
                        SelectedSubjectFilter?.Id;


                    IReadOnlyList<StudentSubjectStatisticsItem> result =
                        await _analyticsService
                            .GetStudentSubjectStatisticsAsync(
                                studentId,
                                subjectId
                            );


                    ReplaceCollection(
                        StudentSubjectStatistics,
                        result
                    );
                }
            );
        }



        [RelayCommand]
        private async Task ShowStudentSubjectDetailsAsync(
            StudentSubjectStatisticsItem? item)
        {
            if (item is null)
            {
                Message =
                    "Необхідно вибрати студента та предмет.";

                return;
            }


            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<StudentLessonResultItem> result =
                        await _analyticsService
                            .GetStudentSubjectDetailsAsync(
                                item.StudentId,
                                item.SubjectId
                            );


                    ReplaceCollection(
                        StudentSubjectDetails,
                        result
                    );


                    DetailsTitle =
                        $"{item.StudentFullName} — {item.SubjectName}";
                }
            );
        }


        [RelayCommand]
        private async Task LoadStudentOverallStatisticsAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<StudentOverallStatisticsItem> result =
                        await _analyticsService
                            .GetStudentOverallStatisticsAsync();


                    ReplaceCollection(
                        StudentOverallStatistics,
                        result
                    );
                }
            );
        }



        [RelayCommand]
        private async Task LoadAtRiskStudentsAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<StudentOverallStatisticsItem> result =
                        await _analyticsService
                            .GetAtRiskStudentsAsync(
                                MaxAverageGrade,
                                MinMissingCount
                            );


                    ReplaceCollection(
                        AtRiskStudents,
                        result
                    );
                }
            );
        }



        [RelayCommand]
        private async Task LoadTopStudentsAsync()
        {
            if (SelectedSubjectFilter is null)
            {
                Message =
                    "Для рейтингу необхідно вибрати предмет.";

                return;
            }


            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<StudentSubjectStatisticsItem> result =
                        await _analyticsService
                            .GetTopStudentsBySubjectAsync(
                                SelectedSubjectFilter.Id,
                                TopCount
                            );


                    ReplaceCollection(
                        TopStudents,
                        result
                    );
                }
            );
        }



        [RelayCommand]
        private async Task LoadGroupSubjectStatisticsAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    int? groupId =
                        SelectedGroupFilter?.Id;


                    int? subjectId =
                        SelectedSubjectFilter?.Id;


                    IReadOnlyList<GroupSubjectStatisticsItem> result =
                        await _analyticsService
                            .GetGroupSubjectStatisticsAsync(
                                groupId,
                                subjectId
                            );


                    ReplaceCollection(
                        GroupSubjectStatistics,
                        result
                    );
                }
            );
        }



        [RelayCommand]
        private async Task LoadTeachersWithoutLessonsAsync()
        {
            await ExecuteAsync(
                async () =>
                {
                    IReadOnlyList<TeacherWorkloadItem> result =
                        await _analyticsService
                            .GetTeachersWithoutLessonsAsync();


                    ReplaceCollection(
                        TeachersWithoutLessons,
                        result
                    );
                }
            );
        }


        [RelayCommand]
        private void ClearFilters()
        {
            SelectedTeacherFilter =
                null;


            SelectedSubjectFilter =
                null;


            SelectedStudentFilter =
                null;


            SelectedGroupFilter =
                null;


            MaxAverageGrade =
                7;


            MinMissingCount =
                3;


            TopCount =
                5;


            Message =
                string.Empty;
        }


        private async Task ExecuteAsync(
            Func<Task> operation)
        {
            try
            {
                IsLoading =
                    true;


                Message =
                    string.Empty;


                await operation();
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



        private static void ReplaceCollection<T>(
            ObservableCollection<T> collection,
            IEnumerable<T> items)
        {
            collection.Clear();


            foreach (T item in items)
            {
                collection.Add(
                    item
                );
            }
        }
    }
}