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
            
            planszaSiatka.ColumnDefinitions.Clear();
            planszaSiatka.RowDefinitions.Clear();
            planszaSiatka.Children.Clear();
        }

        private WspółrzędnePola? ustalNajlepszyRuch()
        {
            if (!planszaSiatka.IsEnabled) return null;

            if(silnik.LiczbaPustychPól == 0)
            {
                MessageBox.Show("Brak wolnych pól", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Gracze nie może wykonać ruchu", Title, MessageBoxButton.OK, MessageBoxImage.Warning);
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
            if (poziomo > 25 || pionowo > 25) 
                return "(" + poziomo.ToString() + "," + pionowo.ToString() + ")";
            return "" + "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[poziomo] + (pionowo + 1).ToString();
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
                    przygotowaniePlanszyDoNowejGry(1, silnik.SzerokośćPlanszy, silnik.WysokośćPlanszy);
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
                        tmr = new DispatcherTimer();
                        tmr.Interval = new TimeSpan(0, 0, 0, 0, 500);
                        tmr.Tick += (_sender, _e) =>
                        { 
                            tmr.IsEnabled = false; wykonajNajlepszyRuch(); 
                        };
                    }
                    tmr.Start();
                }
            }
        }

        private void przygotowaniePlanszyDoNowejGry(
            int numerGraczaRozpoczynającego,
            int szerokośćPlanszy = 8,
            int wysokośćPlanszy = 8)
        {
            planszaSiatka.Children.Clear();
            planszaSiatka.ColumnDefinitions.Clear();
            planszaSiatka.RowDefinitions.Clear();

            for (int i = 0; i < szerokośćPlanszy; i++)
                planszaSiatka.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < wysokośćPlanszy; i++)
                planszaSiatka.RowDefinitions.Add(new RowDefinition());

            plansza = new Button[szerokośćPlanszy, wysokośćPlanszy];
            for (int i = 0; i < szerokośćPlanszy; i++)
                for (int j = 0; j < wysokośćPlanszy; j++)
                {
                    Button przycisk = new Button();
                    planszaSiatka.Children.Add(przycisk);
                    Grid.SetColumn(przycisk, i);
                    Grid.SetRow(przycisk, j);
                    przycisk.Tag = new WspółrzędnePola { Poziomo = i, Pionowo = j };
                    przycisk.Click += new RoutedEventHandler(klikniętoPolaPlanszy);
                    plansza[i, j] = przycisk;
                }

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
            NowaGraDialog dialog = new NowaGraDialog();
            if (dialog.ShowDialog() == true)
            {
                graPrzeciwkoKomputerowi = true;
                Title = "Reversi - 1 gracz";
                przygotowaniePlanszyDoNowejGry(2, dialog.BoardWidth, dialog.BoardHeight);
                if (silnik.LiczbaPustychPól > 0)
                {
                    wykonajNajlepszyRuch();
                }
            }
        }

        private void MenuItem_NowaGraDla1GraczaRozpoczynaszTy_Click(object sender, RoutedEventArgs e)
        {
            NowaGraDialog dialog = new NowaGraDialog();
            if (dialog.ShowDialog() == true)
            {
                graPrzeciwkoKomputerowi = true;
                Title = "Reversi - 1 gracz";
                przygotowaniePlanszyDoNowejGry(1, dialog.BoardWidth, dialog.BoardHeight);
            }
        }

        private void MenuItem_NowaGraDla2Graczy_Click(object sender, RoutedEventArgs e)
        {
            NowaGraDialog dialog = new NowaGraDialog();
            if (dialog.ShowDialog() == true)
            {
                Title = "Reversi - 2 graczy";
                graPrzeciwkoKomputerowi = false;
                przygotowaniePlanszyDoNowejGry(1, dialog.BoardWidth, dialog.BoardHeight);
            }
        }

        private void MenuItem_RuchWykonanyPrzezKomputer_Click(object sender, RoutedEventArgs e)
        {
            wykonajNajlepszyRuch();
        }

        private void MenuItem_PodpowiedźRuchu_Click(object sender, RoutedEventArgs e)
        {
            zaznaczNajlepszyRuch();
        }

        private void MenuItem_ZasadyGry_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "W grze Reversi gracze zajmują na przemian pola planszy, przejmując przy tym wszystkie pola przeciwnika " +
                    "znajdujące się między nowo zajętym polem a innymi polami gracza wykonującego ruch." +
                " Celem gry jest zdobycie większej liczby pól niż przeciwnik.\n" +
                "\n" +
                "Gracz może zająć jedynie takie pole, które pozwoli mu przejąć przynajmniej jedno pole przeciwnika." +
                " Jeżeli takiego pola nie ma, musi oddać ruch.\n" + 
                "\n" +
                "Gra kończy się w momencie zajęcia wszystkich pól lub gdy żaden z graczy nie może wykonać ruchu.\n",
                        "Reversi - Zasady gry");
        }

        private void MenuItem_StrategiaKomputera_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Komputer kieruje się następującymi priorytetami(od najwyższego):\n" +
                    "1.Ustawić pionek w rogu.\n" +
                    "2.Unikać ustawienia pionka tuż przy rogu.\n" +
                    "3.Ustawić pionek przy krawędzi planszy.\n" +
                    "4.Unikać ustawienia pionka w wierszu lub kolumnie oddalonej o jedno pole krawędzi planszy.\n" +
                    "5.Wybierz pole, w wyniku którego zdobyta zostanie największa liczba pól przeciwnika.\n",
                        "Reversi - Strategia komputera"
                    );
        }

        private void MenuItem_OProgramie_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Autorem programu jest Uniwersytet WSB Merito w Poznaniu," +
                " pracę wykonał student o identyfikatorze 140518 - Mikołaj Kocik", 
                Title, MessageBoxButton.OK);
        }

        private void MenuItem_Zamknij_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        private void przyciskKolorGracza_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) wykonajNajlepszyRuch();
            else zaznaczNajlepszyRuch();
        }
    }
}
