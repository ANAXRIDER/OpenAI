using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_914 : SimTemplate //Raptor Hatchling
    {

        //Deathrattle: Shuffle a 4/3 Raptor into your deck.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            if (m.own)
            {
                p.ownDeckSize++;
            }
            else
            {
                p.enemyDeckSize++;
            }
        }

    }

}