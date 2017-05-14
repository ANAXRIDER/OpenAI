using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_091 : SimTemplate //lightsjustice
	{

//
        CardDB.Card w = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_091);

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(w, ownplay);
        }
	}
}