﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;	// For DllImport.
using System;

public class GStreamerCore {
	

	static public float Time=0;

	static float GetEngineTime()
	{
		return Time;
	}

	static void CallBackFunction(string str)
	{
		Debug.Log("mrayGStreamer: " + str);
	}


	static IntPtr _nativeLibraryPtr;


	public static bool IsActive
	{
		get 	
		{
			if (GStLib.isUnloaded) 
				return false;
			
			return GStLib.mray_gstreamer_isActive();	
		}
	}
	public static void Ref()
	{
	/*	if (_nativeLibraryPtr == IntPtr.Zero) {
			_nativeLibraryPtr=NativeDLL.LoadLibrary(DllName);
			if(_nativeLibraryPtr==IntPtr.Zero)
			{
				Debug.LogError("Failed to load mrayGStreamer Library:"+DllName);
			}
		}
		if (!IsActive) {

			MyDelegate callback=new MyDelegate(CallBackFunction);
			IntPtr intptr_del=Marshal.GetFunctionPointerForDelegate(callback);
			mray_SetDebugFunction(intptr_del);
		}*/
		GStLib.mray_gstreamer_initialize();
	}
	public static void Unref()
	{
		if (IsActive && !GStLib.isUnloaded) 
			GStLib.mray_gstreamer_shutdown();
		/*
		if (!IsActive) {
			if(_nativeLibraryPtr!=IntPtr.Zero)
			{
			//	NativeDLL.UnloadModule(DllName);
				Debug.Log(NativeDLL.FreeLibrary(_nativeLibraryPtr)?
				          "mrayGStreamer Library successfuly Unloaded":
				          "Failed to unload mrayGStreamer Library");
			}
		}*/
	}


}
