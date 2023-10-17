using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;	// For DllImport.
using System;

public class GstMultipleNetworkPlayer:IGstPlayer  {
	
	
	public EPixelFormat Format
	{
		set{
			GStLib.mray_gst_multiNetPlayerSetFormat(m_Instance,(int)value);
		}
		get{
			return (EPixelFormat)GStLib.mray_gst_multiNetPlayerGetFormat(m_Instance);
		}
	}

	public Vector2 FrameSize
	{
		get
		{
			int w=0,h=0,comp=0;
			GStLib.mray_gst_multiNetPlayerGetFrameSize(m_Instance,ref w,ref h,ref comp);
			return new Vector2(w,h);
		}
	}
	IntPtr m_arrayPtr=IntPtr.Zero;
	int m_arraySize=0;

	public ulong NetworkUsage
	{
		get{
			return GStLib.mray_gst_multiNetPlayerGetNetworkUsage(m_Instance);
		}
	}
	
	public GstMultipleNetworkPlayer()
	{
		m_Instance = GStLib.mray_gst_createNetworkMultiplePlayer();	
	}

	public uint GetVideoPort(int index)
	{
		return GStLib.mray_gst_multiNetPlayerGetVideoPort (m_Instance, index);
	}
	
	public override int GetCaptureRate (int index)
	{
		return GStLib.mray_gst_multiNetPlayerFrameCount (m_Instance,index);
	}
	
	public void SetIP(string ip,int baseVideoPort,int count,bool rtcp)
	{		
		GStLib.mray_gst_multiNetPlayerSetIP (m_Instance, ip, baseVideoPort,count, rtcp);
	}
	public void SetDecoder(string dec)
	{		
		GStLib.mray_gst_multiNetPlayerSetDecoderType(m_Instance,dec);
	}
	public bool CreateStream()
	{		
		return GStLib.mray_gst_multiNetPlayerCreateStream (m_Instance);
	}
	
	public bool GrabFrame(out Vector2 frameSize,out int comp,int index)
	{
		int w=0,h=0,c=0;
		if(GStLib.mray_gst_multiNetPlayerGrabFrame(m_Instance,ref w,ref h,index))
		{
			GStLib.mray_gst_multiNetPlayerGetFrameSize(m_Instance,ref w,ref h,ref c);
			comp=c;
			frameSize.x=w;
			frameSize.y=h;
			return true;
		}
		comp = 3;
		frameSize.x = frameSize.y = 0;
		return false;
	}

	public System.IntPtr CopyTextureData( byte[] _dataPtr,int index )
	{
		int w=0,h=0,comp=0;
		GStLib.mray_gst_multiNetPlayerGetFrameSize(m_Instance,ref w,ref h,ref comp);
		int len = w * h * comp;
		if (_dataPtr!=null && _dataPtr.Length != w * h * comp)
			return System.IntPtr.Zero;
		if (m_arraySize!=len ||
			_dataPtr!=null && m_arraySize != _dataPtr.Length) {

			if(m_arrayPtr!=IntPtr.Zero)
				Marshal.FreeHGlobal(m_arrayPtr);

			m_arraySize = len;
			m_arrayPtr = Marshal.AllocHGlobal(m_arraySize);
		}

		GStLib.mray_gst_multiNetPlayerCopyData(m_Instance, m_arrayPtr,index);	
		if(_dataPtr!=null)
			Marshal.Copy(m_arrayPtr,_dataPtr,0, m_arraySize);
		return m_arrayPtr;
	}
	
	public void BlitTexture( System.IntPtr _NativeTexturePtr, int _TextureWidth, int _TextureHeight,int index )
	{
		if (_NativeTexturePtr == System.IntPtr.Zero) return;
		
		Vector2 sz = FrameSize;
		if (_TextureWidth != sz.x || _TextureHeight != sz.y) return;	// For now, only works if the texture has the exact same size as the webview.
		
		GL.IssuePluginEvent(GStLib.mray_gst_multiNetPlayerBlitImageNativeGLCall(m_Instance, _NativeTexturePtr, _TextureWidth, _TextureHeight,index), 1);
		//GStLib.mray_gst_multiNetPlayerBlitImage(m_Instance, _NativeTexturePtr, _TextureWidth, _TextureHeight,index);	// We pass Unity's width and height values of the texture
	}
}







