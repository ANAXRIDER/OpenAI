using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_AT_118 : SimTemplate //Grand Crusader
    {

        //Battlecry: Add a random Paladin card to your hand

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardIDEnum.CS2_093, own.own, true); //consecration
        }
    }
}