using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Reversi
{
    internal class ReversiSilnik
    {
        public int SzerokośćPlanszy { get; private set; }
        public int WysokośćPlanszy { get; private set; }
        public int NumerGraczaWykonujacegoNastepnyRuch { get; private set; } = 1;
        private int[,] plansza;
        private int[] liczbyPól = new int[3];
        public int LiczbaPustychPól { get { return liczbyPól[0]; } }
        public int LiczbaPólGracz1 { get { return liczbyPól[1]; } }
        public int LiczbaPólGracz2 { get { return liczbyPól[2]; } }

        public int NumerGraczaMającegoPrzewagę
        {
            get
            {
                if (LiczbaPólGracz1 == LiczbaPólGracz2) return 0;
                else if (LiczbaPólGracz1 > LiczbaPólGracz2) return 1;
                else return 2;
            }
        }

        public enum SytuacjaNaPlanszy
        {
            RuchJestMożliwy,
            BieżącyGraczNieMożeWykonaćRuchu,
            ObajGraczeNieMogąWykonaćRuchu,
            WszystkiePolaPlanszySąZajęte
        }

        public ReversiSilnik(int numerGraczaRozpoczynającego, int szerokośćPlanszy = 8,
            int wysokośćPlanszy = 8)
        {
            SzerokośćPlanszy = szerokośćPlanszy;
            WysokośćPlanszy = wysokośćPlanszy;
            plansza = new int[szerokośćPlanszy, wysokośćPlanszy];
            NumerGraczaWykonujacegoNastepnyRuch = numerGraczaRozpoczynającego;
            czyśćPlanszę();
        }

        private void obliczLiczbyPól()
        {
            liczbyPól[0] = 0;
            liczbyPól[1] = 0;
            liczbyPól[2] = 0;

            for (int i = 0; i < SzerokośćPlanszy; i++)
            {
                for (int j = 0; j < WysokośćPlanszy; j++)   
                {
                    liczbyPól[plansza[i, j]]++;
                }
            }
        }

        private void zmieńBieżącegoGracza()
        {
            NumerGraczaWykonujacegoNastepnyRuch =
                numerPrzeciwnika(NumerGraczaWykonujacegoNastepnyRuch);
        }

        private void czyśćPlanszę()
        {
            for (int i = 0; i < SzerokośćPlanszy; i++)
            {
                for (int j = 0; j < WysokośćPlanszy; j++)
                {
                    plansza[i, j] = 0;
                }
            }

            int środekSzer = SzerokośćPlanszy / 2;
            int środekWys = WysokośćPlanszy / 2;
            plansza[środekSzer - 1, środekWys -1] = 1;
            plansza[środekSzer, środekWys] =1;
            plansza[środekSzer, środekWys -1] = 2;
            plansza[środekSzer - 1, środekWys] = 2;
        }

        public SytuacjaNaPlanszy ZbadajSytuacjęNaPlanszy()
        {
            if (LiczbaPustychPól == 0) return SytuacjaNaPlanszy.WszystkiePolaPlanszySąZajęte;

            bool czyMożliwyRuch = czyBieżącyGraczMożeWykonaćRuch();
            if (czyMożliwyRuch) return SytuacjaNaPlanszy.RuchJestMożliwy;
            else
            {
                zmieńBieżącegoGracza();
                bool czyMożliwyRuchPrzeciwnika = czyBieżącyGraczMożeWykonaćRuch();
                zmieńBieżącegoGracza();
                if (czyMożliwyRuchPrzeciwnika)
                    return SytuacjaNaPlanszy.BieżącyGraczNieMożeWykonaćRuchu;
                else
                {
                    return SytuacjaNaPlanszy.ObajGraczeNieMogąWykonaćRuchu;
                }
            }
        }

        public int PobierzStanPola(int poziomo, int pionowo)
        {
            if (!czyWspółrzędnePolaPrawidłowe(poziomo, pionowo))
                throw new Exception("Nieprawidłowe współrzędne pola");

            return plansza[poziomo, pionowo];
        }

        private bool czyWspółrzędnePolaPrawidłowe(int poziomo, int pionowo)
        {
            return poziomo >= 0 && pionowo >= 0 &&
                poziomo < SzerokośćPlanszy && pionowo < WysokośćPlanszy;
        }

        private static int numerPrzeciwnika(int numerGracza)
        {
            if (numerGracza == 1) return 2;
            else return 1;
        }

        private bool czyBieżącyGraczMożeWykonaćRuch()
        {
            int liczbaPoprawnychPól = 0;
            for(int i = 0; i < SzerokośćPlanszy; i++)
            {
                for(int j = 0; j < WysokośćPlanszy; j++)
                {
                    if (plansza[i, j] == 0 && PołóżKamień(i, j, true) > 0)
                    {
                        liczbaPoprawnychPól++;
                    }
                }
            }
            return liczbaPoprawnychPól > 0;
        }
        public void Pasuj()
        {
            if (czyBieżącyGraczMożeWykonaćRuch())
                throw new Exception("Gracz nie może oddać ruchu - wykonanie ruchu jest możliwe");
            zmieńBieżącegoGracza();
        }

        public int PołóżKamień(int poziomo, int pionowo)
        {
            return PołóżKamień(poziomo, pionowo, false);
        }

        protected int PołóżKamień(int poziomo, int pionowo, bool tylkoTest)
        {
            if (plansza[poziomo, pionowo] != 0) return -1;
            int ilePólPrzejętych = 0;
            for(int kierunekPoziomo = -1; kierunekPoziomo <= 1; kierunekPoziomo++)
                for(int kierunekPionomo = -1; kierunekPionomo <= 1; kierunekPionomo++)
                {
                    if (kierunekPionomo == 0 && kierunekPoziomo == 0) continue; 
                    int i = poziomo;
                    int j = pionowo;
                    bool znalezionoPustePole = false;
                    bool osiągniętaKrawędźPlanszy = false;
                    bool znalezionyKamieńGraczaWykonującegoRuch = false;
                    bool znalezionyKamieńPrzeciwnika = false;
                    do
                    {
                        i+= kierunekPoziomo;
                        j+= kierunekPionomo;
                        if (!czyWspółrzędnePolaPrawidłowe(i, j)) osiągniętaKrawędźPlanszy = true;
                        if (!osiągniętaKrawędźPlanszy)
                        {
                            if (plansza[i, j] == 0) znalezionoPustePole = true;
                            if (plansza[i, j] == NumerGraczaWykonujacegoNastepnyRuch)
                                znalezionyKamieńGraczaWykonującegoRuch = true;
                            if (plansza[i, j] == numerPrzeciwnika(NumerGraczaWykonujacegoNastepnyRuch))
                                znalezionyKamieńPrzeciwnika = true;
                        }
                    }
                    while (!(znalezionoPustePole || osiągniętaKrawędźPlanszy 
                        || znalezionyKamieńGraczaWykonującegoRuch));

                    bool położenieKamieniaMożliwe = znalezionyKamieńPrzeciwnika &&
                         znalezionyKamieńGraczaWykonującegoRuch && !znalezionoPustePole;

                    if (położenieKamieniaMożliwe)
                    {
                        int maxIndex = Math.Max(Math.Abs(i - poziomo), Math.Abs(j - pionowo));
                        if (!tylkoTest)
                        {
                            for (int indeks = 0; indeks < maxIndex; indeks++)
                            {
                                plansza[poziomo + indeks * kierunekPoziomo, pionowo + indeks * kierunekPionomo] = NumerGraczaWykonujacegoNastepnyRuch;
                            }
                        }
                        ilePólPrzejętych += maxIndex - 1;
                    }
                }
            if (ilePólPrzejętych > 0 && !tylkoTest) zmieńBieżącegoGracza();
            obliczLiczbyPól();
            return ilePólPrzejętych;
        }
    }
}
