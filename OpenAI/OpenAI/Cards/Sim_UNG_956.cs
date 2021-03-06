using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_956 : SimTemplate //Spirit Echo
    {

        //Give your minions 'Deathrattle: Return _this to your hand'

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;

            foreach (Minion m in temp)
            {
                m.spiritecho++;
            }
        }

    }

}