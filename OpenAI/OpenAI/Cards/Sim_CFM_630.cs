using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_630 : SimTemplate //* Counterfeit Coin
	{
		// Gain 1 Mana Crystal this turn only.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.mana = Math.Min(p.mana + 1, 10);
        }
    }
}