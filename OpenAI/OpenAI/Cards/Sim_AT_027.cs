using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_027 : SimTemplate //Wilfred Fizzlebang
    {

        //Cards you draw from your Hero Power cost (0).

        public override void OnAuraStarts(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnFizzlebang++;
            }
            else
            {
                p.anzEnemyFizzlebang++;
            }
        }

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnFizzlebang--;
            }
            else
            {
                p.anzEnemyFizzlebang--;
            }
        }

       

    }
}