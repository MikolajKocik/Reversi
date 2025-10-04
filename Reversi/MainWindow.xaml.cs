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

namespace Reversi
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button[,] plansza;
        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 8; i++)
                planszaSiatka.ColumnDefinitions.Add(new ColumnDefinition());
            
            for (int i = 0; i < 8; i++)           
                planszaSiatka.RowDefinitions.Add(new RowDefinition());

            plansza = new Button[8, 8];
            for(int i = 0;i < 8; i++)
                for(int j=0;j<8;j++)
                {
                    Button przycisk = new Button();
                    planszaSiatka.Children.Add(przycisk);
                    Grid.SetColumn(przycisk, i);
                    Grid.SetRow(przycisk, j);
                    plansza[i,j] = przycisk;
                }
            
        }
        #region
        private void MenuItem_NowaGraDla1GraczaRozpoczynaKopmuter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_NowaGraDla1GraczaRozpoczynaszTy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_NowaGraDla2Graczy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_RuchWykonanyPrzezKomputer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_PodpowiedźRuchu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_ZasadyGry_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_StrategiaKomputera_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_OProgramie_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Zamknij_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
