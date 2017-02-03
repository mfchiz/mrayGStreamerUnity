

#include "StreamersAPI.h"
#include "GStreamerCore.h"
#include "GraphicsInclude.h"
#include "PixelUtil.h"

#ifdef USE_UNITY_GRABBER
extern "C" UNITY_INTERFACE_EXPORT void* mray_gst_createUnityImageGrabber()
{
	GStreamerCore* c = GStreamerCore::Instance();
	if (c)
	{
		UnityImageGrabber* g = new UnityImageGrabber();
		return g;
	}
	return 0;
}

extern "C" UNITY_INTERFACE_EXPORT void mray_gst_UnityImageGrabberSetData(UnityImageGrabber* g, void* _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight, int Format)
{
	if (!g)
		return;
	g->SetData(_TextureNativePtr, _UnityTextureWidth, _UnityTextureHeight, Format);
}
#endif

extern "C" UNITY_INTERFACE_EXPORT void mray_gst_StreamerDestroy(IGStreamerStreamer* p)
{
	if (p != NULL)
	{
		p->Close();
		delete p;
	}

}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_StreamerStream(IGStreamerStreamer* p)
{

	if (p != NULL)
	{
		p->Stream();
	}
}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_StreamerPause(IGStreamerStreamer* p)
{
	if (p != NULL)
	{
		p->SetPaused(true);
	}
}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_StreamerStop(IGStreamerStreamer* p)
{
	if (p != NULL)
	{
		p->Stop();
	}

}
extern "C" UNITY_INTERFACE_EXPORT bool mray_gst_StreamerIsStreaming(IGStreamerStreamer* p)
{
	if (p != NULL)
	{
		return p->IsStreaming();
	}
	return false;

}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_StreamerClose(IGStreamerStreamer* p)
{
	if (p != NULL)
	{
		return p->Close();
	}

}

#ifdef USE_UNITY_NETWORK

extern "C" UNITY_INTERFACE_EXPORT void* mray_gst_createNetworkStreamer()
{
	GStreamerCore* c = GStreamerCore::Instance();
	if (c)
	{
		GstCustomVideoStreamer* g = new GstCustomVideoStreamer();
		return g;
	}
	return 0;
}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_netStreamerSetIP(GstCustomVideoStreamer* p, const char* ip, int videoPort, bool rtcp)
{
	if (p)
	{
		p->BindPorts(ip, videoPort, 0, rtcp);
	}
}
extern "C" UNITY_INTERFACE_EXPORT bool mray_gst_netStreamerCreateStream(GstCustomVideoStreamer* p)
{
	if (p)
	{
		return p->CreateStream();
	}
	return false;
}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_netStreamerSetGrabber(GstCustomVideoStreamer* p, UnityImageGrabber* g)
{
	if (p)
	{
		p->SetVideoGrabber(g);
	}

}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_netStreamerSetBitRate(GstCustomVideoStreamer* p, int bitRate)
{
	if (p)
	{
		p->SetBitRate(bitRate);
	}

}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_netStreamerSetResolution(GstCustomVideoStreamer* p, int width, int height, int fps)
{
	if (p)
	{
		p->SetResolution(width,height,fps);
	}

}

extern "C" UNITY_INTERFACE_EXPORT void* mray_gst_createAudioNetworkStreamer()
{

	GStreamerCore* c = GStreamerCore::Instance();
	if (c)
	{
		GstNetworkAudioStreamer* g = new GstNetworkAudioStreamer();
		return g;
	}
	return 0;
}

extern "C" UNITY_INTERFACE_EXPORT void mray_gst_audioStreamerAddClient(GstNetworkAudioStreamer* p, const char* ip, int port)
{
	if (p)
	{
		p->AddClient(ip, port, 0, 0);
	}

}
extern "C" UNITY_INTERFACE_EXPORT int mray_gst_audioStreamerGetClientCount(GstNetworkAudioStreamer* p)
{
	if (p)
	{
		return p->GetClientCount();
	}
	return 0;
}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_audioStreamerRemoveClient(GstNetworkAudioStreamer* p, int i)
{
	if (p)
	{
		return p->RemoveClient(i);
	}
}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_audioStreamerClearClients(GstNetworkAudioStreamer* p)
{
	if (p)
	{
		return p->ClearClients();
	}
}
extern "C" UNITY_INTERFACE_EXPORT const char* mray_gst_audioStreamerGetClient(GstNetworkAudioStreamer* p, int i)
{
	if (p)
	{
		return p->GetClientAddress(i).c_str();
	}
	return 0;
}
extern "C" UNITY_INTERFACE_EXPORT void mray_gst_audioStreamerSetClientVolume(GstNetworkAudioStreamer* p, int i, float vol)
{
	if (p)
	{
		return p->SetVolume(i, vol);
	}
}
extern "C" UNITY_INTERFACE_EXPORT bool mray_gst_audioStreamerCreateStream(GstNetworkAudioStreamer* p)
{

	if (p)
	{
		return p->CreateStream();
	}
	return false;
}

extern "C" UNITY_INTERFACE_EXPORT void mray_gst_audioStreamerSetChannels(GstNetworkAudioStreamer* p, int c)
{
	if (p)
	{
		p->SetChannels(c);
	}

}

#endif
