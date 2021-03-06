using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_334 : SimTemplate //* Smuggler's Crate
	{
		// Give a random Beast in your hand +2/+2.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (ownplay)
            {
                Handmanager.Handcard hc = p.searchRandomMinionInHand(p.owncards, Playfield.searchmode.searchLowestCost, Playfield.cardsProperty.Race, TAG_RACE.BEAST);
                if (hc != null)
                {
                    hc.addattack += 2;
                    hc.addHp += 2;
                    p.anzOwnExtraAngrHp += 4;
                }
            }
        }
    }
}
