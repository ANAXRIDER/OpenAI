using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_034 : SimTemplate //Radiant Elemental
    {

        //Your spells cost (1) less.


        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnSorcerersApprentice++;
            }
            else
            {
                p.anzEnemysorcerersapprentice++;

            }

        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnSorcerersApprentice--;
            }
            else
            {
                p.anzEnemysorcerersapprentice--;
            }
        }
    }

}