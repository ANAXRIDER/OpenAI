using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Pen_LOE_019t : PenTemplate //maptothegoldenmonkey
	{
		public override float getPlayPenalty(Playfield p, Handmanager.Handcard hc, Minion target, int choice, bool isLethal)
		{
			return 0;
		}
	}
}
