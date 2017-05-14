using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_085 : SimTemplate //Emerald Hive Queen
    {

        //Your minions cost (2) more.

        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own) p.anzEmeraldHiveQueen++;
        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own) p.anzEmeraldHiveQueen--;
        }

    }

}