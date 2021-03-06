﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_077 : SimTemplate //Argent Lance
    {

        //Battlecry: Reveal a minion in each deck. If yours costs more, +1 Durability.

        CardDB.Card w = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.AT_077);

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.equipWeapon(w, ownplay);
            
            p.lowerWeaponDurability(-1, ownplay);//-1 = raise dura :D
        }
    }
}