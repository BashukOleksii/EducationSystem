using EducationSystem.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace EducationSystem.Views
{
    public partial class AnalyticsView
        : UserControl
    {
        private readonly AnalyticsViewModel _viewModel;


        public AnalyticsView(
            AnalyticsViewModel viewModel)
        {
            InitializeComponent();


            _viewModel =
                viewModel;


            DataContext =
                _viewModel;


            Loaded +=
                AnalyticsView_Loaded;
        }


        private async void AnalyticsView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -=
                AnalyticsView_Loaded;


            await _viewModel
                .LoadAsync();
        }
    }
}