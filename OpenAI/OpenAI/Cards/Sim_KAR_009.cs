using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_009 : SimTemplate //Babbling Book
    {
        // Battlecry: Add a random Mage spell to your hand.
        
        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.drawACard(CardDB.cardName.unknown, own.own, true);
        }
    }
}