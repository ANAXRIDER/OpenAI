using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_UNG_011 : SimTemplate //* Hydrologist
    {
        //Battlecry: Discover a Secret.

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.CardToHand(CardDB.cardName.noblesacrifice, own.own); //assume always pick sacrifice
        }
    }
}