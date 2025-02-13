﻿using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;	// For DllImport.
using AOT;
using System;


public class GstCustomPlayer : IGstPlayer
{

    protected static Dictionary<int, GstCustomPlayer> _playerList = new Dictionary<int, GstCustomPlayer>();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void NewVideoSampleCallback(System.IntPtr playerInstance);

    public delegate void OnFrameReady();

	protected NewVideoSampleCallback _videoSampleCallback;

    protected OnFrameReady _onFrameReadyCallback;

    class _AudioWrapper : IGstAudioPlayer
    {
        GstCustomPlayer _owner;
        public _AudioWrapper(GstCustomPlayer o)
        {
            _owner = o;
        }
        public bool IsLoaded()
        {
            return _owner.IsLoaded;
        }
        public bool IsPlaying()
        {
            return _owner.IsPlaying;
        }
        public void Play()
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
            return _owner.SampleRate();
        }
        public bool IsUsingCustomOutput()
        {
            return true;
        }
        public bool GrabFrame()
        {
            return _owner.GrabAudioFrame();
        }
        public int GetFrameSize()
        {
            return _owner.GetFrameSize();
        }
        public bool CopyAudioFrame([In, Out]float[] data)
        {
            return _owner.CopyAudioFrame(data);
        }
    }

    _AudioWrapper _audioWrapper;

    public IGstAudioPlayer AudioWrapper
    {
        get { return _audioWrapper; }
    }

    public Vector2 FrameSize
    {
        get
        {
            int w = 0, h = 0, comp = 0;
            GStLib.mray_gst_customPlayerGetFrameSize(m_Instance, ref w, ref h, ref comp);
            return new Vector2(w, h);
        }
    }
    public Vector2 FrameSizeImage
    {
        get
        {
            int w = 0, h = 0, comp = 0;
            GStLib.mray_gst_customPlayerGetFrameSize(m_Instance, ref w, ref h, ref comp);
            if (comp == 1)
                h = h * 2 / 3;
            return new Vector2(w, h);
        }
    }

    public GstCustomPlayer()
    {
        m_Instance = GStLib.mray_gst_createCustomVideoPlayer();

        //record the player in the list
        _playerList.Add(m_Instance.GetHashCode(), this);
    
        //setup callback delegate from C
        _videoSampleCallback=new NewVideoSampleCallback(OnVideoSample);

        //send to C code
        GStLib.mray_gst_customPlayerSetNewVideoSampleCallback(m_Instance, _videoSampleCallback);

        _audioWrapper = new _AudioWrapper(this);
    }

    public override void Destroy()
    {
  
        _onFrameReadyCallback=null;

        //remove player from list
        _playerList.Remove(m_Instance.GetHashCode());
        
        base.Destroy();
    }

    public static void KillAll()
    {
        foreach( GstCustomPlayer p in _playerList.Values )
        {
            p.Destroy();
        }
    }

    public void SetOnFrameReady(OnFrameReady cb)
    {
        _onFrameReadyCallback=cb;
    }

    [MonoPInvokeCallback(typeof(NewVideoSampleCallback))]
    public static void OnVideoSample(System.IntPtr player)
	{
        if(_playerList.ContainsKey(player.GetHashCode()))
        {     
           _playerList[player.GetHashCode()]._onFrameReadyCallback();
        }
    }


    public override int GetCaptureRate(int index)
    {
        return GStLib.mray_gst_customPlayerFrameCount(m_Instance);
    }


    public void SetPipeline(string pipeline)
    {
        GStLib.mray_gst_customPlayerSetPipeline(m_Instance, pipeline);
    }
    public bool CreateStream()
    {
        return GStLib.mray_gst_customPlayerCreateStream(m_Instance);
    }


    public void SetLoop(bool loop)
    {
        GStLib.mray_gst_customPlayerSetLoop(m_Instance,loop);
    }
    public bool IsLoop()
    {
        return GStLib.mray_gst_customPlayerIsLoop(m_Instance);
    }

    public bool GrabFrame(out Vector2 frameSize, out int components)
    {
        int w = 0, h = 0, c = 0;
        
        if(!GStLib.isUnloaded && m_Instance != IntPtr.Zero)
        {
		    if (GStLib.mray_gst_playerGrabFrame(m_Instance, ref w, ref h, ref c, 0))
		    {
		        components = c;
		        frameSize.x = w;
		        frameSize.y = h;
		        return true;
		    }
		}
        components = 3;
        frameSize.x = frameSize.y = 0;
        return false;
    }
    public bool CopyFrame(GstImageInfo image)
    {
        bool ret = GStLib.mray_gst_customPlayerCopyFrame(m_Instance, image.GetInstance());
        if (ret)
            image.UpdateInfo();
        return ret;
    }
    public bool CopyFrameCropped(GstImageInfo image, int x, int y, int w, int h)
    {
        bool ret = GStLib.mray_gst_customPlayerCropFrame(m_Instance, image.GetInstance(), x, y, w, h);
        if (ret)
            image.UpdateInfo();
        return ret;
    }

    public void BlitTexture(System.IntPtr _NativeTexturePtr, int _TextureWidth, int _TextureHeight)
    {
        if (_NativeTexturePtr == System.IntPtr.Zero) return;

        Vector2 sz = FrameSize;
        if (_TextureWidth != sz.x || _TextureHeight != sz.y) return;    // For now, only works if the texture has the exact same size as the webview.

        GL.IssuePluginEvent(GStLib.mray_gst_customPlayerBlitImageNativeGLCall(m_Instance, _NativeTexturePtr, _TextureWidth, _TextureHeight), 1);
        //GStLib.mray_gst_customPlayerBlitImage(m_Instance, _NativeTexturePtr, _TextureWidth, _TextureHeight);	// We pass Unity's width and height values of the texture
    }

    public bool GrabAudioFrame()
    {
        return GStLib.mray_gst_customPlayerGrabAudioFrame(m_Instance);
    }
    public int GetFrameSize()
    {
        return GStLib.mray_gst_customPlayerGetAudioFrameSize(m_Instance);
    }

    public bool CopyAudioFrame([In, Out]float[] data)
    {
        return GStLib.mray_gst_customPlayerCopyAudioFrame(m_Instance, data);
    }

    public int ChannelsCount()
    {
        return GStLib.mray_gst_customPlayerChannelsCount(m_Instance);
    }
    public int SampleRate()
    {
        return 32000;
    }
}
