using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_116 : SimTemplate //Wyrmrest Agent
    {

        //Battlecry: If you're holding a Dragon, gain +1 Attack and Taunt

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            bool hasdragon = false;
            if (own.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.race == TAG_RACE.DRAGON) hasdragon = true;
                }
            }
            else
            {
                hasdragon = true;
            }
            if (hasdragon)
            {
                p.minionGetBuffed(own, 1, 0);
                own.taunt = true;
            }

        }

       

    }
}