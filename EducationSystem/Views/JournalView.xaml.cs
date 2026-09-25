using System.Windows;
using System.Windows.Controls;
using EducationSystem.ViewModels;

namespace EducationSystem.Views
{
    public partial class JournalView : UserControl
    {
        private readonly JournalViewModel _viewModel;


        public JournalView(
            JournalViewModel viewModel)
        {
            InitializeComponent();


            _viewModel =
                viewModel;


            DataContext =
                _viewModel;


            Loaded +=
                JournalView_Loaded;
        }


        private async void JournalView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -=
                JournalView_Loaded;


            await _viewModel
                .LoadAsync();
        }
    }
}