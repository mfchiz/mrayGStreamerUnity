using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;	// For DllImport.
using System;

public abstract class IGstPlayer
{

    protected System.IntPtr m_Instance;

    public bool IsLoaded
    {
        get
        {
            return GStLib.mray_gst_PlayerIsLoaded(m_Instance);
        }
    }

    public bool IsPlaying
    {
        get
        {
        	if(GStLib.isUnloaded)
        		return false;
        	
        	return GStLib.mray_gst_PlayerIsPlaying(m_Instance);
        }
    }

    public System.IntPtr NativePtr
    {
        get
        {
            return m_Instance;
        }
    }

    public abstract int GetCaptureRate(int index);
    public virtual void Destroy()
    {
        if(!GStLib.isUnloaded)
        	GStLib.mray_gst_PlayerDestroy(m_Instance);
        
        m_Instance = IntPtr.Zero;
    }

    public void Play()
    {
    	if(!GStLib.isUnloaded)
        	GStLib.mray_gst_PlayerPlay(m_Instance);
    }

    public void Pause()
    {
        if(!GStLib.isUnloaded)
        	GStLib.mray_gst_PlayerPause(m_Instance);
    }

    public void Stop()
    {
    	if(!GStLib.isUnloaded)
        	GStLib.mray_gst_PlayerStop(m_Instance);
    }

    public void Close()
    {
    	if(!GStLib.isUnloaded)    
        	GStLib.mray_gst_PlayerClose(m_Instance);
    }

    public bool Seek(long pos)
    {
        return GStLib.mray_gst_PlayerSetPosition(m_Instance, pos);
    }

    public long GetPosition()
    {
        return GStLib.mray_gst_PlayerGetPosition(m_Instance);
    }


    public long GetDuration()
    {
        return GStLib.mray_gst_PlayerGetDuration(m_Instance);
    }

    public System.IntPtr GetLastImage(int index)
    {
        return GStLib.mray_gst_PlayerGetLastImage(m_Instance, index);
    }
    public ulong GetLastImageTimestamp(int index)
    {
        return GStLib.mray_gst_PlayerGetLastImageTimestamp(m_Instance, index);
    }

    public void SendRTPMetaToHost(int index, string host, int port)
    {
        GStLib.mray_gst_PlayerSendRTPMetaToHost(m_Instance, index, host, port);
    }
    public Vector4 RTPGetEyegaze(int index)
    {
        int x = 0, y = 0, w = 0, h = 0;
        GStLib.mray_gst_PlayerRTPGetEyeGazeData(m_Instance, index, ref x, ref y, ref w, ref h);
        return new Vector4(x, y, w, h);
    }
}







