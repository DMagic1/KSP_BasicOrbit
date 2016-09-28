using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit.Unity
{
	[RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
	public class CanvasFader : MonoBehaviour
	{
		[SerializeField]
		private float SlowRate = 0.9f;
		[SerializeField]
		private float FastRate = 0.3f;

		private CanvasGroup canvas;
		private IEnumerator fader;
		private bool allowInterrupt = true;

		protected virtual void Awake()
		{
			canvas = GetComponent<CanvasGroup>();
		}

		public bool Fading
		{
			get { return fader != null; }
		}

		protected void Fade(float to, bool fast, Action call = null, bool interrupt = true)
		{
			if (canvas == null)
				return;

			Fade(canvas.alpha, to, fast ? FastRate : SlowRate, call, interrupt);
		}

		protected void Alpha(float to)
		{
			if (canvas == null)
				return;

			to = Mathf.Clamp01(to);
			canvas.alpha = to;
		}

		private void Fade(float from, float to, float duration, Action call, bool interrupt)
		{
			if (!allowInterrupt)
				return;

			if (fader != null)
				StopCoroutine(fader);

			fader = FadeRoutine(from, to, duration, call, interrupt);
			StartCoroutine(fader);
		}

		private IEnumerator FadeRoutine(float from, float to, float duration, Action call, bool interrupt)
		{
			allowInterrupt = interrupt;

			yield return new WaitForEndOfFrame();

			float f = 0;

			while (f <= 1)
			{
				f += Time.deltaTime / duration;
				Alpha(Mathf.Lerp(from, to, f));
				yield return null;
			}

			if (call != null)
				call.Invoke();

			allowInterrupt = true;

			fader = null;
		}

	}
}
