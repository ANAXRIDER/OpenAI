﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_BRM_001 : SimTemplate //Solemn Vigil
    {


        //    Draw 2 cards. Costs (1) less for each minion that died this turn.


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.None, ownplay);
            p.drawACard(CardDB.cardIDEnum.None, ownplay);

        }

    }
}