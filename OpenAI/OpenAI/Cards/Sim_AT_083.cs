using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_083 : SimTemplate //Dragonhawk Rider
    {

        //Inspire: Gain Windfury this turn.

        public override void OnInspire(Playfield p, Minion m)
        {
            p.minionGetWindfurry(m);
        }

        public override void OnTurnEndsTrigger(Playfield p, Minion triggerEffectMinion, bool turnEndOfOwner)
        {
            //We do it dirty! we remove allways windfurry of him at end of turn :D //its unlikely that someone buffs this with windfury!
            triggerEffectMinion.windfury = false;
        }


    }
}