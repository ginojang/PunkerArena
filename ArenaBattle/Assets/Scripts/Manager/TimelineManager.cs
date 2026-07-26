using System.IO;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class TimelineData
{
	[HideInInspector]
	public PlayableDirector[] m_Director;
	public TimelineInfo[] m_Info;
	[HideInInspector]
	public int m_DirectorIndex;

	public void SetDirector(PlayableDirector[] director)
	{
		m_Director = director;
	}
}

[System.Serializable]
public class TimelineInfo
{
	public float[] m_Time;
	public int[] m_Look;
	public Vector3[] m_LookPos;
	public int[] m_Follow;
	public float[] m_SlowTime;
	public float[] m_SlowScale;
}

public class TimelineManager : MonoBehaviour
{
	private static TimelineManager m_this = null;
	public static TimelineManager Instance
	{
		get
		{
			return m_this;
		}
	}

	public TimelineData[] m_TimelineData;

	public void Awake()
	{
		m_this = this;
	}


	public void IniTimeLine()
	{
        //foreach(TimelineData data in m_TimelineData)
        for (int i = 0; i < m_TimelineData.Length; i++)
        {
            //foreach(PlayableDirector pa in m_TimelineData[i].m_Director)
            for (int j = 0; j < m_TimelineData[i].m_Director.Length; j++)
            {				
				if(m_TimelineData[i].m_Director[j] != null)
                    m_TimelineData[i].m_Director[j].gameObject.SetActive(false);
			}
		}
	}

	public void SetDirector()
	{
		int cnt = m_TimelineData.Length;
		for(int i=0; i<cnt; i++)
		{
			SetDirector(i);
		}
	}

	public void SetDirector(int idx)
	{
		GameObject obj = GameObject.Find("Cinemachine" + idx.ToString("00"));
		if(obj != null)
		{
			PlayableDirector[] director = obj.GetComponentsInChildren<PlayableDirector>();
			m_TimelineData[idx].SetDirector(director);
		}				
	}

	public void SetTenKoku()
	{

	}

	public TimelineData GetTimeLine(int idx)
	{
		if (m_TimelineData.Length <= idx)
			return null;

		return m_TimelineData[idx];
	}
}
