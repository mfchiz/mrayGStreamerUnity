

#ifndef __IAPPSINKHANDLER__
#define __IAPPSINKHANDLER__

#include <gst/app/gstappsink.h>
#include <gst/gst.h>

#include <memory>

#include "ListenerContainer.h"

namespace mray {
namespace video {
class IAppSinkHandler;

class IAppSinkHandlerListener {
 public:
  virtual void OnStreamPrepared(IAppSinkHandler* a){};
  virtual void OnNewSample(IAppSinkHandler* a){};
  virtual void OnNewVideoSample(IAppSinkHandler* a){};
  virtual void OnNewAudioSample(IAppSinkHandler* a){};
};

class IAppSinkHandler : public ListenerContainer<IAppSinkHandlerListener*> {
 protected:
  GstAppSink* m_sink;
  DECLARE_FIRE_METHOD(OnStreamPrepared, (IAppSinkHandler* v), (v));
  DECLARE_FIRE_METHOD(OnNewSample, (IAppSinkHandler* v), (v));
  DECLARE_FIRE_METHOD(OnNewVideoSample, (IAppSinkHandler* v), (v));
  DECLARE_FIRE_METHOD(OnNewAudioSample, (IAppSinkHandler* v), (v));

 public:
  IAppSinkHandler();
  ~IAppSinkHandler();

  virtual void SetSink(GstAppSink* s) { m_sink = s; }

  virtual void Close();

  virtual GstFlowReturn process_sample(std::shared_ptr<GstSample> sample,
                                       bool preroll) = 0;
  virtual GstFlowReturn preroll_cb(std::shared_ptr<GstSample> buffer);
  virtual GstFlowReturn buffer_cb(std::shared_ptr<GstSample> buffer);
  virtual void eos_cb();

  static GstFlowReturn on_new_buffer_from_source(GstAppSink* elt, void* data);
  static GstFlowReturn on_new_preroll_from_source(GstAppSink* elt, void* data);
  static void on_eos_from_source(GstAppSink* elt, void* data);
};

}  // namespace video
}  // namespace mray

#endif
