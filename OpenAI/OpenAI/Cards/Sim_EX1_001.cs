using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_001 : SimTemplate //lightwarden
	{

        //   Whenever a character is healed, gain +2 Attack.
        public override void OnAHeroGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            p.minionGetBuffed(triggerEffectMinion, 2, 0);
        }

        public override void OnAMinionGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            p.minionGetBuffed(triggerEffectMinion, 2, 0);
        }

	}
}