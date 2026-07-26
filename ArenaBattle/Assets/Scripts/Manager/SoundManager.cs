using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
//using PiratePanic;

public static class SoundConstants
{
	public const string ButtonClick01 = "ButtonClick01";
	public const string CardDrop01 = "CardDrop01";
	public const string ShipShoot01 = "ShipShoot01";
}

public class SoundManager : MonoBehaviour
{
	#region Singleton
	private static SoundManager instance = null;

	public static SoundManager Instance
	{
		get
		{
			if (instance == null)
			{
				GameObject go = new GameObject("SoundManager");
				instance = go.AddComponent<SoundManager>();
				ImmortalGameObject.AttachObject(go);
			}

			return instance;
		}
	}
	#endregion


	//[SerializeField]
	//private GameConfiguration _gameConfiguration = null;

	[SerializeField]
	private List<AudioClip> _audioClips = new List<AudioClip>();

	[SerializeField]
	private List<AudioSource> _audioSources = new List<AudioSource>();

	public void PlayAudioClip(string audioClipName)
	{
		foreach (AudioClip audioClip in _audioClips)
		{
			if (audioClip.name == audioClipName)
			{
				PlayAudioClip(audioClip);
				return;
			}
		}
	}

	public void PlayAudioClip(AudioClip audioClip)
	{
		//if (!_gameConfiguration.IsAudioEnabled)
		//{
		//	return;
		//}

		//foreach (AudioSource audioSource in _audioSources)
		//{
		//	if (!audioSource.isPlaying)
		//	{
		//		audioSource.volume = _gameConfiguration.AudioVolume;
		//		audioSource.clip = audioClip;
		//		audioSource.Play();
		//		return;
		//	}
		//}
	}

	public void PlayButtonClick()
	{
		PlayAudioClip(SoundConstants.ButtonClick01);
	}

	protected void Start()
	{
		GameObject.DontDestroyOnLoad(gameObject);
	}

	/************************************************************************/
	// Variables
	public Sound SoundBGM { get; private set; }

	public float VolumeBGM
	{
		get { return volBGM; }
		set
		{
			volBGM = value;
			// UnityEngine.Debug.LogWarning($"sound volume bgm {volBGM}");

			if (null != SoundBGM)
				SoundBGM.Volume = volBGM;
		}
	}

	public float VolumeEffect
	{
		get { return volEffect; }
		set
		{
			volEffect = value;
			// UnityEngine.Debug.LogWarning($"sound volume eff {volEffect}");

			foreach (Sound sound in soundList)
			{
				sound.Volume = volEffect;
			}

		}
	}

	public bool CompleteLoadFadeBGM { get; private set; }

	private List<Sound> soundList;
	private Dictionary<string, AudioClip> clips;
	private SoundFadeEvent fadeEvent;
//	private Coroutine checkVolumeCoroutine = null;

	[HideInInspector]
	[SerializeField]
	float volBGM = 1.0f;
	[HideInInspector]
	[SerializeField]
	float volEffect = 1.0f;

	/************************************************************************/
	// Functions

	#region Initialzation and event functions
	public void Initialize()
	{
	}

	void Awake()
	{
		//if (true == Application.isEditor) volumeBGM = 0.0f;
		soundList = new List<Sound>();
		clips = new Dictionary<string, AudioClip>();
		SoundBGM = null;
	}

	void Update()
	{
//		if (null != SoundBGM)
//			SoundBGM.Update();

		soundList.RemoveAll(UpdateInstanceSoundCallback);

		if (null != fadeEvent)
			fadeEvent.Update();
	}
	#endregion

	#region Public methods
	public void LoadBGM(string path, bool isloop = true)
	{
		Sound bgm = new Sound();
		bgm.SetSound(path, false, true, isloop, LoadFinshBGMCallback);
	}

	public void LoadBGMWithTag(string soundname)
	{
		LoadBGM(soundname);
	}

	public void SetBGM(AudioClip audio_clip)
	{
		Sound bgm = new Sound();
		bgm.SetSound(audio_clip, false, true, true);
		LoadFinshBGMCallback(bgm);
	}

	public void LoadEventBGM(string path, float fadeInTime, float fadeOutTime)
	{
		if (null == fadeEvent)
			fadeEvent = new SoundFadeEvent();

		CompleteLoadFadeBGM = false;
		fadeEvent.SetEvent(SoundBGM, path, volBGM, fadeInTime, fadeOutTime, FadeEventEndCallback);
	}

	public void LoadEventBGM(AudioClip nextBGM, float fadeInTime, float fadeOutTime)
	{
		if (null == fadeEvent)
			fadeEvent = new SoundFadeEvent();

		CompleteLoadFadeBGM = false;
		fadeEvent.SetEvent(SoundBGM, nextBGM, volBGM, fadeInTime, fadeOutTime, FadeEventEndCallback);
	}

	public void AddAudioClip(string newAudioName, AudioClip newClip)
	{
		if (clips == null)
			return;

		string audioName = newAudioName;

		if (IsLoadedAudioClip(audioName))
		{
			return;
		}

		clips.Add(audioName, newClip);
	}

	public bool IsLoadedAudioClip(string audioname)
	{
		return clips.ContainsKey(audioname);
	}

	public AudioClip GetLoadedAudioClip(string audioname)
	{
		string audioPath = audioname;

		if (false == IsLoadedAudioClip(audioPath))
		{
			return null;
		}

		return clips[audioPath];
	}

	public void PlayBGM()
	{
		if (null != SoundBGM)
		{
			SoundBGM.Volume = volBGM;
			SoundBGM.Play();
		}
	}

	public void StopBGM()
	{
		if (null != SoundBGM)
		{
			SoundBGM.Stop();
			SoundBGM.Destroy();
			SoundBGM = null;
		}
	}

	public Sound PlayInstanceSound(int index, System.Action onPlayDoneEvent = null, Sound sound = null)
	{
		if (sound != null)
			StopInstanceSound(sound);

		Generated.CsvData.soundData data = CSVDataManager.GetTable<SoundTable>().GetData(index);

		if(data == null)
		{
			Debug.LogError($"{index} Sound/Effect is not Exist!!!!");
			return null;
		}

		return PlayInstanceSound(data.filename, false, onPlayDoneEvent);
	}

	public Sound PlayInstanceSound(string path, System.Action onPlayDoneEvent = null, Sound sound = null)
	{
		if (sound != null)
			StopInstanceSound(sound);

		return PlayInstanceSound(path, false, onPlayDoneEvent);
	}

	public Sound PlayInstanceSoundWithTag(string tag, System.Action onPlayDoneEvent = null, Sound sound = null)
	{
		if (string.IsNullOrEmpty(tag))
			return null;

		return PlayInstanceSound(tag, onPlayDoneEvent, sound);
	}

	public Sound PlayInstanceSound(string path, bool b3D, System.Action onPlayDoneEvent = null)
	{
		if (string.IsNullOrEmpty(path) == true)
			return null;

		Sound sound = new Sound();

		// string clipname = Path.GetFileNameWithoutExtension(path);
		string clipname = path;

		if (true == clips.TryGetValue(clipname, out AudioClip clip))
		{
			sound.Path = path;
			sound.SetSound(clip, b3D, false, false);
			sound.OnPlayDoneEvent = onPlayDoneEvent;
			PlaySound(sound);
		}
		else
		{
			sound.OnPlayDoneEvent = onPlayDoneEvent;
			sound.SetSound(path, b3D, false, false, LoadFinishSoundCallback);
		}

		return sound;
	}

	public void StopInstanceSound(Sound sound)
	{
		if (sound == null)
			return;

		if (sound.IsPlaying)
		{
			sound.Stop();
			sound.Destroy();
		}
	}

	public void StopSound(Sound sound)
	{
		if (sound == null)
			return;

		if (sound.IsPlaying)
		{
			sound.Stop();
		}
	}

	public void PlaySound(Sound sound)
	{
		sound.Volume = volEffect;
		for (int i = 0; i < soundList.Count; ++i)
		{
			if (soundList[i].Path == sound.Path)
			{
				soundList.RemoveAt(i);
				soundList.Add(sound);
				break;
			}
		}

		if (!soundList.Contains(sound))
			soundList.Add(sound);

		if (sound.IsReadyToPlay)
		{
			sound.Play();
		}
	}

	public void LoadAudioClipCallback(AssetLoader ld, object p)
	{
		if (false == ld.IsLoadSucceed)
		{
			return;
		}

		AudioClip clip = ld.MainAsset as AudioClip;
		string key = p as string;

		if (false == clips.ContainsKey(key))
		{
			clips.Add(key, clip);
		}
	}

	public void Release()
	{
		foreach (Sound sound in soundList)
		{
			sound.Destroy();
		}

		soundList.Clear();
		clips.Clear();

		if (null != fadeEvent)
			fadeEvent.Release();

		if (null != SoundBGM)
			SoundBGM.Destroy();

		fadeEvent = null;
		SoundBGM = null;
	}
/*
	public string GetSoundStringWithTag(string tag)
	{
		if (string.IsNullOrEmpty(tag) == true || GenDataMgr.Instance.MasterStaticData == null)
			return "";

		var soundElm = GenDataMgr.Instance.MasterStaticData.Sounds;

		if (soundElm.ContainsKey(tag) == false)
		{
			Debug.LogWarning($"Tag {tag} not found in sound path data");
			return tag;
		}

		return soundElm[tag].SoundPath;
	}
*/
	public void FadeBGM(float targetVolume, float duration = 0.1f)
	{
		if (null != SoundBGM && SoundBGM.AudioSrc != null)
		{
			if (SoundBGM.Volume == targetVolume)
				return;

			SoundBGM.AudioSrc.DOKill();
			SoundBGM.AudioSrc.DOFade(targetVolume, duration).SetEase(Ease.Linear).onComplete = () =>
			{
				VolumeBGM = targetVolume;
			};
			//DOTween.To(() => SoundBGM.Volume, x => SoundBGM.Volume = x, targetVolume, 0.4f).SetEase(Ease.Linear).
			//	OnComplete(()=> { VolumeBGM = targetVolume; });
		}
		else
		{
			VolumeBGM = targetVolume;
		}
	}
	#endregion Public methods

	#region Private methods
	public void StopAllSound()
	{
		foreach (var sound in soundList)
		{
			sound.Stop();
		}
	}

	private void LoadFinshBGMCallback(Sound bgmCallback)
	{
		if (null != SoundBGM)
		{
			SoundBGM.Destroy();
			SoundBGM = null;
		}

		SoundBGM = bgmCallback;

		if (null != SoundBGM)
		{
			SoundBGM.Volume = volBGM;

			if (SoundBGM.IsReadyToPlay)
			{
				SoundBGM.Play();

//				Messenger.Broadcast(Definition.READY_MUSIC);
			}
			else
			{
				Debug.Log("load fail... BGM \npath : " + SoundBGM.Path);
			}
		}
	}

	private void FadeEventEndCallback(Sound bgmCallback)
	{
		if (null != SoundBGM)
		{
			SoundBGM.Destroy();
			SoundBGM = null;
		}

		SoundBGM = bgmCallback;
		CompleteLoadFadeBGM = true;
	}

	private void LoadFinishSoundCallback(Sound sound)
	{
		if (null == sound)
		{
			return;
		}

		PlaySound(sound);
	}

	private bool UpdateInstanceSoundCallback(Sound sound)
	{
		if (null == sound)
		{
			return false;
		}

		sound.Update();
		return (null == sound.AudioSrc);
	}

	private IEnumerator CheckVolume()
	{
		while (true)
		{
			yield return null;
			VolumeBGM = 1;
			VolumeEffect = 1;
		}
	}

	#endregion Private methods

	#region Static methods
	public static void FadeOutSound(Sound sound)
	{
		DOTween.To(() => sound.Volume, x => sound.Volume = x, 0f, 1.5f).SetEase(Ease.OutQuart);
	}
	#endregion
}
