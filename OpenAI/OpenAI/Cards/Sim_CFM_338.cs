using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_338 : SimTemplate //* Trogg Beastrager
	{
        // Battlecry: Give a random Beast in your hand +1/+1.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                Handmanager.Handcard hc = p.searchRandomMinionInHand(p.owncards, Playfield.searchmode.searchLowestCost, Playfield.cardsProperty.Race, TAG_RACE.BEAST);
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