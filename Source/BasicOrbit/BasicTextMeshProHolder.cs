using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit
{
	public class BasicTextMeshProHolder : MonoBehaviour
	{
		private TextMeshProUGUI _textmesh;
		private Text _text;

		public void Setup(TextMeshProUGUI tmp, Text t)
		{
			_textmesh = tmp;
			_text = t;
		}

		private void Update()
		{
			if (_textmesh == null || _text == null)
				return;

			_textmesh.text = _text.text;
		}
	}
}
