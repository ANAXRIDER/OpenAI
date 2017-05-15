using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_LOE_110 : SimTemplate //Ancient Shade
    {

        //Battlecry: Shuffle an 'Ancient Curse' into your deck that deals 7 damage to you when drawn.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own)
            {
                p.ownDeckSize++;
            }
            else
            {
                p.enemyDeckSize++;
            }
        }
    }
}