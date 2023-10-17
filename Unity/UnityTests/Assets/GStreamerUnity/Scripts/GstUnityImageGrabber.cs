using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

public class GstUnityImageGrabber {
	

	byte[] m_Texture;
	int m_width,m_height;
	//protected Color32[] m_Pixels;
	//protected GCHandle m_PixelsHandle;
	EPixelFormat m_format;

	protected System.IntPtr m_Instance;

	IntPtr m_lastArrayPtr=IntPtr.Zero;
	int m_lastArrayLength = 0;

	public System.IntPtr Instance
	{
		get{
			return m_Instance;
		}
	}

	public GstUnityImageGrabber()
	{
		GStreamerCore.Ref ();
		m_Instance = GStLib.mray_gst_createUnityImageGrabber();	
	}

	public void Destroy()
	{
		GStLib.mray_gst_UnityImageGrabberDestroy (m_Instance);
	}

	public void SetTexture2D(Texture2D tex)
	{
		SetTexture2D (tex.GetRawTextureData (), tex.width, tex.height, tex.format);
	}
	public void SetTexture2D(byte[] texture,int width,int height,TextureFormat format)
	{
		m_Texture = texture;
		m_width = width;
		m_height = height;
		if (m_Texture == null) {
			return;
		}

		if (m_Texture.Length != m_lastArrayLength) {
			if(m_lastArrayPtr!=IntPtr.Zero)
				Marshal.FreeHGlobal(m_lastArrayPtr);
			
			m_lastArrayPtr = Marshal.AllocHGlobal(m_Texture.Length);
		}

		switch (format) {
		case TextureFormat.ARGB32:
		case TextureFormat.RGBA32:
			m_format=EPixelFormat.EPixel_R8G8B8A8;
			break;
		case TextureFormat.Alpha8:
			m_format=EPixelFormat.EPixel_Alpha8;
			break;
		case TextureFormat.RGB24:
			m_format=EPixelFormat.EPixel_R8G8B8;
			break;
		}
	}

	public void Update()
	{
		if (m_Texture == null)
			return;
	//	m_Pixels = m_Texture.GetPixels32 (0);
	//	m_PixelsHandle = GCHandle.Alloc(m_Pixels, GCHandleType.Pinned);

		Marshal.Copy(m_Texture, 1, m_lastArrayPtr, m_Texture.Length-1);

		GStLib.mray_gst_UnityImageGrabberSetData (m_Instance, m_lastArrayPtr, m_width,m_height, (int)m_format);
	}
}
