using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;	// For DllImport.
using System;

public class GstNetworkAudioPlayer:IGstPlayer  {
	


	public static float ProcessAudioPackets([In,Out] float[] srcData,int startIndex,int channelIndex,int count,int stride,int srcChannels,[In,Out] float[] data,int length,int channels)
	{
		return GStLib.mray_gst_ProcessAudioPackets (srcData, startIndex, channelIndex, count, stride, srcChannels, data, length, channels);
	}

	class Wrapper:IGstAudioPlayer
	{
		GstNetworkAudioPlayer _owner;
		public Wrapper(GstNetworkAudioPlayer o)
		{
			_owner=o;
		}
		public bool IsLoaded()
		{
			return _owner.IsLoaded;
		}
		public bool IsPlaying()
		{
			return _owner.IsPlaying;
		}
		public void Play ()
		{
			_owner.Play();
		}
		public void Pause()
		{
			_owner.Pause();
		}
		public void Stop()
		{
			_owner.Stop();
		}
		public void Destroy()
		{
			_owner.Destroy();
		}
		public void Close()
		{
			_owner.Close();
		}
		public int ChannelsCount()
		{
			return _owner.ChannelsCount();
		}
		public int SampleRate()
		{
			return _owner.SampleRate ();
		}
		public bool IsUsingCustomOutput()
		{
			return _owner.IsUsingCustomOutput();
		}
		public bool GrabFrame()
		{
			return _owner.GrabFrame();
		}
		public int GetFrameSize()
		{
			return _owner.GetFrameSize();
		}
		public bool CopyAudioFrame([In,Out]float[] data)
		{
			return _owner.CopyAudioFrame(data);
		}
	}

	Wrapper _audioWrapper;

	public IGstAudioPlayer AudioWrapper
	{
		get{ return _audioWrapper; }
	}

	public GstNetworkAudioPlayer()
	{
		m_Instance = GStLib.mray_gst_createNetworkAudioPlayer();	
		_audioWrapper = new Wrapper (this);
	}
	
	public uint GetAudioPort()
	{
		return GStLib.mray_gst_netAudioPlayerGetAudioPort (m_Instance);
	}
	public override int GetCaptureRate (int index)
	{
		return 0;
	}
	
	public void SetIP(string ip,int audioPort,bool rtcp)
	{		
		GStLib.mray_gst_netAudioPlayerSetIP (m_Instance, ip, audioPort, rtcp);
	}
	public bool CreateStream()
	{		
		return GStLib.mray_gst_netAudioPlayerCreateStream (m_Instance);
	}

	public void SetVolume(float v)
	{
		GStLib.mray_gst_netAudioPlayerSetVolume (m_Instance, v);
	}

	public void SetUseCustomOutput(bool use)
	{
		GStLib.mray_gst_netAudioPlayerUseCustomOutput (m_Instance,use);
	}
	public bool IsUsingCustomOutput()
	{
		return GStLib.mray_gst_netAudioPlayerIsUsingCustomOutput (m_Instance);
	}

	public bool GrabFrame()
	{
		return GStLib.mray_gst_netAudioPlayerGrabFrame (m_Instance);
	}
	public int GetFrameSize()
	{
		return GStLib.mray_gst_netAudioPlayerGetFrameSize (m_Instance);
	}

	public bool CopyAudioFrame([In,Out]float[] data)
	{
		return GStLib.mray_gst_netAudioPlayerCopyAudioFrame (m_Instance,data);
	}

	public int ChannelsCount()
	{
		return GStLib.mray_gst_netAudioPlayerChannelsCount(m_Instance);
	}
	public void SetSampleRate(int r)
	{
		 GStLib.mray_gst_netAudioPlayerSetSampleRate(m_Instance,r);
	}
	public int SampleRate()
	{
		return GStLib.mray_gst_netAudioPlayerSampleRate(m_Instance);
	}
}







