using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_005 : SimTemplate //Echo of Medivh
    {

        //    Put a copy of each friendly minion into your hand.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            // optimistic
            List<Minion> temp = (ownplay) ? p.ownMinions : p.enemyMinions;

            foreach (Minion m in temp)
            {
                p.drawACard(m.handcard.card.cardIDenum, ownplay, true);
            }
            
        }


    }

}