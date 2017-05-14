using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_127 : SimTemplate //Nexus-Champion Saraad
    {

        //Inspire: Add a random spell to your hand

        public override void OnInspire(Playfield p, Minion m)
        {
            p.drawACard(CardDB.cardIDEnum.None, m.own, true); 
        }
    }
}