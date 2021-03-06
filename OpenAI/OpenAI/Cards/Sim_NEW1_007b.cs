﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_NEW1_007b : SimTemplate //starfall choice left
    {

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(5) : p.getEnemySpellDamageDamage(5);
            p.minionGetDamageOrHeal(target, dmg);
        }

    }
}