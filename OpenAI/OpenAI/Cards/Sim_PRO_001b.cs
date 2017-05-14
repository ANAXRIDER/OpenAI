﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_PRO_001b : SimTemplate//Rogues Do It...
    {


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(4) : p.getEnemySpellDamageDamage(4);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}
