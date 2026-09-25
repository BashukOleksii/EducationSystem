using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Services;
using System.Collections.ObjectModel;

namespace EducationSystem.ViewModels;

public partial class GroupsViewModel : ObservableObject
{
    private readonly GroupService _groupService;

    private int? _editingId;

    public GroupsViewModel(
        GroupService groupService)
    {
        _groupService = groupService;
    }

    public ObservableCollection<Group> Groups { get; }
        = new();

    [ObservableProperty]
    private Group? selectedGroup;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string prefix = string.Empty;

    [ObservableProperty]
    private int number = 1;

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int totalPages = 1;

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private string message = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    public int PageSize { get; } = 10;

    public bool IsEditing =>
        _editingId.HasValue;

    partial void OnSelectedGroupChanged(
        Group? value)
    {
        if (value is null)
        {
            return;
        }

        _editingId = value.Id;

        Prefix = value.Prefix;
        Number = value.Number;

        OnPropertyChanged(nameof(IsEditing));
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            Message = string.Empty;

            PagedResult<Group> result =
                await _groupService.GetPagedAsync(
                    SearchText,
                    CurrentPage,
                    PageSize
                );

            Groups.Clear();

            foreach (Group group in result.Items)
            {
                Groups.Add(group);
            }

            TotalCount = result.TotalCount;

            TotalPages =
                Math.Max(
                    1,
                    result.TotalPages
                );

            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;

                await LoadAsync();

                return;
            }
        }
        catch (Exception exception)
        {
            Message = exception.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        CurrentPage = 1;

        await LoadAsync();
    }

    [RelayCommand]
    private async Task ClearSearchAsync()
    {
        SearchText = string.Empty;
        CurrentPage = 1;

        await LoadAsync();
    }

    [RelayCommand]
    private void New()
    {
        SelectedGroup = null;

        _editingId = null;

        Prefix = string.Empty;
        Number = 1;

        Message = string.Empty;

        OnPropertyChanged(nameof(IsEditing));
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            Message = string.Empty;

            if (_editingId.HasValue)
            {
                await _groupService.UpdateAsync(
                    _editingId.Value,
                    Prefix,
                    Number
                );

                Message =
                    "Групу успішно оновлено.";
            }
            else
            {
                await _groupService.CreateAsync(
                    Prefix,
                    Number
                );

                Message =
                    "Групу успішно створено.";
            }

            New();

            await LoadAsync();
        }
        catch (Exception exception)
        {
            Message = exception.Message;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedGroup is null)
        {
            Message =
                "Спочатку виберіть групу.";

            return;
        }

        try
        {
            int id =
                SelectedGroup.Id;

            await _groupService.DeleteAsync(
                id
            );

            New();

            Message =
                "Групу видалено.";

            await LoadAsync();
        }
        catch (Exception exception)
        {
            Message = exception.Message;
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

        await LoadAsync();
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (CurrentPage >= TotalPages)
        {
            return;
        }

        CurrentPage++;

        await LoadAsync();
    }
}