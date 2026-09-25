using System.Windows;
using System.Windows.Controls;
using EducationSystem.ViewModels;

namespace EducationSystem.Views
{
    public partial class LessonsView : UserControl
    {
        private readonly LessonsViewModel _viewModel;


        public LessonsView(
            LessonsViewModel viewModel)
        {
            InitializeComponent();


            _viewModel =
                viewModel;


            DataContext =
                _viewModel;


            Loaded +=
                LessonsView_Loaded;
        }


        private async void LessonsView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -=
                LessonsView_Loaded;


            await _viewModel
                .LoadAsync();
        }
    }
}