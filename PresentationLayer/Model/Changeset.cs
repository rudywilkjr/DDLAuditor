using System.Windows;
using System.Windows.Controls;
using Core;

namespace PresentationLayer.Model
{
    public class Changeset
    {
        public User Developer { get; set; }
        public int Id { get; set; }
        public string Comment { get; set; }
        public string Files { get; set; }

        private void Button_RequestCodeReview(object sender, RoutedEventArgs e)
        {
            object id = ((Button)sender).CommandParameter;
        }
    }
}
