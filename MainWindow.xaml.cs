using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhotoSort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PhotoCollection Photos;
        int index = 0;
        public MainWindow()
        {
            InitializeComponent();
            this.ImagesDir.Text = @"D:\Photo\Yas&Jym\Lina\20140601";
        }

        private void OnImagesDirChangeClick(object sender, RoutedEventArgs e)
        {
            this.Photos.Path = ImagesDir.Text;
            index = 0;
            DisplayImageAtIndex(index);
        }

        private void DisplayImageAtIndex(int index)
        {
            foreach (var item in this.Photos)
            {
                item.Image = null;
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (this.Photos.Count == 0)
            {
                return;
            }

            if (index < 0 || index > this.Photos.Count)
            {
                this.Image.Source = null;
                this.chkPrinted.IsChecked = false;
                rectIsPrinted.Fill = new SolidColorBrush(Colors.White);
            }
            else
            {

                this.Image.Source = this.Photos[index].Image;
                this.chkPrinted.IsChecked = this.Photos[index].Print;
                if (this.Photos[index].Print)
                    rectIsPrinted.Fill = new SolidColorBrush(Colors.LightBlue);
                else
                    rectIsPrinted.Fill = new SolidColorBrush(Colors.LightCoral);
            }
        }

        private void ActionNext(object sender, RoutedEventArgs e)
        {
            index++;

            if (index >= this.Photos.Count)
                index = 0;

            DisplayImageAtIndex(index);
        }

        private void ActionNext10(object sender, RoutedEventArgs e)
        {
            index += 10;

            if (index >= this.Photos.Count)
                index = 0;

            DisplayImageAtIndex(index);
        }

        private void ActionPrevious(object sender, RoutedEventArgs e)
        {
            index--;

            if (index < 0)
                index = this.Photos.Count - 1;

            DisplayImageAtIndex(index);
        }

        private void ActionPrevious10(object sender, RoutedEventArgs e)
        {
            index -= 10;

            if (index < 0)
                index = this.Photos.Count - 1;

            DisplayImageAtIndex(index);
        }

        private void ActionDelete(object sender, RoutedEventArgs e)
        {
            var indexTemp = index;

            if (index + 1 >= this.Photos.Count)
                index = 0;

            DisplayImageAtIndex(index + 1);

            this.Photos.Delete(indexTemp);
        }

        private void ActionPrint(object sender, RoutedEventArgs e)
        {

            this.Photos.Print(index);
            DisplayImageAtIndex(index);

        }

    }
}
