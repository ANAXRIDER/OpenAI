using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Sim_FP1_009 : SimTemplate //deathlord
	{
        CardDB.Card c = CardDB.Instance.getCardDataFromID(CardDB.cardIDEnum.EX1_017);//nerubian
//    spott. todesröcheln:/ euer gegner legt einen diener aus seinem deck auf das schlachtfeld.
        public override void OnDeathrattle(Playfield p, Minion m)
        {
            int place = (m.own) ? p.enemyMinions.Count : p.ownMinions.Count;
            p.callKid(c, place, !m.own);
        }
	}
}