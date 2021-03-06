using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_DS1_178 : SimTemplate //tundrarhino
	{

//    eure wildtiere haben ansturm/.
        //todo charge?
        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnTundrarhino++;
                foreach (Minion m in p.ownMinions)
                {
                    if ((TAG_RACE)m.handcard.card.race == TAG_RACE.BEAST) p.minionGetCharge(m);
                }
            }
            else
            {
                p.anzEnemyTundrarhino++;
                foreach (Minion m in p.enemyMinions)
                {
                    if ((TAG_RACE)m.handcard.card.race == TAG_RACE.BEAST) p.minionGetCharge(m);
                }
            }

        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnTundrarhino--;
                foreach (Minion m in p.ownMinions)
                {
                    if ((TAG_RACE)m.handcard.card.race == TAG_RACE.BEAST) p.minionLostCharge(m);
                }
            }
            else
            {
                p.anzEnemyTundrarhino--;
                foreach (Minion m in p.enemyMinions)
                {
                    if ((TAG_RACE)m.handcard.card.race == TAG_RACE.BEAST) p.minionLostCharge(m);
                }
            }
        }

	}
}