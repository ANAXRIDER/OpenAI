using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_667 : SimTemplate //* Bomb Squad
	{
		// Battlecry: Deal 5 damage to an enemy minion. Deathrattle: Deal 5 damage to your hero.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (target != null) p.minionGetDamageOrHeal(target, 5);
        }

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            p.minionGetDamageOrHeal(m.own ? p.ownHero : p.enemyHero, 5);
        }
	}
}