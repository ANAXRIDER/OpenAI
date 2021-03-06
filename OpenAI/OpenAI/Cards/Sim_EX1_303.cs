using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_303 : SimTemplate //shadowflame
	{

//    vernichtet einen befreundeten diener und fügt allen feindlichen dienern schaden zu, der seinem angriff entspricht.
		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            int damage1 = (ownplay) ? p.getSpellDamageDamage(target.Angr) : p.getEnemySpellDamageDamage(target.Angr);

            p.minionGetDestroyed(target);

            p.allMinionOfASideGetDamage(!ownplay, damage1);

		}

	}
}