using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_238 : SimTemplate //lightningbolt
	{

//    verursacht $3 schaden. überladung:/ (1)

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(3) : p.getEnemySpellDamageDamage(3);
            p.minionGetDamageOrHeal(target, dmg);
            p.changeRecall(ownplay, 1);
            
		}

	}
}