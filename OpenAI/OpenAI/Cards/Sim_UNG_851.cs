using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_851 : SimTemplate //Elise the Trailblazer
    {

        //Battlecry: Shuffle a sealed_Un'Goro pack into_your deck.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own) p.ownDeckSize++;
            else p.enemyDeckSize++;
        }
    }

}