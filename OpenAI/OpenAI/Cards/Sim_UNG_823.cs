using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_823 : SimTemplate //Envenom Weapon
    {

        //Give your weapon Poisonous.

        public override void OnCardPlay(Playfield p, bool ownplay, Minion target, int choice)
        {
            if (p.ownWeaponDurability >= 1) p.ownHero.poisonous = true;
        }


    }

}