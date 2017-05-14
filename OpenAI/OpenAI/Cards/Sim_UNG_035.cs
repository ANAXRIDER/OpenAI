using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_035 : SimTemplate //* Curious Glimmerroot
    {
        //Battlecry: Look at 3 cards. Guess which one started in your opponent's deck to get a copy of it.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.unknown, own.own);
        }
    }
}