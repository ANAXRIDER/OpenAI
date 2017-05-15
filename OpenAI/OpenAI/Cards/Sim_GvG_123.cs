using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_123 : SimTemplate //Soot Spewer
    {

        //   Spell Damage +1

        public override void OnAuraStarts(Playfield p, Minion own)
        {
            own.spellpower = 1;
            if (own.own)
            {
                p.spellpower++;
            }
            else
            {
                p.enemyspellpower++;
            }
        }


    }

}