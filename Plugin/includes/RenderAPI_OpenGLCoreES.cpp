#include "PlatformBase.h"
#include "RenderAPI.h"

// OpenGL Core profile (desktop) or OpenGL ES (mobile) implementation of
// RenderAPI. Supports several flavors: Core, ES2, ES3

#if SUPPORT_OPENGL_UNIFIED

#include <assert.h>
#if UNITY_IOS || UNITY_TVOS
#	include <OpenGLES/ES2/gl.h>
#elif UNITY_ANDROID || UNITY_WEBGL
#	include <GLES2/gl2.h>
#elif UNITY_OSX
#	include <OpenGL/gl3.h>
#elif UNITY_WIN
// On Windows, use gl3w to initialize and load OpenGL Core functions. In principle any other
// library (like GLEW, GLFW etc.) can be used; here we use gl3w since it's simple and
// straightforward.
#	include "gl3w/gl3w.h"
#elif UNITY_LINUX
#	define GL_GLEXT_PROTOTYPES
#	include <GL/gl.h>
#elif UNITY_EMBEDDED_LINUX
#	include <GLES2/gl2.h>
#if SUPPORT_OPENGL_CORE
#	define GL_GLEXT_PROTOTYPES
#	include <GL/gl.h>
#endif
#else
#	error Unknown platform
#endif



class RenderAPI_OpenGLCoreES : public RenderAPI {
   public:
    RenderAPI_OpenGLCoreES(UnityGfxRenderer apiType);
    virtual ~RenderAPI_OpenGLCoreES() {}

    virtual void ProcessDeviceEvent(UnityGfxDeviceEventType type,
                                    IUnityInterfaces* interfaces);

    virtual bool GetUsesReverseZ() { return false; }
    virtual void* BeginModifyTexture(void* textureHandle, int textureWidth,
                                     int textureHeight, int* outRowPitch);
    virtual void EndModifyTexture(void* textureHandle, int textureWidth,
                                  int textureHeight, int components,
                                  int rowPitch, void* dataPtr);

    virtual void* BeginModifyVertexBuffer(void* bufferHandle,
                                          size_t* outBufferSize);
    virtual void EndModifyVertexBuffer(void* bufferHandle);

   private:
    void CreateResources();

   private:
    UnityGfxRenderer m_APIType;
};

RenderAPI* CreateRenderAPI_OpenGLCoreES(UnityGfxRenderer apiType) {
    return new RenderAPI_OpenGLCoreES(apiType);
}

void RenderAPI_OpenGLCoreES::CreateResources() {

#	if UNITY_WIN && SUPPORT_OPENGL_CORE
	if (m_APIType == kUnityGfxRendererOpenGLCore)
		gl3wInit();
#	endif

	// Make sure that there are no GL error flags set before creating resources
	while (glGetError() != GL_NO_ERROR) {}


	assert(glGetError() == GL_NO_ERROR);
	
	
}

RenderAPI_OpenGLCoreES::RenderAPI_OpenGLCoreES(UnityGfxRenderer apiType)
    : m_APIType(apiType) {}

void RenderAPI_OpenGLCoreES::ProcessDeviceEvent(UnityGfxDeviceEventType type,
                                                IUnityInterfaces* interfaces) {
    if (type == kUnityGfxDeviceEventInitialize) {
        CreateResources();
    } else if (type == kUnityGfxDeviceEventShutdown) {
        //@TODO: release resources
    }
}

void* RenderAPI_OpenGLCoreES::BeginModifyTexture(void* textureHandle,
                                                 int textureWidth,
                                                 int textureHeight,
                                                 int* outRowPitch) {
    const int rowPitch = textureWidth * 4;
    return 0;
}

void RenderAPI_OpenGLCoreES::EndModifyTexture(void* textureHandle,
                                              int textureWidth,
                                              int textureHeight, int components,
                                              int rowPitch, void* dataPtr) {
    GLuint gltex = (GLuint)(size_t)(textureHandle);
    // Update texture data, and free the memory buffer

    // if(components==1)
    //    fmt=GL_ALPHA8;
    glBindTexture(GL_TEXTURE_2D, gltex);
    GLuint fmt = GL_RGBA;
    if (components == 1) fmt = GL_LUMINANCE;
    // glGetTexLevelParameteriv(GL_TEXTURE_2D,0,GL_TEXTURE_INTERNAL_FORMAT,&fmt);

    glTexSubImage2D(GL_TEXTURE_2D, 0, 0, 0, textureWidth, textureHeight, fmt,
                    GL_UNSIGNED_BYTE, dataPtr);
}

void* RenderAPI_OpenGLCoreES::BeginModifyVertexBuffer(void* bufferHandle,
                                                      size_t* outBufferSize) {
#if SUPPORT_OPENGL_ES
    return 0;
#else
    glBindBuffer(GL_ARRAY_BUFFER, (GLuint)(size_t)bufferHandle);
    GLint size = 0;
    glGetBufferParameteriv(GL_ARRAY_BUFFER, GL_BUFFER_SIZE, &size);
    *outBufferSize = size;
    void* mapped = glMapBuffer(GL_ARRAY_BUFFER, GL_WRITE_ONLY);
    return mapped;
#endif
}

void RenderAPI_OpenGLCoreES::EndModifyVertexBuffer(void* bufferHandle) {
#if !SUPPORT_OPENGL_ES
    glBindBuffer(GL_ARRAY_BUFFER, (GLuint)(size_t)bufferHandle);
    glUnmapBuffer(GL_ARRAY_BUFFER);
#endif
}

#endif  // #if SUPPORT_OPENGL_UNIFIED
