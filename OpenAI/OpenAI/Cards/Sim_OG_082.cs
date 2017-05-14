using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_082 : SimTemplate //* Evolved Kobold
	{
		//Spell Damage +2
		
        public override void OnAuraStarts(Playfield p, Minion own)
		{
            if (own.own) p.spellpower += 2;
            else p.enemyspellpower += 2;
		}

        public override void OnAuraEnds(Playfield p, Minion m)
        {
            if (m.own) p.spellpower -= 2;
            else p.enemyspellpower -= 2;
        }
	}
}