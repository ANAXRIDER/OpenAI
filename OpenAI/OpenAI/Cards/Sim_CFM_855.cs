using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_855 : SimTemplate //* Defias Cleaner
	{
		// Battlecry: Silence a minion with Deathrattle.
        
        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null) p.minionGetSilenced(target);
        }
    }
}