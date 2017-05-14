using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_029 : SimTemplate //Fallen Hero
    {

        //Your Hero Power deals 1 extra damage.

        public override void OnAuraStarts(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnBuccaneer++;
            }
            else
            {
                p.anzEnemyBuccaneer++;
            }
        }

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.anzOwnBuccaneer--;
            }
            else
            {
                p.anzEnemyBuccaneer--;
            }
        }

       

    }
}