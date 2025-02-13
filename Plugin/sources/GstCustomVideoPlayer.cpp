
#include "GstCustomVideoPlayer.h"

#include <gst/app/gstappsink.h>
#include <gst/gst.h>

#include "AudioAppSinkHandler.h"
#include "GstPipelineHandler.h"
#include "UnityHelpers.h"
#include "VideoAppSinkHandler.h"
#include "stdafx.h"

namespace mray {
namespace video {

class GstCustomVideoPlayerImpl : public GstPipelineHandler,
                                 public IPipelineListener {
 public:
  std::string m_inputPipeline;
  std::string m_pipeLineString;

  GstAppSink* m_videoSink;
  GstAppSink* m_audioSink;

  VideoAppSinkHandler m_videoHandler;
  AudioAppSinkHandler m_audioHandler;


  bool _loop;

 public:
  virtual void OnPipelineEOS(GstPipelineHandler* p) {
    if (_loop) this->SetPosition(0);
  }
 public:
  GstCustomVideoPlayerImpl() {
    m_videoSink = 0;
    m_audioSink = 0;
    _loop = false;

    //listen to app sink events
 //   m_videoHandler.AddListener(this); // add sink listening
    this->AddListener(this);          // add pipeline listening

  }
  virtual ~GstCustomVideoPlayerImpl() {
    this->RemoveListener(this);
  }

  void _BuildPipeline() {
    m_pipeLineString = m_inputPipeline;  // +" ! appsink name=videoSink ";
  }

  void SetPipelineString(const std::string& pipeline) {
    m_inputPipeline = pipeline;
  }
  bool CreateStream() {
    if (IsLoaded()) return true;
    _BuildPipeline();

    GError* err = 0;
    GstElement* p = gst_parse_launch(m_pipeLineString.c_str(), &err); 
    LogMessage("Creating Pipeline: " + m_pipeLineString, ELL_INFO);
    
    LogMessage(gst_version_string(), ELL_INFO);
        
    if (err) {
      LogMessage(
          std::string("GstCustomVideoPlayer: Pipeline error: ") + err->message,
          ELL_ERROR);
    }
    if (!p) return false;

    SetPipeline(p);

    m_videoSink = GST_APP_SINK(gst_bin_get_by_name(GST_BIN(p), "videoSink"));
    if (m_videoSink) {
    
      m_videoHandler.SetSink(m_videoSink);

      // attach videosink callbacks
      GstAppSinkCallbacks gstCallbacks;
    
      memset(&gstCallbacks, 0, sizeof(gstCallbacks));
      
      gstCallbacks.eos = &VideoAppSinkHandler::on_eos_from_source;
     
      gstCallbacks.new_preroll = &VideoAppSinkHandler::on_new_preroll_from_source;

      gstCallbacks.new_sample = &VideoAppSinkHandler::on_new_buffer_from_source;

      gst_app_sink_set_callbacks(m_videoSink, &gstCallbacks,&m_videoHandler, NULL);

    }

    void* audio = (gst_bin_get_by_name(GST_BIN(p), "audioSink"));
    if (audio) {
      m_audioSink = GST_APP_SINK(audio);
      m_audioHandler.SetSink(m_audioSink);

      // setup video sink
      gst_base_sink_set_sync(GST_BASE_SINK(m_audioSink), true);
      g_object_set(G_OBJECT(m_audioSink), "emit-signals", FALSE, (void*)NULL);
      // 		gst_base_sink_set_async_enabled(GST_BASE_SINK(m_videoSink),
      // TRUE);

      GstAppSinkCallbacks gstCallbacks;
      gstCallbacks.eos = &AudioAppSinkHandler::on_eos_from_source;
      gstCallbacks.new_preroll =
          &AudioAppSinkHandler::on_new_preroll_from_source;

      gstCallbacks.new_sample = &AudioAppSinkHandler::on_new_buffer_from_source;

      gst_app_sink_set_callbacks(GST_APP_SINK(m_audioSink), &gstCallbacks,
                                 &m_audioHandler, NULL);
    }

    return CreatePipeline(false, "", 0);
  }

  bool IsStream() { return true; }


  virtual void Close() {
    m_videoHandler.Close();
    m_audioHandler.Close();
    GstPipelineHandler::Close();
  }

  void Play() { SetPaused(false); }
  void Pause() { SetPaused(true); }
  void SetVolume(float vol) {
    g_object_set(G_OBJECT(GetPipeline()), "volume", (gdouble)vol, (void*)0);
  }

  virtual const Vector2d& GetFrameSize() {
    return m_videoHandler.GetFrameSize();
  }

  virtual video::EPixelFormat GetImageFormat() {
    return m_videoHandler.getPixelsRef()->format;
  }
  virtual float GetCaptureFrameRate() {
    return m_videoHandler.GetCaptureFrameRate();
  }

  virtual bool GrabFrame() {
    bool ok = m_videoHandler.GrabFrame();
    return ok;
  }
  virtual bool HasNewFrame() { return m_videoHandler.isFrameNew(); }
  virtual ulong GetBufferID() { return m_videoHandler.GetFrameID(); }

  virtual const ImageInfo* GetLastFrame() {
    return m_videoHandler.getPixelsRef();
  }

  virtual unsigned long GetLastFrameTimestamp() {
    return m_videoHandler.getPixelFrame()->RTPPacket.timestamp;
  }
  void* GetLastFrameRTPMeta() {
    return &m_videoHandler.getPixelFrame()->RTPPacket;
  }

  virtual bool GrabAudioFrame() {
    bool ok = m_audioHandler.GrabFrame();
    return ok;
  }
  int GetAudioFrameSize() { return m_audioHandler.GetFrameSize(); }
  bool CopyAudioFrame(float* output) {
    m_audioHandler.CopyAudioFrame(output);
    return true;
  }
  int GetAudioChannelsCount() { return m_audioHandler.ChannelsCount(); }

};

GstCustomVideoPlayer::GstCustomVideoPlayer() {
  m_impl = new GstCustomVideoPlayerImpl();
  m_impl->m_videoHandler.AddListener(this); // add sink listening
}

GstCustomVideoPlayer::~GstCustomVideoPlayer() { 
  m_impl->m_videoHandler.RemoveListener(this); 
  m_newVideoSampleCallback = nullptr;
  delete m_impl; 
}

GstPipelineHandler* GstCustomVideoPlayer::GetPipeline() { return m_impl; }
void GstCustomVideoPlayer::SetPipelineString(const std::string& pipeline) {
  m_impl->SetPipelineString(pipeline);
}

bool GstCustomVideoPlayer::CreateStream() { return m_impl->CreateStream(); }

void GstCustomVideoPlayer::SetVolume(float vol) { m_impl->SetVolume(vol); }
bool GstCustomVideoPlayer::IsStream() { return m_impl->IsStream(); }

void GstCustomVideoPlayer::Play() { m_impl->Play(); }
void GstCustomVideoPlayer::Stop() { m_impl->Stop(); }
void GstCustomVideoPlayer::Pause() { m_impl->Pause(); }
bool GstCustomVideoPlayer::IsLoaded() { return m_impl->IsLoaded(); }
bool GstCustomVideoPlayer::IsPlaying() { return m_impl->IsPlaying(); }

void GstCustomVideoPlayer::Close() { 
  m_newVideoSampleCallback=nullptr;
  m_impl->Close(); 
}

const Vector2d& GstCustomVideoPlayer::GetFrameSize() {
  return m_impl->GetFrameSize();
}

video::EPixelFormat GstCustomVideoPlayer::GetImageFormat() {
  return m_impl->GetImageFormat();
}

bool GstCustomVideoPlayer::GrabFrame(int i) { return m_impl->GrabFrame(); }
bool GstCustomVideoPlayer::GrabAudioFrame() { return m_impl->GrabAudioFrame(); }

bool GstCustomVideoPlayer::HasNewFrame() { return m_impl->HasNewFrame(); }

ulong GstCustomVideoPlayer::GetBufferID() { return m_impl->GetBufferID(); }
float GstCustomVideoPlayer::GetCaptureFrameRate() {
  return m_impl->GetCaptureFrameRate();
}
bool GstCustomVideoPlayer::SetPosition(signed long pos) {
  return m_impl->SetPosition(pos);
}
signed long GstCustomVideoPlayer::GetPosition() {
  return m_impl->GetPosition();
}
signed long GstCustomVideoPlayer::GetDuration() {
  return m_impl->GetDuration();
}

const ImageInfo* GstCustomVideoPlayer::GetLastFrame(int i) {
  return m_impl->GetLastFrame();
}
unsigned long GstCustomVideoPlayer::GetLastFrameTimestamp(int i) {
  return m_impl->GetLastFrameTimestamp();
}
void* GstCustomVideoPlayer::GetLastFrameRTPMeta(int i) {
  return m_impl->GetLastFrameRTPMeta();
}
void GstCustomVideoPlayer::SetLoop(bool loop) { m_impl->_loop = loop; }

bool GstCustomVideoPlayer::IsLoop() { return m_impl->_loop; }

int GstCustomVideoPlayer::GetAudioFrameSize() {
  return m_impl->GetAudioFrameSize();
}
bool GstCustomVideoPlayer::CopyAudioFrame(float* output) {
  return m_impl->CopyAudioFrame(output);
}
int GstCustomVideoPlayer::GetAudioChannelsCount() {
  return m_impl->GetAudioChannelsCount();
}

}  // namespace video
}  // namespace mray
