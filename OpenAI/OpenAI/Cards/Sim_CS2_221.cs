using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_CS2_221 : SimTemplate //spitefulsmith
	{

//    wutanfall:/ eure waffe hat +2 angriff.
        public override void OnEnrageStart(Playfield p, Minion m)
        {
            if (m.own)
            {
                if (p.ownWeaponDurability >= 1)
                {
                    p.minionGetBuffed(p.ownHero, 2, 0);
                    p.ownWeaponAttack += 2;
                }
            }
            else 
            {
                if (p.enemyWeaponDurability >= 1)
                {
                    p.enemyWeaponAttack += 2;
                    p.minionGetBuffed(p.enemyHero, 2, 0);
                }
            }
        }

        public override void OnEnrageStop(Playfield p, Minion m)
        {
            if (m.own)
            {
                if (p.ownWeaponDurability >= 1)
                {
                    p.minionGetBuffed(p.ownHero, -2, 0);
                    p.ownWeaponAttack -= 2;
                }
            }
            else
            {
                if (p.enemyWeaponDurability >= 1)
                {
                    p.enemyWeaponAttack -= 2;
                    p.minionGetBuffed(p.enemyHero, -2, 0);
                }
            }
        }

	}

}