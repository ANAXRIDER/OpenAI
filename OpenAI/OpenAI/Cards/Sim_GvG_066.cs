using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_066 : SimTemplate //Dunemaul Shaman
    {

        //   Windfury, Overload: (1)&lt;/b&gt

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.changeRecall(own.own, 1);
        }


    }

}