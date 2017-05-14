using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_712 : SimTemplate //Violet Illusionist
    {
        // During your turn, your hero is Immune.

        public override void OnAuraStarts(Playfield p, Minion own)
        {
            if (own.own) p.ownHero.immune = true;
            else p.enemyHero.immune = true;
        }

        public override void OnAuraEnds(Playfield p, Minion own)
        {
            if (own.own) p.ownHero.immune = false;
            else p.enemyHero.immune = false;
        }

        public override void OnTurnStartTrigger(Playfield p, Minion triggerEffectMinion, bool turnStartOfOwner)
        {
            if (triggerEffectMinion.own == turnStartOfOwner)
            {
                if (turnStartOfOwner) p.ownHero.immune = true;
                else p.enemyHero.immune = true;
            }
        }

        public override void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            if (turnEndOfOwner == true && triggerEffectMinion.own == turnEndOfOwner)
            {
                if (turnEndOfOwner) p.ownHero.immune = false;
                else p.enemyHero.immune = false;
            }
        }
    }
}