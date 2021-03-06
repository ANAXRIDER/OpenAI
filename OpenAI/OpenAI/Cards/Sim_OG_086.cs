using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_086 : SimTemplate //* Forbidden Flame
	{
		//Spend all your Mana. Deal that much damage to a minion.
		
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(p.mana) : p.getEnemySpellDamageDamage(p.enemyMaxMana);
            p.minionGetDamageOrHeal(target, dmg);
			if (ownplay) p.mana = 0;
		}
	}
}