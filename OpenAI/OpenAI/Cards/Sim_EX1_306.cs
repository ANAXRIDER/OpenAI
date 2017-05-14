using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_EX1_306 : SimTemplate //* Succubus
    {
        // Battlecry: Discard a random card.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.discardACard(own.own);
		}
	}
}