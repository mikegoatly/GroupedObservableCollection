// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DemoApp.UWP
{
    #region Using statements

    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    using Munklesoft.Common.Collections;

    #endregion

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GroupedObservableCollection<char, string> data;

        private static string[] initialData = new[]
                                       {
                                           "Apples",
                                           "Bananas",
                                           "Peaches",
                                           "Pears",
                                       };

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += this.MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.data = new GroupedObservableCollection<char, string>(s => s[0], initialData);

            this.Source.Source = this.data;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            this.data.Add(this.TextBox.Text);
        }

        private void RemoveItemClick(object sender, RoutedEventArgs e)
        {
            this.data.Remove((string)((FrameworkElement)sender).DataContext);
        }

        private void Reset_OnClick(object sender, RoutedEventArgs e)
        {
            this.data.ReplaceWith(new GroupedObservableCollection<char, string>(s => s[0], initialData), StringComparer.OrdinalIgnoreCase);
        }
    }
}
