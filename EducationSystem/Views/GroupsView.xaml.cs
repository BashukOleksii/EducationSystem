using System.Windows;
using System.Windows.Controls;
using EducationSystem.ViewModels;

namespace EducationSystem.Views;

public partial class GroupsView : UserControl
{
    private readonly GroupsViewModel _viewModel;

    public GroupsView(
        GroupsViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        DataContext = _viewModel;

        Loaded += GroupsView_Loaded;
    }

    private async void GroupsView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        Loaded -= GroupsView_Loaded;

        await _viewModel.LoadAsync();
    }
}