using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Pen_EX1_134 : PenTemplate //si7agent
	{
		public override float getPlayPenalty(Playfield p, Handmanager.Handcard hc, Minion target, int choice, bool isLethal)
		{
			return 0;
		}
	}
}
