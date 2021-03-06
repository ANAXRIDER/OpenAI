﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_018 : SimTemplate //Confessor Paletress
    {

        //Inspire: Summon a random Legendary minion

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.NEW1_024); //captain greenskin

        public override void OnInspire(Playfield p, Minion m)
        {

            int pos = (m.own) ? p.ownMinions.Count : p.enemyMinions.Count;
            
            p.callKid(kid, pos, m.own);
        }
    }
}