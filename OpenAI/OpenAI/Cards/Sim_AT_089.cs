using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_089 : SimTemplate //Boneguard Lieutenant
    {

        //Inspire: Gain +1 Health.

        public override void OnInspire(Playfield p, Minion m)
        {
            p.minionGetBuffed(m, 0, 1);
        }


    }
}