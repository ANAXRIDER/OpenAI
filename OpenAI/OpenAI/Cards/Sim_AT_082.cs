using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_082 : SimTemplate //Lowly Squire
    {

        //Inspire: Gain +1 Attack.

        public override void OnInspire(Playfield p, Minion m)
        {
            p.minionGetBuffed(m, 1, 0);
        }


    }
}