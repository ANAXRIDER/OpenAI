using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_004 : SimTemplate //youngpriestess
	{

//    verleiht am ende eures zuges einem anderen zufälligen befreundeten diener +1 leben.
        public override void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            List<Minion> temp2 = new List<Minion>((turnEndOfOwner) ? p.ownMinions : p.enemyMinions);
            temp2.Sort((a, b) => a.Hp.CompareTo(b.Hp));//buff the weakest
            foreach (Minion mins in temp2)
            {
                if (triggerEffectMinion.entityID == mins.entityID) continue;
                p.minionGetBuffed(mins, 0, 1);
                break;
            }
        }

	}
}