using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_755 : SimTemplate //* Grimestreet Pawnbroker
	{
		// Battlecry: Give a random weapon in your hand +1/+1.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                Handmanager.Handcard hc = p.searchRandomMinionInHand(p.owncards, Playfield.searchmode.searchLowestCost, Playfield.cardsProperty.Weapon);
                if (hc != null)
                {
                    hc.addattack++;
                    hc.addHp++;
                    p.anzOwnExtraAngrHp += 2;
                }
            }
        }
	}
}