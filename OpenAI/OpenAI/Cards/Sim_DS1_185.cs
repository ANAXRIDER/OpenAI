using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_DS1_185 : SimTemplate //arcaneshot
	{

//    verursacht $2 schaden.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            p.minionGetDamageOrHeal(target, dmg);
		}

	}
}