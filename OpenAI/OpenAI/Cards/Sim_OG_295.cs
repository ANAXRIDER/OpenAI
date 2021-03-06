using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_295 : SimTemplate //* Cult Apothecary
	{
		//Battlecry: For each enemy minion, restore 2 Health to your hero.
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
			if (own.own) p.minionGetDamageOrHeal(p.ownHero, -p.getMinionHeal(p.enemyMinions.Count));
			else p.minionGetDamageOrHeal(p.enemyHero, -p.getEnemyMinionHeal(p.ownMinions.Count));
        }
    }
}