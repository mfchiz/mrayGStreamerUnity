using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class GstImageInfo {

	System.IntPtr _instance;

	int _width,_height;
	EPixelFormat _format;

	public int Width{ get{ return _width;} }
	public int Height{ get { return _height; } }
	public EPixelFormat Format{get{return _format;}}

	public static TextureFormat ConvertFormat(EPixelFormat fmt)
	{
		switch (fmt) {
		case EPixelFormat.EPixel_LUMINANCE8:
		case EPixelFormat.EPixel_Alpha8:
		case EPixelFormat.EPixel_I420:
			return TextureFormat.Alpha8;

		case EPixelFormat.EPixel_R8G8B8:
		case EPixelFormat.EPixel_B8G8R8:
			return TextureFormat.RGB24;

		case EPixelFormat.EPixel_R8G8B8A8:
		case EPixelFormat.EPixel_X8R8G8B8:
		case EPixelFormat.EPixel_B8G8R8A8:
		case EPixelFormat.EPixel_X8B8G8R8:
			return TextureFormat.RGBA32;
		}

		return TextureFormat.Alpha8;
	}

	public static int GetChannelsCount(EPixelFormat fmt)
	{
		switch (fmt) {
		case EPixelFormat.EPixel_LUMINANCE8:
		case EPixelFormat.EPixel_Alpha8:
		case EPixelFormat.EPixel_I420:
			return 1;

		case EPixelFormat.EPixel_R8G8B8:
		case EPixelFormat.EPixel_B8G8R8:
			return 3;

		case EPixelFormat.EPixel_R8G8B8A8:
		case EPixelFormat.EPixel_X8R8G8B8:
		case EPixelFormat.EPixel_B8G8R8A8:
		case EPixelFormat.EPixel_X8B8G8R8:
			return 4;
		}

		return 1;
	}
	public GstImageInfo()
	{
		_instance = System.IntPtr.Zero;
		_width = _height = 0;
		_format = EPixelFormat.EPixel_Unkown;

	}

	public System.IntPtr GetInstance(){
		return _instance;
	}

	public void CloneTo(GstImageInfo ifo)
	{
		GStLib.mray_cloneImageData (GetInstance (), ifo.GetInstance ());
	}
	public void CopyFrom(System.IntPtr ifo)
	{
		GStLib.mray_cloneImageData (ifo, GetInstance());
		UpdateInfo ();
	}

	public void CopyCroppedFrom(System.IntPtr ifo,int x,int y,int width,int height,bool clamp)
	{
		GStLib.mray_copyCroppedImageData (ifo, _instance, x, y, width, height, clamp);
		UpdateInfo ();
	}

	public void Create(int width,int height,EPixelFormat fmt)
	{
		_width = width;
		_height = height;
		_format = fmt;
		if (_instance != System.IntPtr.Zero)
			GStLib.mray_resizeImageData (_instance, _width, _height, _format);
		else
			_instance = GStLib.mray_createImageData (_width, _height, _format);
	}

	public void UpdateInfo()
	{
		GStLib.mray_getImageDataInfo (_instance, ref _width,ref  _height,ref  _format);
	}

	public void BlitToTexture(Texture2D tex)
	{
		if (tex.width != _width || tex.height != _height || tex.format != ConvertFormat(_format)) {
			tex.Reinitialize (_width, _height, ConvertFormat(_format), false);
			tex.Apply (false,false);

		}
		GL.IssuePluginEvent(GStLib.mray_BlitImageNativeGLCall(_instance, tex.GetNativeTexturePtr()), 1);
		//GStLib.mray_BlitImageDataInfo (_instance, tex.GetNativeTexturePtr ());
	}

	public void FlipImage(bool horizontal,bool vertical)
	{
		GStLib.mray_FlipImageData (_instance, horizontal, vertical);
	}

	public void CopyImageData(ref byte[] result)
	{
		int targetLen = Width * Height*GetChannelsCount(_format);//*2/3;
		if (result == null || result.Length != targetLen) {
			result = new byte[targetLen];
		}

		var src=GStLib.mray_getImageDataPtr (_instance);

		Marshal.Copy (src,  result, 0,targetLen);
	}

	public void Destory()
	{
		GStLib.mray_deleteImageData (_instance);
		_instance = System.IntPtr.Zero;
	}

}
