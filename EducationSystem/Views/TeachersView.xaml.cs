using System.Windows;
using System.Windows.Controls;
using EducationSystem.ViewModels;

namespace EducationSystem.Views
{
    public partial class TeachersView : UserControl
    {
        private readonly TeachersViewModel _viewModel;


        public TeachersView(
            TeachersViewModel viewModel)
        {
            InitializeComponent();


            _viewModel =
                viewModel;


            DataContext =
                _viewModel;


            Loaded +=
                TeachersView_Loaded;
        }


        private async void TeachersView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -=
                TeachersView_Loaded;


            await _viewModel
                .LoadAsync();
        }
    }
}