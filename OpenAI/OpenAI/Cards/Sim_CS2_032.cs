using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_032 : SimTemplate //flamestrike
	{

//    fügt allen feindlichen dienern $4 schaden zu.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
            p.allMinionOfASideGetDamage(!ownplay, dmg);
		}

	}
}