using System.Windows;

namespace Reversi
{
    public partial class NowaGraDialog : Window
    {
        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }

        public NowaGraDialog()
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
                    MessageBox.Show("Szerokoœæ i wysokoœæ musz¹ byæ w zakresie od 4 do 26.", "B³êdna wartoœæ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("WprowadŸ prawid³owe liczby ca³kowite dla szerokoœci i wysokoœci.", "B³êdne dane", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
