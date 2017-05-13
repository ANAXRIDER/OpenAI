using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_011 : SimTemplate //Holy Champion
	{

        //   Whenever a character is healed, gain +2 Attack.
        public override void onAHeroGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            p.minionGetBuffed(triggerEffectMinion, 2, 0);
        }

        public override void onAMinionGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            p.minionGetBuffed(triggerEffectMinion, 2, 0);
        }

	}
}