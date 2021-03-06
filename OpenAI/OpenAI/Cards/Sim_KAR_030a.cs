using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_KAR_030a : SimTemplate //Pantry Spider
    {
        // Battlecry: Summon a 1/3 Spider.

        CardDB.Card kid = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.KAR_030);//Cellar Spider

        public override void GetBattlecryEffect(Playfield p, Minion own, Minion target, int choice)
        {
            p.callKid(kid, own.zonepos, own.own, true);
        }
    }
}