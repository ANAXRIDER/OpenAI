using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_008 : SimTemplate //Coldarra Drake
    {

        //You can use your Hero Power any number of times.

        public override void OnAuraStarts(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnGarrisonCommander+=1000;
            }
            else
            {
                p.anzEnemyGarrisonCommander+=1000;
            }
        }

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnGarrisonCommander -= 1000;
            }
            else
            {
                p.anzEnemyGarrisonCommander -= 1000;
            }
        }

       

    }
}