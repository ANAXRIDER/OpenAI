using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_113 : SimTemplate //Recruiter
    {

        //insprire: Add a 2/2 Squire to your hand

        public override void OnInspire(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.CS2_152, m.own, true);
        }



    }

}