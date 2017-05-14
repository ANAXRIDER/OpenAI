using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_NEW1_007a : SimTemplate //starfall choice left
    {

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(2) : p.getEnemySpellDamageDamage(2);
            p.allMinionOfASideGetDamage(!ownplay, dmg);
        }

    }
}