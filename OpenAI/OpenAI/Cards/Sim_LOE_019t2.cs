using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_LOE_019t2 : SimTemplate //golden monkey
	{
        // Battlecry:do some crazy stuff
        // =Replace your hand and deck with Legendary minions.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            // TODO: to random... just give him a high value :D
            Probabilitymaker.Instance.hasDeck = false;
        }
	}
}