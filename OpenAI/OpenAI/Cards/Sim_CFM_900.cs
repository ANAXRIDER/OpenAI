using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_900 : SimTemplate //* Unlicensed Apothecary
	{
		// Whenever you summon a minion, deal 5 damage to your Hero.

        public override void OnMinionIsSummoned(Playfield p, Minion triggerEffectMinion, Minion summonedMinion)
        {
            if (triggerEffectMinion.own == summonedMinion.own)
            {
                p.minionGetDamageOrHeal(triggerEffectMinion.own ? p.ownHero : p.enemyHero, 5);
            }
        }
    }
}