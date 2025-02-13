

#ifndef __GSTCUSTOMVIDEOPLAYER__
#define __GSTCUSTOMVIDEOPLAYER__

#include <string>

#include "IGStreamerPlayer.h"
#include "IVideoGrabber.h"
#include "VideoAppSinkHandler.h"
#include "UnityHelpers.h"
namespace mray {

namespace video {

// expose the NewSampleCallback signature where relevant.
extern "C"
{
	typedef void(UNITY_INTERFACE_API* NewVideoSampleCallback)(void* p);
}

class GstCustomVideoPlayerImpl;

class GstCustomVideoPlayer : public IGStreamerPlayer,
                            public IAppSinkHandlerListener {
   protected:
    GstCustomVideoPlayerImpl* m_impl;
    GstPipelineHandler* GetPipeline();

    NewVideoSampleCallback m_newVideoSampleCallback = nullptr;

   public:
    GstCustomVideoPlayer();
    virtual ~GstCustomVideoPlayer();

    void SetPipelineString(const std::string& pipeline);
    bool CreateStream();

    virtual bool IsStream();

    void SetVolume(float vol);

    virtual void Play();
    virtual void Pause();
    virtual void Stop();
    virtual void SetLoop(bool loop);
    virtual bool IsLoop();
    virtual bool IsLoaded();
    virtual bool IsPlaying();
    virtual void Close();
    virtual bool SetPosition(signed long pos);
    virtual signed long GetPosition();
    virtual signed long GetDuration();

    // defined by the source video stream
    virtual void SetFrameSize(int w, int h) {}
    virtual const Vector2d& GetFrameSize();

    // defined by the source video stream
    virtual void SetImageFormat(video::EPixelFormat fmt) {}
    virtual video::EPixelFormat GetImageFormat();

    virtual bool GrabFrame(int i = 0);
    virtual bool GrabAudioFrame();
    virtual bool HasNewFrame();
    virtual ulong GetBufferID();  // incremented once per frame

    virtual const ImageInfo* GetLastFrame(int i = 0);
    virtual unsigned long GetLastFrameTimestamp(int i = 0);
    virtual void* GetLastFrameRTPMeta(int i = 0);
    virtual float GetCaptureFrameRate();

    int GetAudioFrameSize();
    bool CopyAudioFrame(float* output);
    int GetAudioChannelsCount();

    virtual int GetPort(int i) { return 0; }

    void SetNewVideoSampleCallback(NewVideoSampleCallback cb)
    {
        m_newVideoSampleCallback = cb;
    }

    virtual void OnNewVideoSample(IAppSinkHandler* a) {
        if(!m_newVideoSampleCallback)
        {
         return;
        }
        if(this->IsPlaying())
        {
            m_newVideoSampleCallback(this);  
        }    
    }

};

}  // namespace video
}  // namespace mray

#endif
