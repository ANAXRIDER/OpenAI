using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CFM_648 : SimTemplate //* Big-Time Racketeer
	{
		// Battlecry: Summon a 6/6 Ogre.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CFM_648t); //6/6 Ogre

        public override void GetBattlecryEffect(Playfield p, Minion m, Minion target, int choice)
        {
            p.callKid(kid, m.zonepos, m.own, true);
        }
    }
}