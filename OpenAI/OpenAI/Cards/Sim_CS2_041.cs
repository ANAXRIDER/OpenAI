using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_041 : SimTemplate //ancestralhealing
	{

//    stellt das volle leben eines dieners wieder her und verleiht ihm spott/.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            target.taunt = true;
            int heal = (ownplay)? p.getSpellHeal(target.maxHp) : p.getEnemySpellHeal(target.maxHp);
            p.minionGetDamageOrHeal(target, -heal);
		}

	}
}