using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_BRM_028 : SimTemplate //Emperor Thaurissan
    {

        //    At the end of your turn, reduce the Cost of cards in your hand by 1.

        public override void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            if (turnEndOfOwner == true && triggerEffectMinion.own == turnEndOfOwner)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.manacost >= 1) hc.manacost -= 1;
                }
            }
        }
    }
}