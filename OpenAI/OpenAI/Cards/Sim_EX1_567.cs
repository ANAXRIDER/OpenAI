using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_EX1_567 : SimTemplate //doomhammer
	{

//    windzorn/, überladung:/ (2)
        CardDB.Card card = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_567);

		public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
		{
            
            p.equipWeapon(card, ownplay);
            p.changeRecall(ownplay, 2);
		}

	}
}