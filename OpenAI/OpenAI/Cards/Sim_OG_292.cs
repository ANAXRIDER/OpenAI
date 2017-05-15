using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_OG_292 : SimTemplate //* Forlorn Stalker
    {
        //Battlecry: Give all minions with Deathrattle in your hand +1/+1.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.deathrattle)
                    {
                        hc.addattack++;
                        hc.addHp++;
                    }
                }
            }
        }
    }
}