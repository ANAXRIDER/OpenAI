using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_006 : SimTemplate //Mechwarper
    {

        //    Your Mechs cost (1) less.

        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnMechwarper++;
            }
            else
            {
                p.anzEnemyMechwarper++;

            }

        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnMechwarper--;
            }
            else
            {
                p.anzEnemyMechwarper--;
            }
        }


    }

}