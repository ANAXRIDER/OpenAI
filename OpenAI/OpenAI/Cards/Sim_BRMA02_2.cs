using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_BRMA02_2 : SimTemplate //* Jeering Crowd
	{
		// Hero Power: Summon a 1/1 Spectator with Taunt.
		
		CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.BRMA02_2t);//Dark Iron Spectator
		
        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int place = (ownplay) ? p.ownMinions.Count : p.enemyMinions.Count;
            p.callKid(kid, place, ownplay, false);
        }
	}
}