using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_003 : SimTemplate //Fallen Hero
    {

        //Your Hero Power deals 1 extra damage.

        public override void OnAuraStarts(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnFallenHeros++;
            }
            else
            {
                p.anzEnemyFallenHeros++;
            }
        }

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnFallenHeros--;
            }
            else
            {
                p.anzEnemyFallenHeros--;
            }
        }

       

    }
}