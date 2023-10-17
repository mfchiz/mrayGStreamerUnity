using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GstNetworkAudioStreamer:IGstStreamer {


	GstUnityImageGrabber _grabber;

	public GstNetworkAudioStreamer()
	{
		m_Instance = GStLib.mray_gst_createAudioNetworkStreamer();	
	}
	
	
	public void AddClient(string ip,int port)
	{		
		GStLib.mray_gst_audioStreamerAddClient(m_Instance, ip, port);
	}
	public void RemoveClient(int i)
	{		
		GStLib.mray_gst_audioStreamerRemoveClient(m_Instance, i);
	}
	public int GetClientCount()
	{		
		return GStLib.mray_gst_audioStreamerGetClientCount(m_Instance);
	}
	public string GetClient(int i)
	{		
		return GStLib.mray_gst_audioStreamerGetClient(m_Instance, i);
	}

	public void ClearClients()
	{		
		GStLib.mray_gst_audioStreamerClearClients(m_Instance);
	}

	public void SetClientVolume(int i,float volume)
	{		
		GStLib.mray_gst_audioStreamerSetClientVolume(m_Instance,i,volume);
	}


	public void SetChannels(int c)
	{
		GStLib.mray_gst_audioStreamerSetChannels (m_Instance, c);
	}

}
