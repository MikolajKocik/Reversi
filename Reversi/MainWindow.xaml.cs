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
using System.Windows.Threading;

namespace Reversi
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button[,] plansza;
        private ReversiSilnikAI silnik = new ReversiSilnikAI(1);
        private SolidColorBrush[] kolory = { Brushes.Beige, Brushes.Green, Brushes.Brown };
        private string[] nazwyGraczy = {"", "zielony", "brązowy" };
        private bool graPrzeciwkoKomputerowi = true;
        private DispatcherTimer tmr;

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

        private WspółrzędnePola? ustalNajlepszyRuch()
        {
            if(silnik.LiczbaPustychPól == 0)
            {
                MessageBox.Show("Brak wolnych pól");
                return null;
            }

            try
            {
                int poziomo, pionowo;
                silnik.ProponujNajlepszyRuch(out poziomo, out pionowo);
                return new WspółrzędnePola() { Poziomo = poziomo, Pionowo = pionowo };
            }
            catch
            {
                MessageBox.Show("Gracze nie może wykonać ruchu");
                return null;
            }
        }

        private void zaznaczNajlepszyRuch()
        {
            WspółrzędnePola? wspPola = ustalNajlepszyRuch();
            if(wspPola.HasValue)
            {
                SolidColorBrush kolorPodpowiedzi = 
                    kolory[silnik.NumerGraczaWykonujacegoNastepnyRuch]
                    .Lerp(kolory[0]);
                plansza[wspPola.Value.Poziomo, wspPola.Value.Pionowo].Background
                    = kolorPodpowiedzi;
            }
        }

        public void wykonajNajlepszyRuch()
        {
            WspółrzędnePola? wspPola = ustalNajlepszyRuch();
            if (wspPola.HasValue)
            {
                Button przycisk = plansza[wspPola.Value.Poziomo, wspPola.Value.Pionowo];
                klikniętoPolaPlanszy(przycisk, null);
            }
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

            ReversiSilnik.SytuacjaNaPlanszy sytuacjaNaPlanszy = 
                silnik.ZbadajSytuacjęNaPlanszy();

            bool koniecGry = false;
            switch (sytuacjaNaPlanszy)
            {
                case ReversiSilnik.SytuacjaNaPlanszy.WszystkiePolaPlanszySąZajęte:
                    koniecGry = true;
                    break;
                case ReversiSilnik.SytuacjaNaPlanszy.BieżącyGraczNieMożeWykonaćRuchu:
                    MessageBox.Show("Gracz " + nazwyGraczy[silnik.NumerGraczaWykonujacegoNastepnyRuch] 
                        + " nie może wykonać ruchu");
                    silnik.Pasuj();
                    UzgodnijZawartośćPlanszy();
                    break;
                case ReversiSilnik.SytuacjaNaPlanszy.ObajGraczeNieMogąWykonaćRuchu:
                    MessageBox.Show("Obaj gracze nie mogą wykonać ruchu");
                    koniecGry = true;
                    break;
            }

            if (koniecGry)
            {
                int numerZwycięzcy = silnik.NumerGraczaMającegoPrzewagę;
                if (numerZwycięzcy != 0)
                    MessageBox.Show("Wygrał gracz " + nazwyGraczy[numerZwycięzcy],
                        Title, MessageBoxButton.OK, MessageBoxImage.Information);
                else MessageBox.Show("Remis ", Title,
                    MessageBoxButton.OK, MessageBoxImage.Information);

                 if (MessageBox.Show("Czy rozpocząć grę od nowa?", "Reversi", MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                 {
                    przygotowaniePlanszyDoNowejGry(1, 8, 8);
                 }
                 else
                 {
                    planszaSiatka.IsEnabled = false;
                    przyciskKolorGracza.IsEnabled = false;
                 }
            }
            else
            {
                if (graPrzeciwkoKomputerowi && silnik.NumerGraczaWykonujacegoNastepnyRuch == 2)
                {
                    if(tmr == null)
                    {
                        //tmr = new DispatcherTimer();
                        //tmr.Interval = new TimeSpan(0, 0, 0, 0, 300);
                        //tmr.Tick += (_sender, _e) =>
                        //{ tmr.IsEnabled = false; wykonajNajlepszyRuch(); };
                    }               
                }
                //wykonajNajlepszyRuch();
            }
        }

        private void przygotowaniePlanszyDoNowejGry(
            int numerGraczaRozpoczynającego,
            int szerokośćPlanszy = 8,
            int wysokośćPlanszy = 8)
        {
            silnik = new ReversiSilnikAI(numerGraczaRozpoczynającego,
                szerokośćPlanszy, wysokośćPlanszy);

            ListaRuchówZielony.Items.Clear();
            ListaRuchówBrązowy.Items.Clear();
            planszaSiatka.IsEnabled = true;
            przyciskKolorGracza.IsEnabled = true;
            UzgodnijZawartośćPlanszy();
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

        private void przyciskKolorGracza_Click(object sender, RoutedEventArgs e)
        {
            zaznaczNajlepszyRuch();
        }
    }
}
