using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_LOE_077 : SimTemplate //Brann bronzebeard
	{

        //    YourBattlecries trigger twice.


        public override void OnAuraStarts(Playfield p, Minion own)
        {
            
            if (own.own)
            {
                p.anzOwnBranns++;
            }
            else
            {
                p.anzEnemyBranns++;
            }
        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnBranns--;
            }
            else
            {
                p.anzEnemyBranns--;
            }
        }
	}
}