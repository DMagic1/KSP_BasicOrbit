using BasicOrbit.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace BasicOrbit
{
	public class BasicOrbitTextMeshProHolder : TextMeshProUGUI
	{
		private TextHandler _handler;

		new private void Awake()
		{
			base.Awake();

			_handler = GetComponent<TextHandler>();

			if (_handler == null)
				return;

			_handler.OnTextUpdate.AddListener(new UnityAction<string>(UpdateText));
		}

		private void UpdateText(string t)
		{
			text = t;
		}
	}
}