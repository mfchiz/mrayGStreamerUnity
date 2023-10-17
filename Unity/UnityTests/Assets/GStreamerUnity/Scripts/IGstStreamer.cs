using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class IGstStreamer : MonoBehaviour {
	
	
	protected System.IntPtr m_Instance;
	
	public bool IsStreaming {
		get {
			return GStLib.mray_gst_StreamerIsStreaming(m_Instance);
		}
	}

	public System.IntPtr NativePtr
	{
		get
		{
			return m_Instance;
		}
	}
	
	public void Destroy()
	{
		GStLib.mray_gst_StreamerDestroy (m_Instance);
	}
	public bool CreateStream()
	{		
		return GStLib.mray_gst_StreamerCreateStream (m_Instance);
	}
	
	public void Stream()
	{
		GStLib.mray_gst_StreamerStream (m_Instance);
	}
	
	public void Pause()
	{
		GStLib.mray_gst_StreamerPause (m_Instance);
	}
	public void Resume()
	{
		GStLib.mray_gst_StreamerResume (m_Instance);
	}
	
	public void Stop()
	{
		GStLib.mray_gst_StreamerStop (m_Instance);
	}
	
	public void Close()
	{
		GStLib.mray_gst_StreamerClose (m_Instance);
	}
}
