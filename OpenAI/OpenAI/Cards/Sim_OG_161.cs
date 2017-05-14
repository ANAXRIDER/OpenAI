using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_161 : SimTemplate //* Corrupted Seer
    {
        //Battlecry: Deal 2 damage to all non-Murloc minions.

		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            foreach (Minion m in p.ownMinions)
            {
                if ((TAG_RACE)m.handcard.card.race != TAG_RACE.MURLOC) p.minionGetDamageOrHeal(m, 2);
            }
            foreach (Minion m in p.enemyMinions)
            {
                if ((TAG_RACE)m.handcard.card.race != TAG_RACE.MURLOC) p.minionGetDamageOrHeal(m, 2);
            }
		}
	}
}