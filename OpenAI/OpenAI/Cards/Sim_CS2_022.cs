﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_CS2_022 : SimTemplate//Polymorph
    {

        private CardDB.Card sheep = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.CS2_tk1);

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.minionTransform(target, sheep);
        }

    }
}
