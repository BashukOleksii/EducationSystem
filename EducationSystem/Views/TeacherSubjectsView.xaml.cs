using System.Windows;
using System.Windows.Controls;
using EducationSystem.ViewModels;

namespace EducationSystem.Views
{
    public partial class TeacherSubjectsView
        : UserControl
    {
        private readonly TeacherSubjectsViewModel
            _viewModel;


        public TeacherSubjectsView(
            TeacherSubjectsViewModel viewModel)
        {
            InitializeComponent();


            _viewModel =
                viewModel;


            DataContext =
                _viewModel;


            Loaded +=
                TeacherSubjectsView_Loaded;
        }


        private async void TeacherSubjectsView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -=
                TeacherSubjectsView_Loaded;


            await _viewModel
                .LoadAsync();
        }
    }
}