using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_093 : SimTemplate //consecration
	{

//    fügt allen feinden $2 schaden zu.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            p.allCharsOfASideGetDamage(!ownplay, dmg);
		}

	}
}