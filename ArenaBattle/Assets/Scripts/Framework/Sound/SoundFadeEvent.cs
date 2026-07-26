using System.Collections.Generic;
using UnityEngine;

public class SoundFadeEvent
{
	public delegate void cbFadeinEnd(Sound _sound);

	public enum EVENT_FADE_TYPE
	{
		FADE_NULL,
		FADE_IN,
		FADE_OUT,
	}

	private enum FADE_STEP
	{
		BEGIN,
		WAIT_LOAD,
		DELAY,
		IN,
		OUT,
		PLAY,
		END,
		IDLE,
	}

	private class FadeEvent
	{
		public string nextBGM;
		public AudioClip nextBGMClip;

		public Sound src;
		public Sound next;

		public float inTime;
		public float outTime;
		public float volume;

		public cbFadeinEnd cb;
	}

	FadeEvent cur;
	FadeEvent reserve;

	private FADE_STEP step = FADE_STEP.IDLE;

	float currTime = 0.0f;
	float delayTime = 1.0f;

	Queue<FADE_STEP> queFadeStep = new Queue<FADE_STEP>();

	public bool SetEvent(Sound src, string nextBGM, float volume, float inTime, float outTime, cbFadeinEnd cb)
	{
		if (string.IsNullOrEmpty(nextBGM))
		{
			return false;
		}

		if (FADE_STEP.IDLE != step)
		{
			reserve = new FadeEvent
			{
				src = null,
				next = null,

				nextBGM = nextBGM,
				inTime = inTime,
				outTime = outTime,
				volume = volume,
				cb = cb
			};
		}
		else
		{
			cur = new FadeEvent
			{
				src = src,
				next = null,
				nextBGM = nextBGM,
				inTime = inTime,
				outTime = outTime,
				volume = volume,
				cb = cb
			};

			step = FADE_STEP.BEGIN;

			AddFadeStep(FADE_STEP.WAIT_LOAD);
			AddFadeStep(FADE_STEP.OUT);
			AddFadeStep(FADE_STEP.DELAY);
			AddFadeStep(FADE_STEP.PLAY);
			AddFadeStep(FADE_STEP.IN);
			AddFadeStep(FADE_STEP.END);

			Sound bgm = new Sound();
			bgm.SetSound(cur.nextBGM, false, true, true, cbLoadFinish);
		}

		return true;
	}


	public bool SetEvent(Sound src, AudioClip nextBGM, float volume, float inTime, float outTime, cbFadeinEnd cb)
	{
		if (nextBGM == null)
		{
			return false;
		}

		if (FADE_STEP.IDLE != step)
		{
			reserve = new FadeEvent
			{
				src = null,
				next = null,

				nextBGMClip = nextBGM,
				inTime = inTime,
				outTime = outTime,
				volume = volume,
				cb = cb
			};
		}
		else
		{
			cur = new FadeEvent
			{
				src = src,
				next = null,
				nextBGMClip = nextBGM,
				inTime = inTime,
				outTime = outTime,
				volume = volume,
				cb = cb
			};

			step = FADE_STEP.BEGIN;

			AddFadeStep(FADE_STEP.WAIT_LOAD);
			AddFadeStep(FADE_STEP.OUT);
			AddFadeStep(FADE_STEP.DELAY);
			AddFadeStep(FADE_STEP.PLAY);
			AddFadeStep(FADE_STEP.IN);
			AddFadeStep(FADE_STEP.END);

			Sound bgm = new Sound();
			bgm.SetSound(cur.nextBGMClip, false, true, true);
			cbLoadFinish(bgm);
		}

		return true;
	}

	public void Update()
	{
		FadeStep(step);
	}

	public void Release()
	{
		queFadeStep.Clear();
		step = FADE_STEP.IDLE;

		if (null != cur)
		{
			if (null != cur.next)
				cur.next.Destroy();
		}

		cur = null;
		reserve = null;
	}

	private void AddFadeStep(FADE_STEP step)
	{
		if (null != queFadeStep)
			queFadeStep.Enqueue(step);
	}

	private void FadeStep(FADE_STEP step)
	{
		switch (step)
		{
			case FADE_STEP.IDLE:
				break;

			case FADE_STEP.BEGIN:
				MoveToNextStep();
				break;

			case FADE_STEP.WAIT_LOAD:
				if (null != cur.next) MoveToNextStep();
				break;

			case FADE_STEP.DELAY:
				if (true == OnFadeDelay()) MoveToNextStep();
				break;

			case FADE_STEP.IN:
				if (true == OnFadeIn()) MoveToNextStep();
				break;

			case FADE_STEP.OUT:
				if (true == OnFadeOut()) MoveToNextStep();
				break;

			case FADE_STEP.PLAY:
				{
					if (null != cur.next)
					{
						cur.next.Volume = 0.0f;
						cur.next.Play();
					}

					MoveToNextStep();
				}
				break;

			case FADE_STEP.END:
				{
					if (null != cur)
					{
						cur.cb?.Invoke(cur.next);
					}

					if (null != reserve)
					{
						if (reserve.nextBGM == cur.next.Path)
						{
							reserve = null;
							MoveToNextStep();
						}
						else
						{
							reserve.src = cur.next;

							cur = reserve;
							reserve = null;

							step = FADE_STEP.BEGIN;

							AddFadeStep(FADE_STEP.WAIT_LOAD);
							AddFadeStep(FADE_STEP.OUT);
							AddFadeStep(FADE_STEP.DELAY);
							AddFadeStep(FADE_STEP.PLAY);
							AddFadeStep(FADE_STEP.IN);
							AddFadeStep(FADE_STEP.END);

							Sound bgm = new Sound();
							bgm.SetSound(cur.nextBGM, false, true, true, cbLoadFinish);
						}
					}
					else
					{
						cur = null;
						MoveToNextStep();
					}
				}
				break;
		}
	}

	private void MoveToNextStep()
	{
		if (queFadeStep.Count != 0)
			step = queFadeStep.Dequeue();
		else
			step = FADE_STEP.IDLE;

		Debug.Log("MoveToNextStep " + step);
	}

	private bool OnFadeDelay()
	{
		delayTime -= Time.deltaTime;
		return delayTime <= 0.0f;
	}

	private bool OnFadeIn()
	{
		if (null == cur.next)
		{
			return true;
		}

		currTime += Time.deltaTime;
		cur.next.Volume = (cur.volume * ((currTime) / cur.inTime));

		if (cur.volume <= cur.next.Volume)
		{
			cur.next.Volume = cur.volume;
			currTime = 0.0f;
			return true;
		}

		return false;
	}

	private bool OnFadeOut()
	{
		if (null == cur.src)
		{
			return true;
		}

		currTime += Time.deltaTime;
		cur.src.Volume = (cur.volume * ((cur.outTime - currTime) / cur.outTime));

		if (cur.src.Volume <= 0.0f)
		{
			cur.src.Stop();
			currTime = 0.0f;
			return true;
		}

		return false;
	}

	private void cbLoadFinish(Sound bgm)
	{
		cur.next = bgm;

		if (null != cur.next)
		{
			cur.next.Loop = true;
			cur.next.Volume = cur.volume;
		}
	}
}
