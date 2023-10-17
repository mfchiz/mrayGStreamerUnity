using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GstCustomVideoStreamer:IGstStreamer {

	GstUnityImageGrabber _grabber;

	public GstCustomVideoStreamer()
	{
		GStreamerCore.Ref();
		m_Instance = GStLib.mray_gst_createCustomImageStreamer();	
	}
	
	
	public void SetPipelineString(string pipeline)
	{		
		GStLib.mray_gst_customImageStreamerSetPipeline (m_Instance, pipeline);
	}

	public void SetGrabber(GstUnityImageGrabber grabber)
	{
		_grabber = grabber;
		if(grabber!=null)
			GStLib.mray_gst_customImageStreamerSetGrabber (m_Instance, grabber.Instance);
		else
			GStLib.mray_gst_customImageStreamerSetGrabber (m_Instance, System.IntPtr.Zero);
	}

	public void SetResolution(int w,int h,int fps)
	{
		GStLib.mray_gst_customImageStreamerSetResolution (m_Instance, w,h,fps);
	}
}
