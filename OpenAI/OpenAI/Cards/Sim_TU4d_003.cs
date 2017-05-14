using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_TU4d_003 : SimTemplate //* Shotgun Blast
	{
		// Hero Power: Deal 1 damage.
		
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
			int dmg = (ownplay) ? p.getHeroPowerDamage(1) : p.getEnemyHeroPowerDamage(1);
            if (target == null) target = ownplay ? p.enemyHero : p.ownHero;
            p.minionGetDamageOrHeal(target, dmg);
        }
	}
}