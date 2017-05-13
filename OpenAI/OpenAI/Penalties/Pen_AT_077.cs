using System;
using System.Collections.Generic;
using System.Text;

namespace OpenAI
{
	class Pen_AT_077 : PenTemplate //argentlance
	{
		public override float getPlayPenalty(Playfield p, Handmanager.Handcard hc, Minion target, int choice, bool isLethal)
		{
			return 0;
		}
	}
}
