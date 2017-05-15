using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_334 : SimTemplate //* Hooded Acolyte
	{
		//Whenever a character is healed, give your C'Thun +1/+1 (wherever it is)
		
        public override void OnAHeroGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            p.anzOgOwnCThunHpBonus++;
            p.anzOgOwnCThunAngrBonus++;
        }

        public override void OnAMinionGotHealedTrigger(Playfield p, Minion triggerEffectMinion)
        {
            p.anzOgOwnCThunHpBonus++;
            p.anzOgOwnCThunAngrBonus++;
        }
    }
}