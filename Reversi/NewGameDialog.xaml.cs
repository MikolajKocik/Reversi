using System.Windows;

namespace Reversi
{
    public partial class NewGameDialog : Window
    {
        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }

        public NewGameDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WidthTextBox.Text, out int width) && int.TryParse(HeightTextBox.Text, out int height))
            {
                if (width >= 4 && width <= 26 && height >= 4 && height <= 26)
                {
                    BoardWidth = width;
                    BoardHeight = height;
                    DialogResult = true;
                }
                else
                {
                    MessageBox.Show("Width and height must be between 4 and 26.", "Invalid Value", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Enter valid integers for width and height.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
