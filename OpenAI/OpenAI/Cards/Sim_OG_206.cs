using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_206 : SimTemplate //* Stormcrack
	{
		//Deal 4 damage to a minion. Overload: (1)
		
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
            p.minionGetDamageOrHeal(target, dmg);

            p.changeRecall(ownplay, 1);
		}
	}
}