using System.Collections;
using UnityEngine;

public class Sound
{
	public Sound()
	{
		refCount = 0;
	}

	public delegate void cbLoadFinish(Sound sound);
	cbLoadFinish cb = null;

	/************************************************************************/
	// Variables

	public bool IsPlaying
	{
		get
		{
			if (null == src) return false;
			if (null == src.clip) return false;
			return src.isPlaying;
		}
	}

	public int RefCount { get { return refCount; } }
	public string Path { get { return path; } set { path = value; } }

	public bool Is3D { get { return is3D; } }
	public bool IsStream { get { return isStream; } }

	public AudioSource AudioSrc
	{
		get { return src; }
	}

	public AudioClip AudioClip
	{
		get { if (null != src) return src.clip; return null; }
	}

	public float Volume
	{
		get { if (null != src) return src.volume; return 0.0f; }
		set { if (null != src) src.volume = value; }
	}

	public bool Loop
	{
		get { return isLoop; }
		set { isLoop = value; }
	}

	public bool IsReadyToPlay
	{
		get
		{
			if (null != src)
			{
				if (null != src.clip)
					return (src.clip.loadState == AudioDataLoadState.Loaded);
			}

			return false;
		}
	}

	public bool is3D = false;
	public bool isStream = false;
	public bool isLoop = false;
	public System.Action OnPlayDoneEvent = null;

	AudioSource src = null;
	int refCount = 0;

	string path;
	bool isStoped = false;

	/************************************************************************/
	// Functions

	public bool SetSound(string path, bool b3D, bool bStream, bool bLoop, cbLoadFinish cb)
	{
		Destroy();

		Path = path;

		is3D = b3D;
		isStream = bStream;

		src = ImmortalGameObject.RootObject.AddComponent<AudioSource>();
		isLoop = bLoop;
		isStoped = false;
		this.cb = cb;

		if (null != src)
			src.loop = false;

		if (false == SoundManager.Instance.IsLoadedAudioClip(path))
		{
			//AudioClip clip = (AudioClip)Resources.Load($"{path}");
			//cbLoadSoundResource(path, clip);
			AssetManager.Instance.LoadAssetAsync<AudioClip>(path, cbLoadSound, path);
		}
		else
		{
			if (null != src)
				src.clip = SoundManager.Instance.GetLoadedAudioClip(path);

			if (null != this.cb)
			{
				this.cb(this);
				this.cb = null;
			}
		}

		return null != src;
	}

	public bool SetSound(AudioClip clip, bool b3D, bool bStream, bool bLoop)
	{
		Destroy();

		if (null == clip)
		{
			return false;
		}

		path = Path;

		is3D = b3D;
		isStream = bStream;

		src = ImmortalGameObject.RootObject.AddComponent<AudioSource>();
		isLoop = bLoop;
		isStoped = false;

		if (null != src)
			src.loop = false;

		src.clip = clip;
		return null != src;
	}

	public void Play()
	{
		isStoped = false;
		if (null != src) src.Play();
	}

	public void Pause()
	{
		isStoped = true;
		if (null != src) src.Pause();
	}

	public void Stop()
	{
		isStoped = true;
		if (null != src) src.Stop();
	}

	public void SetVolume(float f)
	{
		if (null != src) src.volume = f;
	}

	public void AddRef()
	{
		refCount++;
	}

	public void Update()
	{
		if (true == isStoped)
		{
			return;
		}

		// 스트리밍의 경우 loop 에 이상이 생겨서 이런식으로 변경 함
		if (false == IsPlaying)
		{
			if (true == isLoop)
			{
				Play();
			}
			else
			{
				if (0 == refCount) Destroy();
			}
		}
	}

	public void Release()
	{
		refCount--;
	}

	public void Destroy()
	{
		if (null != src)
		{
			GameObject.Destroy(src);
			src = null;
			OnPlayDoneEvent?.Invoke();
		}
	}

	protected void cbLoadSound(AssetLoader ld, object p)
	{
		if (false == ld.IsLoadSucceed)
		{
			return;
		}

		AudioClip newClip = ld.MainAsset as AudioClip;
		string key = p as string;

		if (false == SoundManager.Instance.IsLoadedAudioClip(key))
		{
			SoundManager.Instance.AddAudioClip(key, newClip);
		}

		if(src != null)
			src.clip = newClip;

		if (null != cb)
		{
			cb(this);
			cb = null;
		}
	}

	protected void cbLoadSoundResource(string path, AudioClip clip)
	{
		if (false == SoundManager.Instance.IsLoadedAudioClip(path))
		{
			SoundManager.Instance.AddAudioClip(path, clip);
		}

		if (src != null)
			src.clip = clip;

		if (null != cb)
		{
			cb(this);
			cb = null;
		}
	}
}
