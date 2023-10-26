using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;

public enum EPixelFormat
{
	EPixel_Unkown,

	EPixel_LUMINANCE8,
	EPixel_LUMINANCE16,

	EPixel_Alpha8,
	EPixel_Alpha4Luminance4,
	EPixel_Alpha8Luminance8,

	EPixel_R5G6B5,
	EPixel_R8G8B8,
	EPixel_R8G8B8A8,
	EPixel_X8R8G8B8,

	EPixel_B8G8R8,
	EPixel_B8G8R8A8,
	EPixel_X8B8G8R8,

	EPixel_Float16_R,
	EPixel_Float16_RGB,
	EPixel_Float16_RGBA,
	EPixel_Float16_GR,

	EPixel_Float32_R,
	EPixel_Float32_RGB,
	EPixel_Float32_RGBA,
	EPixel_Float32_GR,

	EPixel_Depth,
	EPixel_Stecil,


	EPixel_Short_RGBA,
	EPixel_Short_RGB,
	EPixel_Short_GR,

	EPixel_DXT1,
	EPixel_DXT2,
	EPixel_DXT3,
	EPixel_DXT4,
	EPixel_DXT5,

	EPixel_I420,
	EPixel_NV12,

	EPixelFormat_Count
};
  
  
//NOTE: The dlllibrary needs to be set to load on startup for this to work
public class GStLib : MonoBehaviour
{

	public static bool isUnloaded = false;
	public static bool isSetup = false;
	
#if UNITY_EDITOR

	const string libName = "libGStreamerUnityPlugin.so";

	// Handle to the C++ DLL
	public static IntPtr libraryHandle;

	public delegate System.IntPtr mray_gst_createNetworkAudioPlayerDelegate();
	public delegate void mray_gst_netAudioPlayerSetIPDelegate(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int audioPort, bool rtcp);
	public delegate bool mray_gst_netAudioPlayerCreateStreamDelegate(System.IntPtr p);
	public delegate void mray_gst_netAudioPlayerSetVolumeDelegate(System.IntPtr p, float v);
	public delegate uint mray_gst_netAudioPlayerGetAudioPortDelegate(System.IntPtr p);
	public delegate uint mray_gst_netAudioPlayerUseCustomOutputDelegate(System.IntPtr p,bool use);
	public delegate bool mray_gst_netAudioPlayerIsUsingCustomOutputDelegate(System.IntPtr p);
	public delegate bool mray_gst_netAudioPlayerGrabFrameDelegate(System.IntPtr p);
	public delegate int mray_gst_netAudioPlayerGetFrameSizeDelegate(System.IntPtr p);
	public delegate bool mray_gst_netAudioPlayerCopyAudioFrameDelegate(System.IntPtr p,[In,Out]float[] data);
	public delegate int mray_gst_netAudioPlayerChannelsCountDelegate(System.IntPtr p);
	public delegate void mray_gst_netAudioPlayerSetSampleRateDelegate(System.IntPtr p,int rate);
	public delegate int mray_gst_netAudioPlayerSampleRateDelegate(System.IntPtr p);
	public delegate float mray_gst_ProcessAudioPacketsDelegate([In,Out] float[] srcData,int startIndex,int channelIndex,int count,int stride,int srcChannels,[In,Out] float[] data,int length,int channels);
	public delegate System.IntPtr mray_gst_createCustomImageStreamerDelegate();
	public delegate void mray_gst_customImageStreamerSetPipelineDelegate(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip);
	public delegate int mray_gst_customImageStreamerSetGrabberDelegate(System.IntPtr p,System.IntPtr g);
	public delegate int mray_gst_customImageStreamerSetResolutionDelegate(System.IntPtr p,int width, int height, int fps);
	public delegate System.IntPtr mray_gst_createNetworkStreamerDelegate();
	public delegate void mray_gst_netStreamerSetIPDelegate(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int videoPort, bool rtcp);
	public delegate int mray_gst_netStreamerSetGrabberDelegate(System.IntPtr p,System.IntPtr g);
	public delegate int mray_gst_netStreamerSetBitRateDelegate(System.IntPtr p,int bitRate);
	public delegate int mray_gst_netStreamerSetResolutionDelegate(System.IntPtr p,int width, int height, int fps);
	public delegate System.IntPtr mray_gst_createCustomVideoPlayerDelegate();
	public delegate void mray_gst_customPlayerSetPipelineDelegate(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string pipeline);
	public delegate bool mray_gst_customPlayerCreateStreamDelegate(System.IntPtr p);
	public delegate void mray_gst_customPlayerSetLoopDelegate(System.IntPtr p,bool loop);
	public delegate bool mray_gst_customPlayerIsLoopDelegate(System.IntPtr p);
	public delegate void mray_gst_customPlayerGetFrameSizeDelegate(System.IntPtr p, ref int w, ref int h, ref int comp);
	public delegate bool mray_gst_customPlayerCopyFrameDelegate(System.IntPtr p, System.IntPtr target);
	public delegate bool mray_gst_customPlayerCropFrameDelegate(System.IntPtr p, System.IntPtr target, int x, int y, int width, int height);
	public delegate void mray_gst_customPlayerBlitImageDelegate(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight);
	public delegate System.IntPtr mray_gst_customPlayerBlitImageNativeGLCallDelegate(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight);
	public delegate int mray_gst_customPlayerFrameCountDelegate(System.IntPtr p);
	public delegate bool mray_gst_customPlayerGrabAudioFrameDelegate(System.IntPtr p);
	public delegate int mray_gst_customPlayerGetAudioFrameSizeDelegate(System.IntPtr p);    
	public delegate int mray_gst_customPlayerChannelsCountDelegate(System.IntPtr p);
	public delegate bool mray_gst_customPlayerCopyAudioFrameDelegate(System.IntPtr p, [In, Out]float[] data);
	public delegate void mray_gst_StreamerDestroyDelegate(System.IntPtr p);
	public delegate bool mray_gst_StreamerCreateStreamDelegate(System.IntPtr p);
	public delegate void mray_gst_StreamerStreamDelegate(System.IntPtr p);
	public delegate void mray_gst_StreamerPauseDelegate(System.IntPtr p);
	public delegate void mray_gst_StreamerResumeDelegate(System.IntPtr p);
	public delegate void mray_gst_StreamerStopDelegate(System.IntPtr p);
	public delegate bool mray_gst_StreamerIsStreamingDelegate(System.IntPtr p);
	public delegate void mray_gst_StreamerCloseDelegate(System.IntPtr p);
	public delegate System.IntPtr mray_gst_createNetworkMultiplePlayerDelegate();
	public delegate void mray_gst_multiNetPlayerSetIPDelegate(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int baseVideoPort,int count, bool rtcp);
	public delegate bool mray_gst_multiNetPlayerCreateStreamDelegate(System.IntPtr p);
	public delegate void mray_gst_multiNetPlayerGetFrameSizeDelegate(System.IntPtr p, ref int w, ref int h, ref int components);
	public delegate bool mray_gst_multiNetPlayerGrabFrameDelegate(System.IntPtr p, ref int w, ref int h,int index);
	public delegate void mray_gst_multiNetPlayerBlitImageDelegate(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight, int index);
	public delegate void mray_gst_multiNetPlayerCopyDataDelegate(System.IntPtr p, System.IntPtr _data, int index);
	public delegate int mray_gst_multiNetPlayerFrameCountDelegate(System.IntPtr p,int index);
	public delegate uint mray_gst_multiNetPlayerGetVideoPortDelegate(System.IntPtr p,int index);
	public delegate void mray_gst_multiNetPlayerSetFormatDelegate(System.IntPtr p,int fmt);
	public delegate int mray_gst_multiNetPlayerGetFormatDelegate(System.IntPtr p);
	public delegate ulong mray_gst_multiNetPlayerGetNetworkUsageDelegate(System.IntPtr p);
	public delegate System.IntPtr mray_gst_multiNetPlayerBlitImageNativeGLCallDelegate(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight, int index);
	public delegate void mray_gst_multiNetPlayerSetDecoderTypeDelegate(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string decoder);
	public delegate System.IntPtr mray_gst_createUnityImageGrabberDelegate();
	public delegate void mray_gst_UnityImageGrabberSetDataDelegate(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight,int Format);
	public delegate void mray_gst_UnityImageGrabberDestroyDelegate(System.IntPtr p);
	public delegate System.IntPtr mray_createImageDataDelegate(int width,int height,EPixelFormat format);
	public delegate void mray_resizeImageDataDelegate(System.IntPtr ifo,int width,int height,EPixelFormat format);
	public delegate void mray_getImageDataInfoDelegate(System.IntPtr ifo,ref int width,ref int height,ref EPixelFormat  format);
	public delegate void mray_deleteImageDataDelegate(System.IntPtr ifo);
	public delegate void mray_BlitImageDataInfoDelegate(System.IntPtr ifo,System.IntPtr TextureNativePtr);
	public delegate System.IntPtr mray_BlitImageNativeGLCallDelegate(System.IntPtr p, System.IntPtr _TextureNativePtr);
	public delegate System.IntPtr mray_FlipImageDataDelegate(System.IntPtr p, bool horizontal,bool vertical);
	public delegate void mray_cloneImageDataDelegate(System.IntPtr p, System.IntPtr dst);
	public delegate void mray_copyCroppedImageDataDelegate(System.IntPtr ifo, System.IntPtr dst, int x, int y, int width, int height, bool clamp);
	public delegate System.IntPtr mray_getImageDataPtrDelegate(System.IntPtr p);
	public delegate System.IntPtr mray_gst_createAudioNetworkStreamerDelegate();
	public delegate void mray_gst_audioStreamerAddClientDelegate(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int port);
	public delegate int mray_gst_audioStreamerGetClientCountDelegate(System.IntPtr p);
	public delegate int mray_gst_audioStreamerRemoveClientDelegate(System.IntPtr p,int i);
	public delegate void mray_gst_audioStreamerClearClientsDelegate(System.IntPtr p);
	public delegate string mray_gst_audioStreamerGetClientDelegate(System.IntPtr p,int i);
	public delegate void mray_gst_audioStreamerSetClientVolumeDelegate(System.IntPtr p,int i,float volume);
	public delegate void mray_gst_audioStreamerSetChannelsDelegate(System.IntPtr p,int c);
	public delegate void mray_gst_PlayerDestroyDelegate(System.IntPtr p);
	public delegate void mray_gst_PlayerPlayDelegate(System.IntPtr p);
	public delegate void mray_gst_PlayerPauseDelegate(System.IntPtr p);
	public delegate void mray_gst_PlayerStopDelegate(System.IntPtr p);
	public delegate bool mray_gst_PlayerIsLoadedDelegate(System.IntPtr p);
	public delegate bool mray_gst_PlayerIsPlayingDelegate(System.IntPtr p);
	public delegate void mray_gst_PlayerCloseDelegate(System.IntPtr p);
	public delegate bool mray_gst_PlayerSetPositionDelegate(System.IntPtr p, long pos);
	public delegate long mray_gst_PlayerGetPositionDelegate(System.IntPtr p);
	public delegate long mray_gst_PlayerGetDurationDelegate(System.IntPtr p);
	public delegate System.IntPtr mray_gst_PlayerGetLastImageDelegate(System.IntPtr p, int index);
	public delegate ulong mray_gst_PlayerGetLastImageTimestampDelegate(System.IntPtr p, int index);
	public delegate void mray_gst_PlayerSendRTPMetaToHostDelegate(System.IntPtr p, int index, [MarshalAs(UnmanagedType.LPStr)]string ip, int port);
	public delegate void mray_gst_PlayerRTPGetEyeGazeDataDelegate(System.IntPtr p, int index, ref int x, ref int y, ref int w, ref int h);   
	public delegate bool mray_gst_playerGrabFrameDelegate(System.IntPtr p, ref int w, ref int h, ref int c, int index);
	public delegate bool mray_gstreamer_initializeDelegate(  );
	public delegate void mray_gstreamer_shutdownDelegate(  );
	public delegate bool mray_gstreamer_isActiveDelegate(  );
	public delegate void mray_SetDebugFunctionDelegate( IntPtr str );
	public delegate System.IntPtr mray_gst_createNetworkPlayerDelegate();
	public delegate void mray_gst_netPlayerSetIPDelegate(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int videoPort, bool rtcp);
	public delegate bool mray_gst_netPlayerCreateStreamDelegate(System.IntPtr p);
	public delegate void mray_gst_netPlayerGetFrameSizeDelegate(System.IntPtr p, ref int w, ref int h, ref int components);
	public delegate bool mray_gst_netPlayerGrabFrameDelegate(System.IntPtr p, ref int w, ref int h);
	public delegate void mray_gst_netPlayerBlitImageDelegate(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight);
	public delegate int mray_gst_netPlayerFrameCountDelegate(System.IntPtr p);
	public delegate System.IntPtr CreateTextureWrapperDelegate();
	public delegate void DeleteTextureWrapperDelegate(System.IntPtr w);
	public delegate bool SendTextureDelegate(System.IntPtr w,System.IntPtr textureID);
	
	
	
	public static mray_gst_createNetworkAudioPlayerDelegate mray_gst_createNetworkAudioPlayer;
	public static mray_gst_netAudioPlayerSetIPDelegate mray_gst_netAudioPlayerSetIP;
	public static mray_gst_netAudioPlayerCreateStreamDelegate mray_gst_netAudioPlayerCreateStream;
	public static mray_gst_netAudioPlayerSetVolumeDelegate mray_gst_netAudioPlayerSetVolume;
	public static mray_gst_netAudioPlayerGetAudioPortDelegate mray_gst_netAudioPlayerGetAudioPort;
	public static mray_gst_netAudioPlayerUseCustomOutputDelegate mray_gst_netAudioPlayerUseCustomOutput;
	public static mray_gst_netAudioPlayerIsUsingCustomOutputDelegate mray_gst_netAudioPlayerIsUsingCustomOutput;
	public static mray_gst_netAudioPlayerGrabFrameDelegate mray_gst_netAudioPlayerGrabFrame;
	public static mray_gst_netAudioPlayerGetFrameSizeDelegate mray_gst_netAudioPlayerGetFrameSize;
	public static mray_gst_netAudioPlayerCopyAudioFrameDelegate mray_gst_netAudioPlayerCopyAudioFrame;
	public static mray_gst_netAudioPlayerChannelsCountDelegate mray_gst_netAudioPlayerChannelsCount;
	public static mray_gst_netAudioPlayerSetSampleRateDelegate mray_gst_netAudioPlayerSetSampleRate;
	public static mray_gst_netAudioPlayerSampleRateDelegate mray_gst_netAudioPlayerSampleRate;
	public static mray_gst_ProcessAudioPacketsDelegate mray_gst_ProcessAudioPackets;
	public static mray_gst_createCustomImageStreamerDelegate mray_gst_createCustomImageStreamer;
	public static mray_gst_customImageStreamerSetPipelineDelegate mray_gst_customImageStreamerSetPipeline;
	public static mray_gst_customImageStreamerSetGrabberDelegate mray_gst_customImageStreamerSetGrabber;
	public static mray_gst_customImageStreamerSetResolutionDelegate mray_gst_customImageStreamerSetResolution;
	public static mray_gst_createNetworkStreamerDelegate mray_gst_createNetworkStreamer;
	public static mray_gst_netStreamerSetIPDelegate mray_gst_netStreamerSetIP;
	public static mray_gst_netStreamerSetGrabberDelegate mray_gst_netStreamerSetGrabber;
	public static mray_gst_netStreamerSetBitRateDelegate mray_gst_netStreamerSetBitRate;
	public static mray_gst_netStreamerSetResolutionDelegate mray_gst_netStreamerSetResolution;
	public static mray_gst_createCustomVideoPlayerDelegate mray_gst_createCustomVideoPlayer;
	public static mray_gst_customPlayerSetPipelineDelegate mray_gst_customPlayerSetPipeline;
	public static mray_gst_customPlayerCreateStreamDelegate mray_gst_customPlayerCreateStream;
	public static mray_gst_customPlayerSetLoopDelegate mray_gst_customPlayerSetLoop;
	public static mray_gst_customPlayerIsLoopDelegate mray_gst_customPlayerIsLoop;
	public static mray_gst_customPlayerGetFrameSizeDelegate mray_gst_customPlayerGetFrameSize;
	public static mray_gst_customPlayerCopyFrameDelegate mray_gst_customPlayerCopyFrame;
	public static mray_gst_customPlayerCropFrameDelegate mray_gst_customPlayerCropFrame;
	public static mray_gst_customPlayerBlitImageDelegate mray_gst_customPlayerBlitImage;
	public static mray_gst_customPlayerBlitImageNativeGLCallDelegate mray_gst_customPlayerBlitImageNativeGLCall;
	public static mray_gst_customPlayerFrameCountDelegate mray_gst_customPlayerFrameCount;
	public static mray_gst_customPlayerGrabAudioFrameDelegate mray_gst_customPlayerGrabAudioFrame;
	public static mray_gst_customPlayerGetAudioFrameSizeDelegate mray_gst_customPlayerGetAudioFrameSize;
	public static mray_gst_customPlayerChannelsCountDelegate mray_gst_customPlayerChannelsCount;			
	public static mray_gst_customPlayerCopyAudioFrameDelegate mray_gst_customPlayerCopyAudioFrame;
	public static mray_gst_StreamerDestroyDelegate mray_gst_StreamerDestroy;
	public static mray_gst_StreamerCreateStreamDelegate mray_gst_StreamerCreateStream;
	public static mray_gst_StreamerStreamDelegate mray_gst_StreamerStream;
	public static mray_gst_StreamerPauseDelegate mray_gst_StreamerPause;
	public static mray_gst_StreamerResumeDelegate mray_gst_StreamerResume;
	public static mray_gst_StreamerStopDelegate mray_gst_StreamerStop;
	public static mray_gst_StreamerIsStreamingDelegate mray_gst_StreamerIsStreaming;
	public static mray_gst_StreamerCloseDelegate mray_gst_StreamerClose;
	public static mray_gst_createNetworkMultiplePlayerDelegate mray_gst_createNetworkMultiplePlayer;
	public static mray_gst_multiNetPlayerSetIPDelegate mray_gst_multiNetPlayerSetIP;
	public static mray_gst_multiNetPlayerCreateStreamDelegate mray_gst_multiNetPlayerCreateStream;
	public static mray_gst_multiNetPlayerGetFrameSizeDelegate mray_gst_multiNetPlayerGetFrameSize;
	public static mray_gst_multiNetPlayerGrabFrameDelegate mray_gst_multiNetPlayerGrabFrame;
	public static mray_gst_multiNetPlayerBlitImageDelegate mray_gst_multiNetPlayerBlitImage;
	public static mray_gst_multiNetPlayerCopyDataDelegate mray_gst_multiNetPlayerCopyData;
	public static mray_gst_multiNetPlayerFrameCountDelegate mray_gst_multiNetPlayerFrameCount;
	public static mray_gst_multiNetPlayerGetVideoPortDelegate mray_gst_multiNetPlayerGetVideoPort;
	public static mray_gst_multiNetPlayerSetFormatDelegate mray_gst_multiNetPlayerSetFormat;
	public static mray_gst_multiNetPlayerGetFormatDelegate mray_gst_multiNetPlayerGetFormat;
	public static mray_gst_multiNetPlayerGetNetworkUsageDelegate mray_gst_multiNetPlayerGetNetworkUsage;
	public static mray_gst_multiNetPlayerBlitImageNativeGLCallDelegate mray_gst_multiNetPlayerBlitImageNativeGLCall;
	public static mray_gst_multiNetPlayerSetDecoderTypeDelegate mray_gst_multiNetPlayerSetDecoderType;
	public static mray_gst_createUnityImageGrabberDelegate mray_gst_createUnityImageGrabber;
	public static mray_gst_UnityImageGrabberSetDataDelegate mray_gst_UnityImageGrabberSetData;
	public static mray_gst_UnityImageGrabberDestroyDelegate mray_gst_UnityImageGrabberDestroy;
	public static mray_createImageDataDelegate mray_createImageData;
	public static mray_resizeImageDataDelegate mray_resizeImageData;
	public static mray_getImageDataInfoDelegate mray_getImageDataInfo;
	public static mray_deleteImageDataDelegate mray_deleteImageData;
	public static mray_BlitImageDataInfoDelegate mray_BlitImageDataInfo;
	public static mray_BlitImageNativeGLCallDelegate mray_BlitImageNativeGLCall;
	public static mray_FlipImageDataDelegate mray_FlipImageData;
	public static mray_cloneImageDataDelegate mray_cloneImageData;
	public static mray_copyCroppedImageDataDelegate mray_copyCroppedImageData;
	public static mray_getImageDataPtrDelegate mray_getImageDataPtr;
	public static mray_gst_createAudioNetworkStreamerDelegate mray_gst_createAudioNetworkStreamer;
	public static mray_gst_audioStreamerAddClientDelegate mray_gst_audioStreamerAddClient;
	public static mray_gst_audioStreamerGetClientCountDelegate mray_gst_audioStreamerGetClientCount;
	public static mray_gst_audioStreamerRemoveClientDelegate mray_gst_audioStreamerRemoveClient;
	public static mray_gst_audioStreamerClearClientsDelegate mray_gst_audioStreamerClearClients;
	public static mray_gst_audioStreamerGetClientDelegate mray_gst_audioStreamerGetClient;
	public static mray_gst_audioStreamerSetClientVolumeDelegate mray_gst_audioStreamerSetClientVolume;
	public static mray_gst_audioStreamerSetChannelsDelegate mray_gst_audioStreamerSetChannels;
	public static mray_gst_PlayerDestroyDelegate mray_gst_PlayerDestroy;
	public static mray_gst_PlayerPlayDelegate mray_gst_PlayerPlay;
	public static mray_gst_PlayerPauseDelegate mray_gst_PlayerPause;
	public static mray_gst_PlayerStopDelegate mray_gst_PlayerStop;
	public static mray_gst_PlayerIsLoadedDelegate mray_gst_PlayerIsLoaded;
	public static mray_gst_PlayerIsPlayingDelegate mray_gst_PlayerIsPlaying;
	public static mray_gst_PlayerCloseDelegate mray_gst_PlayerClose;
	public static mray_gst_PlayerSetPositionDelegate mray_gst_PlayerSetPosition;
	public static mray_gst_PlayerGetPositionDelegate mray_gst_PlayerGetPosition;
	public static mray_gst_PlayerGetDurationDelegate mray_gst_PlayerGetDuration;
	public static mray_gst_PlayerGetLastImageDelegate mray_gst_PlayerGetLastImage;
	public static mray_gst_PlayerGetLastImageTimestampDelegate mray_gst_PlayerGetLastImageTimestamp;
	public static mray_gst_PlayerSendRTPMetaToHostDelegate mray_gst_PlayerSendRTPMetaToHost;
	public static mray_gst_PlayerRTPGetEyeGazeDataDelegate mray_gst_PlayerRTPGetEyeGazeData;
	public static mray_gst_playerGrabFrameDelegate mray_gst_playerGrabFrame;
	public static mray_gstreamer_initializeDelegate mray_gstreamer_initialize;
	public static mray_gstreamer_shutdownDelegate mray_gstreamer_shutdown;
	public static mray_gstreamer_isActiveDelegate mray_gstreamer_isActive;
	public static mray_SetDebugFunctionDelegate mray_SetDebugFunction;
	public static mray_gst_createNetworkPlayerDelegate mray_gst_createNetworkPlayer;
	public static mray_gst_netPlayerSetIPDelegate mray_gst_netPlayerSetIP;
	public static mray_gst_netPlayerCreateStreamDelegate mray_gst_netPlayerCreateStream;
	public static mray_gst_netPlayerGetFrameSizeDelegate mray_gst_netPlayerGetFrameSize;
	public static mray_gst_netPlayerGrabFrameDelegate mray_gst_netPlayerGrabFrame;
	public static mray_gst_netPlayerBlitImageDelegate mray_gst_netPlayerBlitImage;
	public static mray_gst_netPlayerFrameCountDelegate mray_gst_netPlayerFrameCount;
	public static CreateTextureWrapperDelegate CreateTextureWrapper;
	public static DeleteTextureWrapperDelegate DeleteTextureWrapper;
	public static SendTextureDelegate SendTexture;
 	
#else 	

  	const string libName = "GStreamerUnityPlugin"; 
		    
#endif
		
		
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
 
		[DllImport("__Internal")]
		public static extern IntPtr dlopen(
			string path,
			int flag);
	 
		[DllImport("__Internal")]
		public static extern IntPtr dlsym(
			IntPtr handle,
			string symbolName);
	 
		[DllImport("__Internal")]
		public static extern int dlclose(
			IntPtr handle);
	 
		public static IntPtr OpenLibrary(string path)
		{
			//was 0 as flag use 1 instead
			IntPtr handle = dlopen(path, 1);
			
			if (handle == IntPtr.Zero)
			{
				throw new Exception("Couldn't open native library: " + path);
			}
			return handle;
		}
	 
		public static void CloseLibrary(IntPtr libraryHandle)
		{
			dlclose(libraryHandle);
		}
	 
		public static T GetDelegate<T>(
			IntPtr libraryHandle,
			string functionName) where T : class
		{
			IntPtr symbol = dlsym(libraryHandle, functionName);
			if (symbol == IntPtr.Zero)
			{
				throw new Exception("Couldn't get function: " + functionName);
			}
			return Marshal.GetDelegateForFunctionPointer(
				symbol,
				typeof(T)) as T;
		}
	 
	 
	#elif UNITY_EDITOR_WIN
	 
		[DllImport("kernel32")]
		public static extern IntPtr LoadLibrary(
			string path);
	 
		[DllImport("kernel32")]
		public static extern IntPtr GetProcAddress(
			IntPtr libraryHandle,
			string symbolName);
	 
		[DllImport("kernel32")]
		public static extern bool FreeLibrary(
			IntPtr libraryHandle);
	 
		public static IntPtr OpenLibrary(string path)
		{
			IntPtr handle = LoadLibrary(path);
			if (handle == IntPtr.Zero)
			{
				throw new Exception("Couldn't open native library: " + path);
			}
			return handle;
		}
	 
		public static void CloseLibrary(IntPtr libraryHandle)
		{
			FreeLibrary(libraryHandle);
		}
	 
		public static T GetDelegate<T>(
			IntPtr libraryHandle,
			string functionName) where T : class
		{
			IntPtr symbol = GetProcAddress(libraryHandle, functionName);
			if (symbol == IntPtr.Zero)
			{
				throw new Exception("Couldn't get function: " + functionName);
			}
			return Marshal.GetDelegateForFunctionPointer(
				symbol,
				typeof(T)) as T;
		}
	 
 
#else
		
	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_createNetworkAudioPlayer();

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_netAudioPlayerSetIP(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int audioPort, bool rtcp);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_netAudioPlayerCreateStream(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_netAudioPlayerSetVolume(System.IntPtr p, float v);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public uint mray_gst_netAudioPlayerGetAudioPort(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public uint mray_gst_netAudioPlayerUseCustomOutput(System.IntPtr p,bool use);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_netAudioPlayerIsUsingCustomOutput(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_netAudioPlayerGrabFrame(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_netAudioPlayerGetFrameSize(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_netAudioPlayerCopyAudioFrame(System.IntPtr p,[In,Out]float[] data);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_netAudioPlayerChannelsCount(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_netAudioPlayerSetSampleRate(System.IntPtr p,int rate);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_netAudioPlayerSampleRate(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public float mray_gst_ProcessAudioPackets([In,Out] float[] srcData,int startIndex,int channelIndex,int count,int stride,int srcChannels,[In,Out] float[] data,int length,int channels);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_createCustomImageStreamer();

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_customImageStreamerSetPipeline(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_customImageStreamerSetGrabber(System.IntPtr p,System.IntPtr g);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_customImageStreamerSetResolution(System.IntPtr p,int width, int height, int fps);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_createNetworkStreamer();

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_netStreamerSetIP(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int videoPort, bool rtcp);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_netStreamerSetGrabber(System.IntPtr p,System.IntPtr g);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_netStreamerSetBitRate(System.IntPtr p,int bitRate);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_netStreamerSetResolution(System.IntPtr p,int width, int height, int fps);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_createCustomVideoPlayer();

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_customPlayerSetPipeline(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string pipeline);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_customPlayerCreateStream(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_customPlayerSetLoop(System.IntPtr p,bool loop);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_customPlayerIsLoop(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_customPlayerGetFrameSize(System.IntPtr p, ref int w, ref int h, ref int comp);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_customPlayerCopyFrame(System.IntPtr p, System.IntPtr target);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_customPlayerCropFrame(System.IntPtr p, System.IntPtr target, int x, int y, int width, int height);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_customPlayerBlitImage(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_customPlayerBlitImageNativeGLCall(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_customPlayerFrameCount(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_customPlayerGrabAudioFrame(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_customPlayerGetAudioFrameSize(System.IntPtr p);
	
	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_customPlayerChannelsCount(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_customPlayerCopyAudioFrame(System.IntPtr p, [In, Out]float[] data);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_StreamerDestroy(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_StreamerCreateStream(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_StreamerStream(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_StreamerPause(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_StreamerResume(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_StreamerStop(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_StreamerIsStreaming(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_StreamerClose(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_createNetworkMultiplePlayer();

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_multiNetPlayerSetIP(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int baseVideoPort,int count, bool rtcp);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_multiNetPlayerCreateStream(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_multiNetPlayerGetFrameSize(System.IntPtr p, ref int w, ref int h, ref int components);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_multiNetPlayerGrabFrame(System.IntPtr p, ref int w, ref int h,int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_multiNetPlayerBlitImage(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight, int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_multiNetPlayerCopyData(System.IntPtr p, System.IntPtr _data, int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_multiNetPlayerFrameCount(System.IntPtr p,int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public uint mray_gst_multiNetPlayerGetVideoPort(System.IntPtr p,int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_multiNetPlayerSetFormat(System.IntPtr p,int fmt);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_multiNetPlayerGetFormat(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public ulong mray_gst_multiNetPlayerGetNetworkUsage(System.IntPtr p);
	
	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	static extern public System.IntPtr mray_gst_multiNetPlayerBlitImageNativeGLCall(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight, int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_multiNetPlayerSetDecoderType(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string decoder);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_createUnityImageGrabber();

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_UnityImageGrabberSetData(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight,int Format);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_UnityImageGrabberDestroy(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_createImageData(int width,int height,EPixelFormat format);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_resizeImageData(System.IntPtr ifo,int width,int height,EPixelFormat format);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_getImageDataInfo(System.IntPtr ifo,ref int width,ref int height,ref EPixelFormat  format);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_deleteImageData(System.IntPtr ifo);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_BlitImageDataInfo(System.IntPtr ifo,System.IntPtr TextureNativePtr);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_BlitImageNativeGLCall(System.IntPtr p, System.IntPtr _TextureNativePtr);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_FlipImageData(System.IntPtr p, bool horizontal,bool vertical);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_cloneImageData(System.IntPtr p, System.IntPtr dst);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_copyCroppedImageData (System.IntPtr ifo, System.IntPtr dst, int x, int y, int width, int height, bool clamp);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_getImageDataPtr(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_createAudioNetworkStreamer();

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_audioStreamerAddClient(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int port);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_audioStreamerGetClientCount(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_audioStreamerRemoveClient(System.IntPtr p,int i);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_audioStreamerClearClients(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public string mray_gst_audioStreamerGetClient(System.IntPtr p,int i);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_audioStreamerSetClientVolume(System.IntPtr p,int i,float volume);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_audioStreamerSetChannels(System.IntPtr p,int c);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_PlayerDestroy(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_PlayerPlay(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_PlayerPause(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_PlayerStop(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_PlayerIsLoaded(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_PlayerIsPlaying(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_PlayerClose(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_PlayerSetPosition(System.IntPtr p, long pos);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public long mray_gst_PlayerGetPosition(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public long mray_gst_PlayerGetDuration(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_PlayerGetLastImage(System.IntPtr p, int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public ulong mray_gst_PlayerGetLastImageTimestamp(System.IntPtr p, int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_PlayerSendRTPMetaToHost(System.IntPtr p, int index, [MarshalAs(UnmanagedType.LPStr)]string ip, int port);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_PlayerRTPGetEyeGazeData(System.IntPtr p, int index, ref int x, ref int y, ref int w, ref int h);
	
	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_playerGrabFrame(System.IntPtr p, ref int w, ref int h, ref int c, int index);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gstreamer_initialize(  );

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gstreamer_shutdown(  );

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gstreamer_isActive(  );

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_SetDebugFunction( IntPtr str );

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public System.IntPtr mray_gst_createNetworkPlayer();

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_netPlayerSetIP(System.IntPtr p, [MarshalAs(UnmanagedType.LPStr)]string ip, int videoPort, bool rtcp);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_netPlayerCreateStream(System.IntPtr p);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_netPlayerGetFrameSize(System.IntPtr p, ref int w, ref int h, ref int components);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public bool mray_gst_netPlayerGrabFrame(System.IntPtr p, ref int w, ref int h);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public void mray_gst_netPlayerBlitImage(System.IntPtr p, System.IntPtr _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight);

	[DllImport(libName, CallingConvention = CallingConvention.Cdecl)]
	extern static public int mray_gst_netPlayerFrameCount(System.IntPtr p);

	[DllImport(libName,CallingConvention=CallingConvention.Cdecl)]
	extern static public System.IntPtr CreateTextureWrapper();

	[DllImport(libName,CallingConvention=CallingConvention.Cdecl)]
	extern static public void DeleteTextureWrapper(System.IntPtr w);

	[DllImport(libName,CallingConvention=CallingConvention.Cdecl)]
	extern static public bool SendTexture(System.IntPtr w,System.IntPtr textureID);
  
#endif


	public static void Setup()
	{
	#if UNITY_EDITOR
		if(!isSetup){

			string libraryPath = "";
			
 			var assets = AssetDatabase.FindAssets("libGStreamerUnityPlugin");
 			
            foreach (var asset in assets)
            {
                libraryPath=AssetDatabase.GUIDToAssetPath(asset);
              	UnityEngine.Debug.Log("GStLib found librarypath in Unity editor " + libraryPath);

            }

			libraryHandle = OpenLibrary(libraryPath);
		
	/*		
			mray_gst_createNetworkAudioPlayer =
			 GetDelegate<mray_gst_createNetworkAudioPlayerDelegate>(libraryHandle, "mray_gst_createNetworkAudioPlayer");

			mray_gst_netAudioPlayerSetIP =
			 GetDelegate<mray_gst_netAudioPlayerSetIPDelegate>(libraryHandle, "mray_gst_netAudioPlayerSetIP");

			mray_gst_netAudioPlayerCreateStream =
			 GetDelegate<mray_gst_netAudioPlayerCreateStreamDelegate>(libraryHandle, "mray_gst_netAudioPlayerCreateStream");

			mray_gst_netAudioPlayerSetVolume =
			 GetDelegate<mray_gst_netAudioPlayerSetVolumeDelegate>(libraryHandle, "mray_gst_netAudioPlayerSetVolume");

			mray_gst_netAudioPlayerGetAudioPort =
			 GetDelegate<mray_gst_netAudioPlayerGetAudioPortDelegate>(libraryHandle, "mray_gst_netAudioPlayerGetAudioPort");

			mray_gst_netAudioPlayerUseCustomOutput =
			 GetDelegate<mray_gst_netAudioPlayerUseCustomOutputDelegate>(libraryHandle, "mray_gst_netAudioPlayerUseCustomOutput");

			mray_gst_netAudioPlayerIsUsingCustomOutput =
			 GetDelegate<mray_gst_netAudioPlayerIsUsingCustomOutputDelegate>(libraryHandle, "mray_gst_netAudioPlayerIsUsingCustomOutput");

			mray_gst_netAudioPlayerGrabFrame =
			 GetDelegate<mray_gst_netAudioPlayerGrabFrameDelegate>(libraryHandle, "mray_gst_netAudioPlayerGrabFrame");

			mray_gst_netAudioPlayerGetFrameSize =
			 GetDelegate<mray_gst_netAudioPlayerGetFrameSizeDelegate>(libraryHandle, "mray_gst_netAudioPlayerGetFrameSize");

			mray_gst_netAudioPlayerCopyAudioFrame =
			 GetDelegate<mray_gst_netAudioPlayerCopyAudioFrameDelegate>(libraryHandle, "mray_gst_netAudioPlayerCopyAudioFrame");

			mray_gst_netAudioPlayerChannelsCount =
			 GetDelegate<mray_gst_netAudioPlayerChannelsCountDelegate>(libraryHandle, "mray_gst_netAudioPlayerChannelsCount");

			mray_gst_netAudioPlayerSetSampleRate =
			 GetDelegate<mray_gst_netAudioPlayerSetSampleRateDelegate>(libraryHandle, "mray_gst_netAudioPlayerSetSampleRate");

			mray_gst_netAudioPlayerSampleRate =
			 GetDelegate<mray_gst_netAudioPlayerSampleRateDelegate>(libraryHandle, "mray_gst_netAudioPlayerSampleRate");

			mray_gst_ProcessAudioPackets =
			 GetDelegate<mray_gst_ProcessAudioPacketsDelegate>(libraryHandle, "mray_gst_ProcessAudioPackets");
			 
			mray_gst_createNetworkStreamer =
			 GetDelegate<mray_gst_createNetworkStreamerDelegate>(libraryHandle, "mray_gst_createNetworkStreamer");

			mray_gst_netStreamerSetIP =
			 GetDelegate<mray_gst_netStreamerSetIPDelegate>(libraryHandle, "mray_gst_netStreamerSetIP");

			mray_gst_netStreamerSetGrabber =
			 GetDelegate<mray_gst_netStreamerSetGrabberDelegate>(libraryHandle, "mray_gst_netStreamerSetGrabber");
			 
			mray_gst_netStreamerSetBitRate =
			 GetDelegate<mray_gst_netStreamerSetBitRateDelegate>(libraryHandle, "mray_gst_netStreamerSetBitRate");

			mray_gst_netStreamerSetResolution =
			 GetDelegate<mray_gst_netStreamerSetResolutionDelegate>(libraryHandle, "mray_gst_netStreamerSetResolution");

			mray_gst_createNetworkMultiplePlayer =
			 GetDelegate<mray_gst_createNetworkMultiplePlayerDelegate>(libraryHandle, "mray_gst_createNetworkMultiplePlayer");

			mray_gst_multiNetPlayerSetIP =
			 GetDelegate<mray_gst_multiNetPlayerSetIPDelegate>(libraryHandle, "mray_gst_multiNetPlayerSetIP");

			mray_gst_multiNetPlayerCreateStream =
			 GetDelegate<mray_gst_multiNetPlayerCreateStreamDelegate>(libraryHandle, "mray_gst_multiNetPlayerCreateStream");

			mray_gst_multiNetPlayerGetFrameSize =
			 GetDelegate<mray_gst_multiNetPlayerGetFrameSizeDelegate>(libraryHandle, "mray_gst_multiNetPlayerGetFrameSize");

			mray_gst_multiNetPlayerGrabFrame =
			 GetDelegate<mray_gst_multiNetPlayerGrabFrameDelegate>(libraryHandle, "mray_gst_multiNetPlayerGrabFrame");

			mray_gst_multiNetPlayerBlitImage =
			 GetDelegate<mray_gst_multiNetPlayerBlitImageDelegate>(libraryHandle, "mray_gst_multiNetPlayerBlitImage");

			mray_gst_multiNetPlayerCopyData =
			 GetDelegate<mray_gst_multiNetPlayerCopyDataDelegate>(libraryHandle, "mray_gst_multiNetPlayerCopyData");
			 
			 mray_gst_createAudioNetworkStreamer =
			 GetDelegate<mray_gst_createAudioNetworkStreamerDelegate>(libraryHandle, "mray_gst_createAudioNetworkStreamer");

			mray_gst_audioStreamerAddClient =
			 GetDelegate<mray_gst_audioStreamerAddClientDelegate>(libraryHandle, "mray_gst_audioStreamerAddClient");

			mray_gst_audioStreamerGetClientCount =
			 GetDelegate<mray_gst_audioStreamerGetClientCountDelegate>(libraryHandle, "mray_gst_audioStreamerGetClientCount");

			mray_gst_audioStreamerRemoveClient =
			 GetDelegate<mray_gst_audioStreamerRemoveClientDelegate>(libraryHandle, "mray_gst_audioStreamerRemoveClient");

			mray_gst_audioStreamerClearClients =
			 GetDelegate<mray_gst_audioStreamerClearClientsDelegate>(libraryHandle, "mray_gst_audioStreamerClearClients");

			mray_gst_audioStreamerGetClient =
			 GetDelegate<mray_gst_audioStreamerGetClientDelegate>(libraryHandle, "mray_gst_audioStreamerGetClient");

			mray_gst_audioStreamerSetClientVolume =
			 GetDelegate<mray_gst_audioStreamerSetClientVolumeDelegate>(libraryHandle, "mray_gst_audioStreamerSetClientVolume");

			mray_gst_audioStreamerSetChannels =
			 GetDelegate<mray_gst_audioStreamerSetChannelsDelegate>(libraryHandle, "mray_gst_audioStreamerSetChannels");

			mray_gst_createNetworkPlayer =
			 GetDelegate<mray_gst_createNetworkPlayerDelegate>(libraryHandle, "mray_gst_createNetworkPlayer");

			mray_gst_netPlayerSetIP =
			 GetDelegate<mray_gst_netPlayerSetIPDelegate>(libraryHandle, "mray_gst_netPlayerSetIP");

			mray_gst_netPlayerCreateStream =
			 GetDelegate<mray_gst_netPlayerCreateStreamDelegate>(libraryHandle, "mray_gst_netPlayerCreateStream");

			mray_gst_netPlayerGetFrameSize =
			 GetDelegate<mray_gst_netPlayerGetFrameSizeDelegate>(libraryHandle, "mray_gst_netPlayerGetFrameSize");

			mray_gst_netPlayerGrabFrame =
			 GetDelegate<mray_gst_netPlayerGrabFrameDelegate>(libraryHandle, "mray_gst_netPlayerGrabFrame");

			mray_gst_netPlayerBlitImage =
			 GetDelegate<mray_gst_netPlayerBlitImageDelegate>(libraryHandle, "mray_gst_netPlayerBlitImage");

			mray_gst_netPlayerFrameCount =
			 GetDelegate<mray_gst_netPlayerFrameCountDelegate>(libraryHandle, "mray_gst_netPlayerFrameCount");

			CreateTextureWrapper =
			 GetDelegate<CreateTextureWrapperDelegate>(libraryHandle, "CreateTextureWrapper");

			DeleteTextureWrapper =
			 GetDelegate<DeleteTextureWrapperDelegate>(libraryHandle, "DeleteTextureWrapper");		
			 
			SendTexture =
			 GetDelegate<SendTextureDelegate>(libraryHandle, "SendTexture");	
			 
			//mray_gst_StreamerResume =
			// GetDelegate<mray_gst_StreamerResumeDelegate>(libraryHandle, "mray_gst_StreamerResume");
			 
*/

			mray_gst_createCustomImageStreamer =
			 GetDelegate<mray_gst_createCustomImageStreamerDelegate>(libraryHandle, "mray_gst_createCustomImageStreamer");

			mray_gst_customImageStreamerSetPipeline =
			 GetDelegate<mray_gst_customImageStreamerSetPipelineDelegate>(libraryHandle, "mray_gst_customImageStreamerSetPipeline");

			mray_gst_customImageStreamerSetGrabber =
			 GetDelegate<mray_gst_customImageStreamerSetGrabberDelegate>(libraryHandle, "mray_gst_customImageStreamerSetGrabber");

			mray_gst_customImageStreamerSetResolution =
			 GetDelegate<mray_gst_customImageStreamerSetResolutionDelegate>(libraryHandle, "mray_gst_customImageStreamerSetResolution");

			mray_gst_createCustomVideoPlayer =
			 GetDelegate<mray_gst_createCustomVideoPlayerDelegate>(libraryHandle, "mray_gst_createCustomVideoPlayer");

			mray_gst_customPlayerSetPipeline =
			 GetDelegate<mray_gst_customPlayerSetPipelineDelegate>(libraryHandle, "mray_gst_customPlayerSetPipeline");

			mray_gst_customPlayerCreateStream =
			 GetDelegate<mray_gst_customPlayerCreateStreamDelegate>(libraryHandle, "mray_gst_customPlayerCreateStream");

			mray_gst_customPlayerSetLoop =
			 GetDelegate<mray_gst_customPlayerSetLoopDelegate>(libraryHandle, "mray_gst_customPlayerSetLoop");

			mray_gst_customPlayerIsLoop =
			 GetDelegate<mray_gst_customPlayerIsLoopDelegate>(libraryHandle, "mray_gst_customPlayerIsLoop");

			mray_gst_customPlayerGetFrameSize =
			 GetDelegate<mray_gst_customPlayerGetFrameSizeDelegate>(libraryHandle, "mray_gst_customPlayerGetFrameSize");

			mray_gst_customPlayerCopyFrame =
			 GetDelegate<mray_gst_customPlayerCopyFrameDelegate>(libraryHandle, "mray_gst_customPlayerCopyFrame");

			mray_gst_customPlayerCropFrame =
			 GetDelegate<mray_gst_customPlayerCropFrameDelegate>(libraryHandle, "mray_gst_customPlayerCropFrame");

			mray_gst_customPlayerBlitImage =
			 GetDelegate<mray_gst_customPlayerBlitImageDelegate>(libraryHandle, "mray_gst_customPlayerBlitImage");

			mray_gst_customPlayerBlitImageNativeGLCall =
			 GetDelegate<mray_gst_customPlayerBlitImageNativeGLCallDelegate>(libraryHandle, "mray_gst_customPlayerBlitImageNativeGLCall");

			mray_gst_customPlayerFrameCount =
			 GetDelegate<mray_gst_customPlayerFrameCountDelegate>(libraryHandle, "mray_gst_customPlayerFrameCount");

			mray_gst_customPlayerGrabAudioFrame =
			 GetDelegate<mray_gst_customPlayerGrabAudioFrameDelegate>(libraryHandle, "mray_gst_customPlayerGrabAudioFrame");

			mray_gst_customPlayerGetAudioFrameSize =
			 GetDelegate<mray_gst_customPlayerGetAudioFrameSizeDelegate>(libraryHandle, "mray_gst_customPlayerGetAudioFrameSize");
			 
			mray_gst_customPlayerChannelsCount =
			 GetDelegate<mray_gst_customPlayerChannelsCountDelegate>(libraryHandle, "mray_gst_customPlayerChannelsCount");

			mray_gst_customPlayerCopyAudioFrame =
			 GetDelegate<mray_gst_customPlayerCopyAudioFrameDelegate>(libraryHandle, "mray_gst_customPlayerCopyAudioFrame");

			mray_gst_StreamerDestroy =
			 GetDelegate<mray_gst_StreamerDestroyDelegate>(libraryHandle, "mray_gst_StreamerDestroy");

			mray_gst_StreamerCreateStream =
			 GetDelegate<mray_gst_StreamerCreateStreamDelegate>(libraryHandle, "mray_gst_StreamerCreateStream");

			mray_gst_StreamerStream =
			 GetDelegate<mray_gst_StreamerStreamDelegate>(libraryHandle, "mray_gst_StreamerStream");

			mray_gst_StreamerPause =
			 GetDelegate<mray_gst_StreamerPauseDelegate>(libraryHandle, "mray_gst_StreamerPause");


			mray_gst_StreamerStop =
			 GetDelegate<mray_gst_StreamerStopDelegate>(libraryHandle, "mray_gst_StreamerStop");

			mray_gst_StreamerIsStreaming =
			 GetDelegate<mray_gst_StreamerIsStreamingDelegate>(libraryHandle, "mray_gst_StreamerIsStreaming");

			mray_gst_StreamerClose =
			 GetDelegate<mray_gst_StreamerCloseDelegate>(libraryHandle, "mray_gst_StreamerClose");

			mray_gst_createUnityImageGrabber =
			 GetDelegate<mray_gst_createUnityImageGrabberDelegate>(libraryHandle, "mray_gst_createUnityImageGrabber");

			mray_gst_UnityImageGrabberSetData =
			 GetDelegate<mray_gst_UnityImageGrabberSetDataDelegate>(libraryHandle, "mray_gst_UnityImageGrabberSetData");

			mray_gst_UnityImageGrabberDestroy =
			 GetDelegate<mray_gst_UnityImageGrabberDestroyDelegate>(libraryHandle, "mray_gst_UnityImageGrabberDestroy");

			mray_createImageData =
			 GetDelegate<mray_createImageDataDelegate>(libraryHandle, "mray_createImageData");

			mray_resizeImageData =
			 GetDelegate<mray_resizeImageDataDelegate>(libraryHandle, "mray_resizeImageData");

			mray_getImageDataInfo =
			 GetDelegate<mray_getImageDataInfoDelegate>(libraryHandle, "mray_getImageDataInfo");

			mray_deleteImageData =
			 GetDelegate<mray_deleteImageDataDelegate>(libraryHandle, "mray_deleteImageData");

			mray_BlitImageDataInfo =
			 GetDelegate<mray_BlitImageDataInfoDelegate>(libraryHandle, "mray_BlitImageDataInfo");

			mray_BlitImageNativeGLCall =
			 GetDelegate<mray_BlitImageNativeGLCallDelegate>(libraryHandle, "mray_BlitImageNativeGLCall");

			mray_FlipImageData =
			 GetDelegate<mray_FlipImageDataDelegate>(libraryHandle, "mray_FlipImageData");

			mray_cloneImageData =
			 GetDelegate<mray_cloneImageDataDelegate>(libraryHandle, "mray_cloneImageData");

			mray_copyCroppedImageData  =
			 GetDelegate<mray_copyCroppedImageDataDelegate>(libraryHandle, "mray_copyCroppedImageData");

			mray_getImageDataPtr =
			 GetDelegate<mray_getImageDataPtrDelegate>(libraryHandle, "mray_getImageDataPtr");

			
			mray_gst_PlayerDestroy =
			 GetDelegate<mray_gst_PlayerDestroyDelegate>(libraryHandle, "mray_gst_PlayerDestroy");

			mray_gst_PlayerPlay =
			 GetDelegate<mray_gst_PlayerPlayDelegate>(libraryHandle, "mray_gst_PlayerPlay");

			mray_gst_PlayerPause =
			 GetDelegate<mray_gst_PlayerPauseDelegate>(libraryHandle, "mray_gst_PlayerPause");

			mray_gst_PlayerStop =
			 GetDelegate<mray_gst_PlayerStopDelegate>(libraryHandle, "mray_gst_PlayerStop");

			mray_gst_PlayerIsLoaded =
			 GetDelegate<mray_gst_PlayerIsLoadedDelegate>(libraryHandle, "mray_gst_PlayerIsLoaded");

			mray_gst_PlayerIsPlaying =
			 GetDelegate<mray_gst_PlayerIsPlayingDelegate>(libraryHandle, "mray_gst_PlayerIsPlaying");

			mray_gst_PlayerClose =
			 GetDelegate<mray_gst_PlayerCloseDelegate>(libraryHandle, "mray_gst_PlayerClose");

			mray_gst_PlayerSetPosition =
			 GetDelegate<mray_gst_PlayerSetPositionDelegate>(libraryHandle, "mray_gst_PlayerSetPosition");

			mray_gst_PlayerGetPosition =
			 GetDelegate<mray_gst_PlayerGetPositionDelegate>(libraryHandle, "mray_gst_PlayerGetPosition");

			mray_gst_PlayerGetDuration =
			 GetDelegate<mray_gst_PlayerGetDurationDelegate>(libraryHandle, "mray_gst_PlayerGetDuration");

			mray_gst_PlayerGetLastImage =
			 GetDelegate<mray_gst_PlayerGetLastImageDelegate>(libraryHandle, "mray_gst_PlayerGetLastImage");

			mray_gst_PlayerGetLastImageTimestamp =
			 GetDelegate<mray_gst_PlayerGetLastImageTimestampDelegate>(libraryHandle, "mray_gst_PlayerGetLastImageTimestamp");

			mray_gst_PlayerSendRTPMetaToHost =
			 GetDelegate<mray_gst_PlayerSendRTPMetaToHostDelegate>(libraryHandle, "mray_gst_PlayerSendRTPMetaToHost");

			mray_gst_PlayerRTPGetEyeGazeData =
			 GetDelegate<mray_gst_PlayerRTPGetEyeGazeDataDelegate>(libraryHandle, "mray_gst_PlayerRTPGetEyeGazeData");
			 
			mray_gst_playerGrabFrame= 
			 GetDelegate<mray_gst_playerGrabFrameDelegate>(libraryHandle, "mray_gst_playerGrabFrame");

			mray_gstreamer_initialize =
			 GetDelegate<mray_gstreamer_initializeDelegate>(libraryHandle, "mray_gstreamer_initialize");

			mray_gstreamer_shutdown =
			 GetDelegate<mray_gstreamer_shutdownDelegate>(libraryHandle, "mray_gstreamer_shutdown");

			mray_gstreamer_isActive =
			 GetDelegate<mray_gstreamer_isActiveDelegate>(libraryHandle, "mray_gstreamer_isActive");

			mray_SetDebugFunction =
			 GetDelegate<mray_SetDebugFunctionDelegate>(libraryHandle, "mray_SetDebugFunction");


			UnityEngine.Debug.Log("GStLib Setup Complete of Delegates");
		}
		
	#endif
		isSetup=true;		
	}
	
	public static void Unload ()
	{
	#if UNITY_EDITOR
	if(!isUnloaded)
	{
		if (libraryHandle != IntPtr.Zero)
		{	
			CloseLibrary(libraryHandle);	
			libraryHandle = IntPtr.Zero;

			mray_gst_createNetworkAudioPlayer = null;
			mray_gst_netAudioPlayerSetIP = null;
			mray_gst_netAudioPlayerCreateStream = null;
			mray_gst_netAudioPlayerSetVolume = null;
			mray_gst_netAudioPlayerGetAudioPort = null;
			mray_gst_netAudioPlayerUseCustomOutput = null;
			mray_gst_netAudioPlayerIsUsingCustomOutput = null;
			mray_gst_netAudioPlayerGrabFrame = null;
			mray_gst_netAudioPlayerGetFrameSize = null;
			mray_gst_netAudioPlayerCopyAudioFrame = null;
			mray_gst_netAudioPlayerChannelsCount = null;
			mray_gst_netAudioPlayerSetSampleRate = null;
			mray_gst_netAudioPlayerSampleRate = null;
			mray_gst_ProcessAudioPackets = null;
			mray_gst_createCustomImageStreamer = null;
			mray_gst_customImageStreamerSetPipeline = null;
			mray_gst_customImageStreamerSetGrabber = null;
			mray_gst_customImageStreamerSetResolution = null;
			mray_gst_createNetworkStreamer = null;
			mray_gst_netStreamerSetIP = null;
			mray_gst_netStreamerSetGrabber = null;
			mray_gst_netStreamerSetBitRate = null;
			mray_gst_netStreamerSetResolution = null;
			mray_gst_createCustomVideoPlayer = null;
			mray_gst_customPlayerSetPipeline = null;
			mray_gst_customPlayerCreateStream = null;
			mray_gst_customPlayerSetLoop = null;
			mray_gst_customPlayerIsLoop = null;
			mray_gst_customPlayerGetFrameSize = null;
			mray_gst_customPlayerCopyFrame = null;
			mray_gst_customPlayerCropFrame = null;
			mray_gst_customPlayerBlitImage = null;
			mray_gst_customPlayerBlitImageNativeGLCall = null;
			mray_gst_customPlayerFrameCount = null;
			mray_gst_customPlayerGrabAudioFrame = null;
			mray_gst_customPlayerGetAudioFrameSize = null;
			mray_gst_customPlayerChannelsCount = null;
			mray_gst_customPlayerCopyAudioFrame = null;
			mray_gst_StreamerDestroy = null;
			mray_gst_StreamerCreateStream = null;
			mray_gst_StreamerStream = null;
			mray_gst_StreamerPause = null;
			mray_gst_StreamerResume = null;
			mray_gst_StreamerStop = null;
			mray_gst_StreamerIsStreaming = null;
			mray_gst_StreamerClose = null;
			mray_gst_createNetworkMultiplePlayer = null;
			mray_gst_multiNetPlayerSetIP = null;
			mray_gst_multiNetPlayerCreateStream = null;
			mray_gst_multiNetPlayerGetFrameSize = null;
			mray_gst_multiNetPlayerGrabFrame = null;
			mray_gst_multiNetPlayerBlitImage = null;
			mray_gst_multiNetPlayerCopyData = null;
			mray_gst_multiNetPlayerFrameCount = null;
			mray_gst_multiNetPlayerGetVideoPort = null;
			mray_gst_multiNetPlayerSetFormat = null;
			mray_gst_multiNetPlayerGetFormat = null;
			mray_gst_multiNetPlayerGetNetworkUsage = null;
			mray_gst_multiNetPlayerBlitImageNativeGLCall = null;
			mray_gst_multiNetPlayerSetDecoderType = null;
			mray_gst_createUnityImageGrabber = null;
			mray_gst_UnityImageGrabberSetData = null;
			mray_gst_UnityImageGrabberDestroy = null;
			mray_createImageData = null;
			mray_resizeImageData = null;
			mray_getImageDataInfo = null;
			mray_deleteImageData = null;
			mray_BlitImageDataInfo = null;
			mray_BlitImageNativeGLCall = null;
			mray_FlipImageData = null;
			mray_cloneImageData = null;
			mray_copyCroppedImageData  = null;
			mray_getImageDataPtr = null;
			mray_gst_createAudioNetworkStreamer = null;
			mray_gst_audioStreamerAddClient = null;
			mray_gst_audioStreamerGetClientCount = null;
			mray_gst_audioStreamerRemoveClient = null;
			mray_gst_audioStreamerClearClients = null;
			mray_gst_audioStreamerGetClient = null;
			mray_gst_audioStreamerSetClientVolume = null;
			mray_gst_audioStreamerSetChannels = null;
			mray_gst_PlayerDestroy = null;
			mray_gst_PlayerPlay = null;
			mray_gst_PlayerPause = null;
			mray_gst_PlayerStop = null;
			mray_gst_PlayerIsLoaded = null;
			mray_gst_PlayerIsPlaying = null;
			mray_gst_PlayerClose = null;
			mray_gst_PlayerSetPosition = null;
			mray_gst_PlayerGetPosition = null;
			mray_gst_PlayerGetDuration = null;
			mray_gst_PlayerGetLastImage = null;
			mray_gst_PlayerGetLastImageTimestamp = null;
			mray_gst_PlayerSendRTPMetaToHost = null;
			mray_gst_PlayerRTPGetEyeGazeData = null;
			mray_gst_playerGrabFrame = null;
			mray_gstreamer_initialize = null;
			mray_gstreamer_shutdown = null;
			mray_gstreamer_isActive = null;
			mray_SetDebugFunction = null;
			mray_gst_createNetworkPlayer = null;
			mray_gst_netPlayerSetIP = null;
			mray_gst_netPlayerCreateStream = null;
			mray_gst_netPlayerGetFrameSize = null;
			mray_gst_netPlayerGrabFrame = null;
			mray_gst_netPlayerBlitImage = null;
			mray_gst_netPlayerFrameCount = null;
			CreateTextureWrapper = null;
			DeleteTextureWrapper = null;
			SendTexture = null;

		}
	}
	#endif
		isUnloaded=true;
	}
	
	
	void Awake()
	{
		Setup();
	}	


	void OnApplicationQuit()
	{
		Unload();
	}

}


