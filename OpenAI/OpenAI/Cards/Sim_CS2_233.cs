﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_CS2_233 : SimTemplate//Blade Flurry
    {


        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int damage = (ownplay) ? p.getSpellDamageDamage(p.ownWeaponAttack) : p.getEnemySpellDamageDamage(p.enemyWeaponAttack);
            
            p.allCharsOfASideGetDamage(!ownplay, damage);
            //destroy own weapon
            p.lowerWeaponDurability(1000, true);
        }

    }
}
