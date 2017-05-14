using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_OG_220 : SimTemplate //* Malkorok
	{
		//Battlecry: Equip a random weapon.
		
        CardDB.Card w = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_080);

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.equipWeapon(w, own.own);
        }
    }
}