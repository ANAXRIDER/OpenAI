using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_659 : SimTemplate //* Gadgetzan Socialite
	{
		// Battlecry: Restore 2 Health.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null)
            {
                int heal = (m.own) ? p.getMinionHeal(2) : p.getEnemyMinionHeal(2);
                p.minionGetDamageOrHeal(target, -heal);
            }
        }
    }
}