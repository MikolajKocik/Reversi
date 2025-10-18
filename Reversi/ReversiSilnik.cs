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
        public ReversiSilnik(int numerGraczaRozpoczynającego, int szerokośćPlanszy = 8,
            int wysokośćPlanszy = 8)
        {
            SzerokośćPlanszy = szerokośćPlanszy;
            WysokośćPlanszy = wysokośćPlanszy;
            plansza = new int[szerokośćPlanszy, wysokośćPlanszy];

            czyśćPlanszę();
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
            plansza[środekSzer - 1, środekWys] = 1;
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

        private static int numberPrzeciwnika(int numerGracza)
        {
            if (numerGracza == 1) return 2;
            else return 1;
        }

        public void PołóżKamień(int poziomo, int pionowo)
        {
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
                            if (plansza[i, j] == numberPrzeciwnika(NumerGraczaWykonujacegoNastepnyRuch))
                                znalezionyKamieńPrzeciwnika = true;
                        }
                    }
                    while (!(znalezionoPustePole || osiągniętaKrawędźPlanszy 
                        || znalezionyKamieńGraczaWykonującegoRuch));

                    bool położenieKamieniaMożliwe = znalezionyKamieńPrzeciwnika &&
                         znalezionyKamieńGraczaWykonującegoRuch && znalezionoPustePole;

                    if (położenieKamieniaMożliwe)
                    {
                        int maxIndex = Math.Max(Math.Abs(i - poziomo), Math.Abs(j - pionowo));

                        for (int indeks = 0; indeks < maxIndex - poziomo; indeks++)
                        { 
                            plansza[poziomo + indeks*kierunekPoziomo, pionowo + indeks+kierunekPionomo] = NumerGraczaWykonujacegoNastepnyRuch;
                        }
                    }
                }
        }
    }
}
