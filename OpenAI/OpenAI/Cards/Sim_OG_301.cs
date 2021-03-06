using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_301 : SimTemplate //* Ancient Shieldbearer
	{
		//Battlecry: If your C'Thun has at least 10 Attack, gain 10 Armor
		
		public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
		{
            if (own.own && (p.anzOgOwnCThunAngrBonus + 6) > 9) p.minionGetArmor(p.ownHero, 10);
		}
	}
}