using System;
using System.Windows;
using PresentationLayer.ViewModel;

namespace PresentationLayer.Views
{
    /// <summary>
    /// Interaction logic for LinkObject.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
