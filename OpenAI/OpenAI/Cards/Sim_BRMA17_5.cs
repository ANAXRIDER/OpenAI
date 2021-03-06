using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_BRMA17_5 : SimTemplate //* Bone Minions
	{
		// Hero Power: Summon two 2/1 Bone Constructs.

		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BRMA17_6);//2/1Bone Construct
		
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int place = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, place, ownplay, false);
            p.callKid(kid, place, ownplay);
        }
	}
}