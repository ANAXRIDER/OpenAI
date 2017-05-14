using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_CS2_026 : SimTemplate //frostnova
    {

        //    friert/ alle feindlichen diener ein.
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            List<Minion> temp = (ownplay) ? p.enemyMinions : p.ownMinions;
            foreach (Minion t in temp)
            {
                t.frozen = true;
            }
        }
    }
}