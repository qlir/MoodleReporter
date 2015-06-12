using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ReportsGenerator.Settings;
using MessageBox = UIReporter.AccessoryWindow.MessageBox;

namespace UIReporter.SettingsViews
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SystemDataSettingGrid : Grid
    {

        public SystemDataSettingGrid()
        {
            InitializeComponent();
            AddField("Имя письма при сохранении письма", "EmailName");
            AddField("Имя католога при сохранении письма(от формата даты)", "DateFormatForFolderName");
            AddField("Разделитель .csv", "CsvDelimiter");
            AddField("Кодировка .csv", "CsvEncoding");
        }

        private void AddField(string title, string fieldName)
        {
            Binding myBinding = new Binding(fieldName);
            myBinding.Source = ReporterSettings.Default;
            var f = new SettingsField();
            f.Label.Content = title;
            BindingOperations.SetBinding(f.TextBox, TextBox.TextProperty, myBinding);
            FieldsContainer.Children.Add(f);
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            ReporterSettings.Default.Save();
        }

        private void Discard_OnClick(object sender, RoutedEventArgs e)
        {
            ReporterSettings.Default.Reset();
            ReporterSettings.Default.Save();
        }
    }
}
