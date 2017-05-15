using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_310 : SimTemplate //* Doomguard
    {
        // Charge. Battlecry: Discard two random cards.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            p.discardACard(own.own);
            p.discardACard(own.own);
		}
	}
}