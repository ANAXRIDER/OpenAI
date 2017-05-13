using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_122 : SimTemplate //raidleader
	{

//    eure anderen diener haben +1 angriff.
        public override void onAuraStarts(Playfield p, Minion own)
		{
            if (own.own)
            {
                p.anzOwnRaidleader++;
                foreach (Minion m in p.ownMinions)
                {
                    if (own.entityID != m.entityID) p.minionGetBuffed(m, 1, 0);
                }
            }
            else
            {
                p.anzEnemyRaidleader++;
                foreach (Minion m in p.enemyMinions)
                {
                    if (own.entityID != m.entityID) p.minionGetBuffed(m, 1, 0);
                }
            }
            
		}

        public override void onAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnRaidleader--;
                foreach (Minion m in p.ownMinions)
                {
                    if (own.entityID != m.entityID) p.minionGetBuffed(m, -1, 0);
                }
            }
            else
            {
                p.anzEnemyRaidleader--;
                foreach (Minion m in p.enemyMinions)
                {
                    if (own.entityID != m.entityID) p.minionGetBuffed(m, -1, 0);
                }
            }
        }

	}
}