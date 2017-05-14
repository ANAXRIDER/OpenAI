using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_846 : SimTemplate //Shimmering Tempest
    {

        //Deathrattle: Add a random Mage spell to your hand.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.CardToHand(CardDB.cardName.unknown, m.own);
        }

    }

}