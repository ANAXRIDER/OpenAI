using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_315 : SimTemplate //* Alleycat
	{
		// Battlecry: Summon a 1/1 Cat.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CFM_315t); //1/1 Cat

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.callKid(kid, m.zonepos, m.own, true);
        }
    }
}