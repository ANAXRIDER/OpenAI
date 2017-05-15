using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_142 : SimTemplate //koboldgeomancer
	{

//    zauberschaden +1/
        public override void OnAuraStarts(Playfield p, Minion own)
		{
            own.spellpower += 1;
            if (own.own)
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