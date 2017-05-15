using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_531 : SimTemplate //scavenginghyena
	{

//    erhält jedes mal +2/+1, wenn ein befreundetes wildtier stirbt.

        public override void OnMinionDiedTrigger(Playfield p, Minion triggerEffectMinion, Minion diedMinion)
        {
            if (triggerEffectMinion.own == diedMinion.own && (TAG_RACE)diedMinion.handcard.card.race == TAG_RACE.BEAST)
            {
                p.minionGetBuffed(triggerEffectMinion, 2, 1);
            }
        }

	}
}