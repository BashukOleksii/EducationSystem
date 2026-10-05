using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EducationSystem.Controls
{
    public partial class TableSearchControl : UserControl
    {
        private string _activeSearchText =
            string.Empty;


        public TableSearchControl()
        {
            InitializeComponent();
        }


        public DataGrid? TargetDataGrid
        {
            get =>
                (DataGrid?)GetValue(
                    TargetDataGridProperty
                );

            set =>
                SetValue(
                    TargetDataGridProperty,
                    value
                );
        }


        public static readonly DependencyProperty
            TargetDataGridProperty =
                DependencyProperty.Register(
                    nameof(TargetDataGrid),
                    typeof(DataGrid),
                    typeof(TableSearchControl),
                    new PropertyMetadata(null)
                );

        private void SearchTextBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            _activeSearchText =
                string.Empty;


            MatchInfoTextBlock.Text =
                string.Empty;


            PreviousButton.IsEnabled =
                false;


            NextButton.IsEnabled =
                false;
        }
        private void SearchButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            StartSearch();
        }

        private void PreviousButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MoveToMatch(-1);
        }


        private void NextButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MoveToMatch(1);
        }

        private void StartSearch()
        {
            if (TargetDataGrid is null)
            {
                return;
            }


            string searchText =
                SearchTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                searchText))
            {
                _activeSearchText =
                    string.Empty;


                MatchInfoTextBlock.Text =
                    string.Empty;


                PreviousButton.IsEnabled =
                    false;


                NextButton.IsEnabled =
                    false;


                return;
            }

            _activeSearchText =
                searchText;


            List<object> matches =
                GetMatches(
                    _activeSearchText
                );


            if (matches.Count == 0)
            {
                MatchInfoTextBlock.Text =
                    "0 / 0";


                PreviousButton.IsEnabled =
                    false;


                NextButton.IsEnabled =
                    false;


                return;
            }


            PreviousButton.IsEnabled =
                true;


            NextButton.IsEnabled =
                true;


            SelectItem(
                matches,
                0
            );
        }


        private void MoveToMatch(
            int direction)
        {
            if (TargetDataGrid is null)
            {
                return;
            }


            if (string.IsNullOrWhiteSpace(
                _activeSearchText))
            {
                return;
            }


            List<object> matches =
                GetMatches(
                    _activeSearchText
                );


            if (matches.Count == 0)
            {
                MatchInfoTextBlock.Text =
                    "0 / 0";


                PreviousButton.IsEnabled =
                    false;


                NextButton.IsEnabled =
                    false;


                return;
            }


            object? selectedItem =
                TargetDataGrid.SelectedItem;


            int currentIndex =
                selectedItem is null
                    ? -1
                    : matches.IndexOf(
                        selectedItem
                    );


            int nextIndex;


            if (currentIndex < 0)
            {
                nextIndex =
                    direction > 0
                        ? 0
                        : matches.Count - 1;
            }
            else
            {
                nextIndex =
                    currentIndex + direction;


                if (nextIndex >= matches.Count)
                {
                    nextIndex = 0;
                }
                if (nextIndex < 0)
                {
                    nextIndex =
                        matches.Count - 1;
                }
            }


            SelectItem(
                matches,
                nextIndex
            );
        }

        private void SelectItem(
            List<object> matches,
            int index)
        {
            if (TargetDataGrid is null)
            {
                return;
            }


            object item =
                matches[index];


            TargetDataGrid.SelectedItem =
                item;

            TargetDataGrid.ScrollIntoView(
                item
            );


            TargetDataGrid.Focus();

            MatchInfoTextBlock.Text =
                $"{index + 1} / {matches.Count}";
        }

        private List<object> GetMatches(
            string searchText)
        {
            List<object> result =
                new();


            if (TargetDataGrid is null)
            {
                return result;
            }


            foreach (object item
                     in TargetDataGrid.Items)
            {
                if (item ==
                    CollectionView.NewItemPlaceholder)
                {
                    continue;
                }


                if (ItemMatches(
                    item,
                    searchText))
                {
                    result.Add(
                        item
                    );
                }
            }


            return result;
        }



        private bool ItemMatches(
            object item,
            string searchText)
        {
            if (TargetDataGrid is null)
            {
                return false;
            }


            foreach (DataGridColumn column
                     in TargetDataGrid.Columns)
            {
                if (column
                    is not DataGridBoundColumn
                    boundColumn)
                {
                    continue;
                }


                if (boundColumn.Binding
                    is not Binding binding)
                {
                    continue;
                }


                string? propertyPath =
                    binding.Path?.Path;


                if (string.IsNullOrWhiteSpace(
                    propertyPath))
                {
                    continue;
                }


                object? value =
                    GetPropertyValue(
                        item,
                        propertyPath
                    );


                if (value is null)
                {
                    continue;
                }


                string valueText =
                    ConvertValueToText(
                        value,
                        binding.StringFormat
                    );


                if (valueText.Contains(
                    searchText,
                    StringComparison
                        .CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }


            return false;
        }

        private static object? GetPropertyValue(
            object source,
            string propertyPath)
        {
            object? currentValue =
                source;


            string[] properties =
                propertyPath.Split('.');


            foreach (string propertyName
                     in properties)
            {
                if (currentValue is null)
                {
                    return null;
                }


                PropertyInfo? property =
                    currentValue
                        .GetType()
                        .GetProperty(
                            propertyName
                        );


                if (property is null)
                {
                    return null;
                }


                currentValue =
                    property.GetValue(
                        currentValue
                    );
            }


            return currentValue;
        }


        private static string ConvertValueToText(
            object value,
            string? format)
        {
            if (string.IsNullOrWhiteSpace(
                format))
            {
                return value.ToString()
                       ?? string.Empty;
            }


            if (value
                is IFormattable formattable)
            {
                try
                {
                    return formattable
                               .ToString(
                                   format,
                                   CultureInfo
                                       .CurrentCulture
                               )
                           ?? string.Empty;
                }
                catch (FormatException)
                {
                }
            }


            return value.ToString()
                   ?? string.Empty;
        }
    }
}