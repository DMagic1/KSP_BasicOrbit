using UnityEngine;

namespace BasicOrbit.Unity
{
	public class BasicUIStyle
	{
		private Color color;
		private Font font;
		private int size;
		private FontStyle style;

		public Color Color
		{
			get { return color; }
			set { color = value; }
		}

		public Font Font
		{
			get { return font; }
			set { font = value; }
		}

		public int Size
		{
			get { return size; }
			set { size = value; }
		}

		public FontStyle Style
		{
			get { return style; }
			set { style = value; }
		}
	}
}
