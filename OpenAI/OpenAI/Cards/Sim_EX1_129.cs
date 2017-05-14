using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_129 : SimTemplate //fanofknives
	{

//    fügt allen feindlichen dienern $1 schaden zu. zieht eine karte.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            p.allMinionOfASideGetDamage(!ownplay, dmg);
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
		}

	}
}