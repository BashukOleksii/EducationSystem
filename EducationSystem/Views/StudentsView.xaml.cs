using System.Windows;
using System.Windows.Controls;
using EducationSystem.ViewModels;

namespace EducationSystem.Views
{
    public partial class StudentsView : System.Windows.Controls.UserControl
    {
        private readonly StudentsViewModel _viewModel;


        public StudentsView(
            StudentsViewModel viewModel)
        {
            InitializeComponent();


            _viewModel =
                viewModel;


            DataContext =
                _viewModel;


            Loaded +=
                StudentsView_Loaded;
        }


        private async void StudentsView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -=
                StudentsView_Loaded;


            await _viewModel
                .LoadAsync();
        }
    }
}