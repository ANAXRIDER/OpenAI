using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_NEW1_030 : SimTemplate //deathwing
    {
        //Battlecry: Destroy all other minions and discard your hand.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.allMinionsGetDestroyed();
            p.discardACard(own.own, true);
		}
	}
}