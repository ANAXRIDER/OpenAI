using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_074 : SimTemplate //Seal of Champions
    {

        //   Give a minion +3 Attack and Divine Shield

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {

            if (target != null)
            {
                p.minionGetBuffed(target, 3, 0);
                target.divineshild = true;
            }
        }


    }

}