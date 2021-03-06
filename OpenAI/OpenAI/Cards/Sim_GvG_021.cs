using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_021 : SimTemplate //Mal'Ganis
    {

        //    Your other Demons have +2/+2.Your hero is Immune;

        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnMalGanis++;
                p.ownHero.immune = true;
                foreach (Minion m in p.ownMinions)
                {
                    if (own.entityID != m.entityID && (TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) p.minionGetBuffed(m, 2, 2);
                }
            }
            else
            {
                p.anzEnemyMalGanis++;
                p.enemyHero.immune = true;
                foreach (Minion m in p.enemyMinions)
                {
                    if (own.entityID != m.entityID && (TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) p.minionGetBuffed(m, 2, 2);
                }
            }

        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own)
            {
                p.anzOwnMalGanis--;
                p.ownHero.immune = false;
                foreach (Minion m in p.ownMinions)
                {
                    if (own.entityID != m.entityID && (TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) p.minionGetBuffed(m, -2, -2);
                }
            }
            else
            {
                p.anzEnemyMalGanis--;
                p.enemyHero.immune = false;
                foreach (Minion m in p.enemyMinions)
                {
                    if (own.entityID != m.entityID && (TAG_RACE)m.handcard.card.race == TAG_RACE.DEMON) p.minionGetBuffed(m, -2, -2);
                }
            }
        }


    }

}