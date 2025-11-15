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
        private ReversiSilnik silnik = new ReversiSilnik(1);
        private SolidColorBrush[] kolory = { Brushes.Beige, Brushes.Green, Brushes.Brown };
        private struct WspółrzędnePola
        {
            public int Poziomo, Pionowo;
        }

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
                    przycisk.Tag = new WspółrzędnePola { Poziomo = i, Pionowo = j };
                    przycisk.Click += new RoutedEventHandler(klikniętoPolaPlanszy); 
                    plansza[i,j] = przycisk;
                }
            UzgodnijZawartośćPlanszy();
            // silnik.PołóżKamień(2, 4);
            UzgodnijZawartośćPlanszy();
            // silnik.PołóżKamień(2, 5);
            UzgodnijZawartośćPlanszy();
        }

        private static string symbolPola(int poziomo, int pionowo)
        {
            if (poziomo > 25 || pionowo > 8) 
                return "(" + poziomo.ToString() + "," + pionowo.ToString() + ")";
            return "" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[poziomo] + "123456789"[pionowo];
        }

        private void klikniętoPolaPlanszy(object sender, RoutedEventArgs e)
        {
            Button klikniętyPrzycisk = sender as Button;
            WspółrzędnePola współrzędne = (WspółrzędnePola)klikniętyPrzycisk.Tag;
            int klikniętePoziomo = współrzędne.Poziomo;
            int klikniętePionowo = współrzędne.Pionowo;
            int numerGracza = silnik.NumerGraczaWykonujacegoNastepnyRuch;
            if (silnik.PołóżKamień(klikniętePoziomo, klikniętePionowo) > 0)
            {
                UzgodnijZawartośćPlanszy();
                switch (numerGracza)
                {
                    case 1:
                        ListaRuchówZielony.Items.Add(symbolPola(klikniętePoziomo, klikniętePionowo));
                        break;
                    case 2:
                        ListaRuchówBrązowy.Items.Add(symbolPola(klikniętePoziomo, klikniętePionowo));
                        break;
                }
            };
        }

        private void UzgodnijZawartośćPlanszy()
        {
            for(int i = 0; i < silnik.SzerokośćPlanszy; i++)
            {
                for(int j = 0; j < silnik.WysokośćPlanszy; j++)
                {
                    plansza[i, j].Content = silnik.PobierzStanPola(i, j).ToString();
                    plansza[i, j].Background = kolory[silnik.PobierzStanPola(i, j)];
                }
            }
            LiczbaPólZielony.Text = silnik.LiczbaPólGracz1.ToString();
            LiczbaPólBrązowy.Text = silnik.LiczbaPólGracz2.ToString();
            przyciskKolorGracza.Background = kolory[silnik.NumerGraczaWykonujacegoNastepnyRuch];
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
