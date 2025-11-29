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
    }
}
