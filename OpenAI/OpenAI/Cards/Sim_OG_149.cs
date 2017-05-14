using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_149 : SimTemplate //* Ravaging Ghoul
	{
		//Battlecry: Deal 1 damage to all other minions.
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            foreach (Minion m in p.enemyMinions)
            {
                if (own.entityID != m.entityID) p.minionGetDamageOrHeal(m, 1);
            }
            foreach (Minion m in p.ownMinions)
            {
                if (own.entityID != m.entityID) p.minionGetDamageOrHeal(m, 1);
            }           
        }
	}
}