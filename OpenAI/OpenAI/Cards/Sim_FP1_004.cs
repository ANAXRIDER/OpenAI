﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_FP1_004 : SimTemplate//Mad Scientist
    {
        //<deDE>TodesrÃ¶cheln: Legt ein Geheimnis aus Eurem Deck auf das Schlachtfeld.

        public override void OnDeathrattle(Playfield p, Minion m)
        {
            //if(p.isServer)

            if (m.own)
            {
                if (p.ownHeroName == HeroEnum.mage)
                {
                    p.ownSecretsIDList.Add(CardDB.cardIDEnum.EX1_289);
                }
                if (p.ownHeroName == HeroEnum.hunter)
                {
                    p.ownSecretsIDList.Add(CardDB.cardIDEnum.EX1_554);
                }
                if (p.ownHeroName == HeroEnum.pala)
                {
                    p.ownSecretsIDList.Add(CardDB.cardIDEnum.EX1_130);
                }
            }
            else
            {
                if (p.enemyHeroName == HeroEnum.mage || p.enemyHeroName == HeroEnum.hunter || p.enemyHeroName == HeroEnum.pala)
                {
                    if (p.enemySecretCount <= 4)
                    {
                        p.enemySecretCount++;
                        SecretItem si = Probabilitymaker.Instance.getNewSecretGuessedItem(p.getNextEntity(), p.enemyHeroName);
                        if (p.enemyHeroName == HeroEnum.pala)
                        {
                            si.canBe_redemption = false;
                        }
                        if (Settings.Instance.useSecretsPlayArround)
                        {
                            p.enemySecretList.Add(si);
                        }
                    }
                }
            }
            
        }
    }

}
