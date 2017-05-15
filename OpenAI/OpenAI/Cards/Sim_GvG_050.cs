using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_GVG_050 : SimTemplate //Bouncing Blade
    {

        //   Deal $1 damage to a random minion. Repeat until a minion dies.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            int dmg = (ownplay) ? p.getSpellDamageDamage(1) : p.getEnemySpellDamageDamage(1);
            
            int minHp = 100000;
            foreach (Minion m in p.ownMinions)
            {
                int div = 0;
                if (m.divineshild) div = 1;
                if (m.Hp + div < minHp) minHp = m.Hp;
            }
            foreach (Minion m in p.enemyMinions)
            {
                int div = 0;
                if (m.divineshild) div = 1;
                if (m.Hp + div < minHp) minHp = m.Hp;
            }

            int dmgdone = (int)Math.Ceiling((double)minHp / (double)dmg) * dmg;
            for (int i = 0; i < dmgdone; i++)
            {
                p.allMinionsGetDamage(1);
            }
        }
    }
}