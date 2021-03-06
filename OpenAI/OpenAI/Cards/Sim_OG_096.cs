using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_096 : SimTemplate //* Twilight Darkmender
	{
		//Battlecry: If your C'Thun has at least 10 Attack, restore 10 Health to your hero.
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (own.own && (p.anzOgOwnCThunAngrBonus + 6) > 9)
			{
				p.minionGetDamageOrHeal(p.ownHero, -p.getMinionHeal(10));
			}
		}
	}
}