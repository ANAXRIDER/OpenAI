using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_PART_005 : SimTemplate //Emergency Coolant
    {

        //  Freeze a minion. 


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            target.frozen = true;
        }


    }

}