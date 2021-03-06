﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_122 : SimTemplate //Gormok the Impaler
    {

        //Battlecry: If you have at least 4 other minions, deal 4 damage

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {

            if (target != null)
            {
                int count = 0;
                count = own.own ? p.ownMinions.Count : p.enemyMinions.Count;
                if (count >= 4)
                {
                    p.minionGetDamageOrHeal(target, 4);
                }
            }

        }

       

    }
}