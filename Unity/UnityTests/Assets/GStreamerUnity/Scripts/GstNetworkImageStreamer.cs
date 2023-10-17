using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GstNetworkImageStreamer:IGstStreamer {
	

	GstUnityImageGrabber _grabber;

	public GstNetworkImageStreamer()
	{
		m_Instance = GStLib.mray_gst_createNetworkStreamer();	
	}
	
	
	public void SetIP(string ip,int videoPort,bool rtcp)
	{		
		GStLib.mray_gst_netStreamerSetIP (m_Instance, ip, videoPort, rtcp);
	}

	public void SetGrabber(GstUnityImageGrabber grabber)
	{
		_grabber = grabber;
		GStLib.mray_gst_netStreamerSetGrabber (m_Instance, grabber.Instance);
	}
	
	public void SetBitRate(int bitrate)
	{
		GStLib.mray_gst_netStreamerSetBitRate (m_Instance, bitrate);
	}
	public void SetResolution(int w,int h,int fps)
	{
		GStLib.mray_gst_netStreamerSetResolution (m_Instance, w,h,fps);
	}
}
