using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DataTracker.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ScriptViewer.xaml
    /// </summary>
    public partial class ScriptViewer : Window
    {
        public ScriptViewer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
