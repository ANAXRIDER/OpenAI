using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_107 : SimTemplate //Enhance-o Mechano
    {

        //  Battlecry: Give your other minions Windfury Taunt or Divine Shield

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            List<Minion> temp = (own.own) ? p.ownMinions : p.enemyMinions;

            foreach (Minion m in temp)
            {
                m.taunt = true;
            }
        }
    }
}