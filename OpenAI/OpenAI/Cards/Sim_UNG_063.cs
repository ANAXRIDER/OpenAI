using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_063 : SimTemplate //Biteweed
    {

        //Combo: Gain +1/+1 for each other card you've played this turn.
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            if (own.own) p.minionGetBuffed(own, p.cardsPlayedThisTurn, p.cardsPlayedThisTurn);
            else p.minionGetBuffed(own, p.enemyAnzCards, p.enemyAnzCards);
        }
    }

}