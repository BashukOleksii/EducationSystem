using System.Windows;
using System.Windows.Controls;
using EducationSystem.ViewModels;

namespace EducationSystem.Views
{
    public partial class SubjectsView : UserControl
    {
        private readonly SubjectsViewModel _viewModel;

        public SubjectsView(
            SubjectsViewModel viewModel)
        {
            InitializeComponent();

            _viewModel =
                viewModel;

            DataContext =
                _viewModel;

            Loaded +=
                SubjectsView_Loaded;
        }

        private async void SubjectsView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -=
                SubjectsView_Loaded;

            await _viewModel.LoadAsync();
        }
    }
}