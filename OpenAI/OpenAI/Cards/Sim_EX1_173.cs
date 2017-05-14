using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_173 : SimTemplate //starfire
	{

//    verursacht $5 schaden. zieht eine karte.

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int dmg = (ownplay) ? p.getSpellDamageDamage(5) : p.getEnemySpellDamageDamage(5);
            p.minionGetDamageOrHeal(target, dmg);
            //this.owncarddraw++;
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
		}

	}
}