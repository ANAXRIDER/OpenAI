﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
    class Sim_EX1_533 : SimTemplate//Misdirection
    {
        public override void OnSecretPlay(Playfield p, bool ownplay, Minion attacker, Minion target, out int number)
        {
            number = 0;
            Minion newTarget = null;
            if (ownplay)
            {
                foreach (Minion m in p.enemyMinions)
                {
                    if (target.entityID != m.entityID && attacker.entityID != m.entityID)
                    {
                        newTarget = m;
                    }
                }

                if (newTarget == null)
                {
                    foreach (Minion m in p.ownMinions)
                    {
                        if (target.entityID != m.entityID && attacker.entityID != m.entityID)
                        {
                            newTarget = m;
                        }
                    }
                }

                if (newTarget == null)
                {
                    newTarget = p.enemyHero;
                }
            }

            else
            {
                foreach (Minion m in p.ownMinions)
                {
                    if (target.entityID != m.entityID && attacker.entityID != m.entityID)
                    {
                        newTarget = m;
                    }
                }

                if (newTarget == null)
                {
                    foreach (Minion m in p.enemyMinions)
                    {
                        if (target.entityID != m.entityID && attacker.entityID != m.entityID)
                        {
                            newTarget = m;
                        }
                    }
                }

                if (newTarget == null)
                {
                    newTarget = p.ownHero;
                }
            }


            if (newTarget != null)
            {
                number = newTarget.entityID;
            }
        }

    }

}
