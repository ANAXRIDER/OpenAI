using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_643 : SimTemplate //* Hobart Grapplehammer
	{
		// Battlecry: Give all weapons in your hand and deck +1 Attack.

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            if (m.own)
            {
                foreach (Handmanager.Handcard hc in p.owncards)
                {
                    if (hc.card.type == CardDB.cardtype.WEAPON) hc.addattack++;
                }
            }
        }
    }
}