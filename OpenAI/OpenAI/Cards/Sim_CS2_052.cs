using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_052 : SimTemplate //wrathofairtotem
	{

//    zauberschaden +1/
		public override void  OnAuraStarts(Playfield p, Minion m)
        {
            m.spellpower = 1;
            if (m.own)
            {
                p.spellpower++;
            }
            else
            {
                p.enemyspellpower++;
            }
        }

        

	}
}