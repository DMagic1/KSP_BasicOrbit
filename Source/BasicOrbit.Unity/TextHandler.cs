using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BasicOrbit.Unity
{
	public class TextHandler : MonoBehaviour
	{
		public class OnTextEvent : UnityEvent<string> { }

		private OnTextEvent _onTextUpdate = new OnTextEvent();

		public UnityEvent<string> OnTextUpdate
		{
			get { return _onTextUpdate; }
		}
	}
}