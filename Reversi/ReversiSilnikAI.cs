using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi
{
    internal class ReversiSilnikAI : ReversiSilnik
    {
        public ReversiSilnikAI(int numerGraczaRozpoczynającego, 
            int szerokośćPlanszy = 8 , int wysokośćPlanszy = 8) 
            : base(numerGraczaRozpoczynającego, szerokośćPlanszy, wysokośćPlanszy)
        { }

        private struct MożliwyRuch : IComparable<MożliwyRuch>
        {
            public int poziomo;
            public int pionowo;
            public int priorytet;
            public MożliwyRuch(int poziomo, int pionowo, int priorytet)
            {
                this.poziomo = poziomo;
                this.pionowo = pionowo;
                this.priorytet = priorytet;
            }

            public int CompareTo(MożliwyRuch innyRuch)
            {
                return innyRuch.priorytet - this.priorytet;
            }
        }

        public void ProponujNajlepszyRuch(out int najlepszyRuchPoziomo, out int najlepszyRuchPionowo)
        {
            List<MożliwyRuch> możliweRuchy = new List<MożliwyRuch>();
            int skokPriorytetu = SzerokośćPlanszy + WysokośćPlanszy;
            for(int i = 0; i < SzerokośćPlanszy; i++)
            {
                for(int j = 0; j < WysokośćPlanszy; j++)
                {
                    if (PobierzStanPola(i, j) == 0)
                    {
                        int priorytet = PołóżKamień(i, j, true);
                        if(priorytet > 0)
                        {
                            MożliwyRuch nr = new MożliwyRuch(i, j, priorytet);
                            // pole w rogu
                            if((i == 0 || i == SzerokośćPlanszy - 1) && (j == 0 || j== WysokośćPlanszy -1))
                            {
                                nr.priorytet += 2 * skokPriorytetu;
                            }
                            // pole sąsiadujące z rogiem w pionie
                            if((i==0 || i==SzerokośćPlanszy -1) && (j == 1 || j==WysokośćPlanszy-2))
                            {
                                nr.priorytet -= 2 * skokPriorytetu;
                            }
                            // pole sąsiadujące z rogiem w poziomie
                            if((i==1 || i == SzerokośćPlanszy - 2) && (j == 0 || j == WysokośćPlanszy -1))
                            {
                                nr.priorytet -= 2 * skokPriorytetu;
                            }
                            //  pole sąsiadujące z rogiem po skosie
                            if((i == 1 || i == SzerokośćPlanszy - 2) && (j == 1 || j == WysokośćPlanszy -2))
                            {
                                nr.priorytet -= 2 * skokPriorytetu;
                            }
                            // pole na brzegu
                            if((i == 0 || i == SzerokośćPlanszy -1) || (j == 0 || j== WysokośćPlanszy -1))
                            {
                                nr.priorytet += skokPriorytetu;
                            }
                            // pole  
                            if((i == 1 || i == SzerokośćPlanszy - 2) || (j==1 || j == WysokośćPlanszy - 2))
                            {
                                nr.priorytet -= skokPriorytetu;
                            }
                            możliweRuchy.Add(nr);
                        }
                    }
                }     
            }
            if (możliweRuchy.Count > 0)
            {
                możliweRuchy.Sort();
                najlepszyRuchPoziomo = możliweRuchy[0].poziomo;
                najlepszyRuchPionowo = możliweRuchy[0].pionowo;
            }
            else
            {
                throw new Exception("Brak możliwych ruchów");
            }
        }
    }
}
