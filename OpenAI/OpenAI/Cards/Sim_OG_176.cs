using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_176 : SimTemplate //* Shadow Strike
	{
		//Deal 5 damage to an undamaged character.
		
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(5) : p.getEnemySpellDamageDamage(5);
            p.minionGetDamageOrHeal(target, dmg);
		}
	}
}