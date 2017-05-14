using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_221 : SimTemplate //* Selfless Hero
	{
		//Deathrattle: Give a random friendly minion Divine Shield.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
			Minion target = (m.own) ? p.searchRandomMinion(p.ownMinions, Playfield.searchmode.searchLowestAttack) : p.searchRandomMinion(p.enemyMinions, Playfield.searchmode.searchLowestAttack);
			if (target != null) target.divineshild = true;
        }
    }
}