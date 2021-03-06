using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_938 : SimTemplate //Hot Spring Guardian
    {

        //Taunt Battlecry: Restore 3_Health.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            int heal = (own.own) ? p.getMinionHeal(3) : p.getEnemyMinionHeal(3);
            p.minionGetDamageOrHeal(target, -heal);
        }

    }

}