using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_038 : SimTemplate //naga sea witch
	{

        //    Your cards cost (5).

        

        public override void onAuraStarts(Playfield p, Minion own)
        {
            
            if (own.own)
            {
                p.anzOwnNagaSeaWitch++;
            }
            else
            {
                p.anzEnemyNagaSeaWitch++;
            }
        }

        public override void onAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnNagaSeaWitch--;
            }
            else
            {
                p.anzEnemyNagaSeaWitch--;
            }
        }
	}
}