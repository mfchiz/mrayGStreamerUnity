

#include "UnityHelpers.h"

#include "Unity/IUnityInterface.h"
#ifdef WIN32
#include <windows.h>
#endif
//#include <gl/gl.h>
#include <glib.h>
#include <chrono>
#include <ctime>



#include "GraphicsInclude.h"
#include "PlatformBase.h"
#include "PixelUtil.h"
#include "ImageInfo.h"
#include "UnityGraphicsDevice.h"

using namespace mray;
using namespace video;

FuncPtr Debug;
FuncFloatRetPtr GetEngineTimePtr;

void LogMessage(const std::string& msg, ELogLevel level)
{
	std::string m;
	if (level == ELL_INFO)
		m = "Info: ";
	if (level == ELL_WARNING)
		m = "Warning: ";
	if (level == ELL_SUCCESS)
		m = "Success: ";
	if (level == ELL_ERROR)
		m = "Error: ";

	m += msg;


#if UNITY_LINUX
	m += '\n';
#endif

#if UNITY_WIN
	OutputDebugStringA(m.c_str());
#else
	printf("%s", m.c_str());
#endif

	LogManager::Instance()->LogMessage(m);
// 	if (Debug)
// 		Debug(m.c_str());
}
float GetEngineTime()
{
	if (GetEngineTimePtr)
	{
		return GetEngineTimePtr();
	}
	return 0;
}
extern "C" UNITY_INTERFACE_EXPORT void mray_SetDebugFunction(FuncPtr f)
{
	Debug = f;
}
extern "C" UNITY_INTERFACE_EXPORT void mray_SetGetEngineTime(FuncFloatRetPtr f)
{
	GetEngineTimePtr = f;
}

LogManager* LogManager::s_instance=0;
LogManager* LogManager::Instance()
{
	if (!s_instance)
	{
		s_instance = new LogManager();
	}
	return s_instance;
}

LogManager::LogManager()
{
    logFilePath = DetermineLogFilePath("GStreamerLog.txt");

	// empty log
	m_logFile = fopen(logFilePath.c_str(), "w");
	if (m_logFile)
	{
		fclose(m_logFile);
		m_logFile = 0;
	}
}
LogManager::~LogManager()
{
	if (m_logFile)
	{
		fclose(m_logFile);
		m_logFile = 0;
	}
}
void LogManager::LogMessage(const std::string& msg)
{
    auto end = std::chrono::system_clock::now();
    std::time_t end_time = std::chrono::system_clock::to_time_t(end);
    std::string end_ctime(std::ctime(&end_time));
    {
        const std::lock_guard<std::mutex> lock(m_mutex);
        m_logFile = fopen(logFilePath.c_str(), "a");
        fprintf(m_logFile, "[%s] %s\n", end_ctime.substr(end_ctime.find(':') - 2, 8).c_str(), msg.c_str());
        if (m_logFile)
        {
            fclose(m_logFile);
            m_logFile = 0;
        }
    }
}

std::string LogManager::DetermineLogFilePath(const std::string& fileName)
{
    std::string logFilePathDirectory = "";
    const gchar* environmentLogFilePath = g_getenv("GSTREAMER_UNITY_LOG_FILE_PATH");

    if (environmentLogFilePath != NULL)
    {
        logFilePathDirectory = environmentLogFilePath;
        logFilePathDirectory += "\\";
    }

    return logFilePathDirectory + fileName;
}


void CopyToTexture(const ImageInfo* src, uchar* dst,video::EPixelFormat fmt)
{
	if (fmt == video::EPixel_I420 
		|| fmt==video::EPixelFormat::EPixel_LUMINANCE8
		|| fmt == video::EPixelFormat::EPixel_R8G8B8 
		|| fmt == video::EPixelFormat::EPixel_B8G8R8
		|| fmt ==video::EPixelFormat::EPixel_R8G8B8A8 
        || 
		((fmt==video::EPixel_LUMINANCE8 || fmt==video::EPixel_Alpha8 ) &&
		(src->format == video::EPixel_LUMINANCE8 || src->format == video::EPixel_Alpha8 || src->format == video::EPixel_LUMINANCE16)))
	{
		memcpy(dst, src->imageData, src->imageDataSize);
		return;
	}
	int len = src->Size.x*src->Size.y * 3;
	uchar* ptr = dst;
	uchar* srcPtr = src->imageData;
	for (int i = 0; i < len; i += 3)
	{
        if(src->format==video::EPixelFormat::EPixel_R8G8B8 ||
           src->format == video::EPixelFormat::EPixel_B8G8R8)
        {
            ptr[0] = srcPtr[0];
            ptr[1] = srcPtr[1];
            ptr[2] = srcPtr[2];
            srcPtr += 3;
                            
        }else if(src->format==video::EPixelFormat::EPixel_Alpha8 ||
                 src->format == video::EPixelFormat::EPixel_LUMINANCE8)
        {
            ptr[0] = srcPtr[0];
            ptr[1] = srcPtr[0];
            ptr[2] = srcPtr[0];
            srcPtr ++;
        }
        ptr[3] = 255;
		ptr += 4;
	}
}

	const char* getEPixelFormatName(enum EPixelFormat format) 
	{
	   switch (format) 
	   {
		case EPixel_Unkown: return "EPixel_Unkown";
		case EPixel_LUMINANCE8: return "EPixel_LUMINANCE8";
		case EPixel_LUMINANCE16: return "EPixel_LUMINANCE16";
		case EPixel_Alpha8: return "EPixel_Alpha8";
		case EPixel_Alpha4Luminance4: return "EPixel_Alpha4Luminance4";
		case EPixel_Alpha8Luminance8: return "EPixel_Alpha8Luminance8";
		case EPixel_R5G6B5: return "EPixel_R5G6B5";
		case EPixel_R8G8B8: return "EPixel_R8G8B8";
		case EPixel_R8G8B8A8: return "EPixel_R8G8B8A8";
		case EPixel_X8R8G8B8: return "EPixel_X8R8G8B8";
		case EPixel_B8G8R8: return "EPixel_B8G8R8";
		case EPixel_B8G8R8A8: return "EPixel_B8G8R8A8";
		case EPixel_X8B8G8R8: return "EPixel_X8B8G8R8";
		case EPixel_Float16_R: return "EPixel_Float16_R";
		case EPixel_Float16_RGB: return "EPixel_Float16_RGB";
		case EPixel_Float16_RGBA: return "EPixel_Float16_RGBA";
		case EPixel_Float16_GR: return "EPixel_Float16_GR";
		case EPixel_Float32_R: return "EPixel_Float32_R";
		case EPixel_Float32_RGB: return "EPixel_Float32_RGB";
		case EPixel_Float32_RGBA: return "EPixel_Float32_RGBA";
		case EPixel_Float32_GR: return "EPixel_Float32_GR";
		case EPixel_Depth: return "EPixel_Depth";
		case EPixel_Stecil: return "EPixel_Stecil";
		case EPixel_Short_RGBA: return "EPixel_Short_RGBA";
		case EPixel_Short_RGB: return "EPixel_Short_RGB";
		case EPixel_Short_GR: return "EPixel_Short_GR";
		case EPixel_DXT1: return "EPixel_DXT1";
		case EPixel_DXT2: return "EPixel_DXT2";
		case EPixel_DXT3: return "EPixel_DXT3";
		case EPixel_DXT4: return "EPixel_DXT4";
		case EPixel_DXT5: return "EPixel_DXT5";
		case EPixel_I420: return "EPixel_I420";
		case EPixel_Y42B: return "EPixel_Y42B";
		case EPixel_NV12: return "EPixel_NV12";

	   }
	   return "EPixel_Unkown";
	}



void CheckData(const ImageInfo* ifo, int _UnityTextureWidth, int _UnityTextureHeight,uchar** data,int* pitch,int* comps)
{

     
    std::string s = getEPixelFormatName(ifo->format);
        
    LogMessage("CheckDataFormat", ELL_INFO);
    LogMessage(s, ELL_INFO);
        
    
    if (ifo->format == video::EPixel_I420 ||
		ifo->format == video::EPixel_NV12 ||
		ifo->format == video::EPixel_Y42B ||
        (ifo->format==video::EPixel_LUMINANCE8 || ifo->format==video::EPixel_Alpha8 ))
    {
        *comps=1;
        //data = new uchar[ifo->imageDataSize];
        *pitch = _UnityTextureWidth;
        
        *data = ifo->imageData;
                
	}
	else if (ifo->format == video::EPixel_LUMINANCE16)
	{
		*comps = 1;
		*pitch = _UnityTextureWidth;
		*data = ifo->imageData;
	        
	}
	else if (ifo->format == video::EPixel_R8G8B8A8)
	{
		*comps = 4;
		*pitch = _UnityTextureWidth;
		*data = ifo->imageData;
	        
	}
	else if (ifo->format == video::EPixel_B8G8R8A8)
	{
		*comps = 4;
		*pitch = _UnityTextureWidth;
		*data = ifo->imageData;
	        
	}
    else
    {
        //				data = new uchar[_UnityTextureWidth*_UnityTextureHeight * 4];
        // 				pitch = _UnityTextureWidth * 3;
        // 				data = ifo->imageData;

    
       
        if (ifo->tmpBuffer->Size != ifo->Size)
        {
            ifo->tmpBuffer->createData(ifo->Size, video::EPixel_R8G8B8A8);
        }
      
      
        *pitch = _UnityTextureWidth * 4;
        *data = ifo->tmpBuffer->imageData;
        *comps=4;
        CopyToTexture(ifo, (uchar*)*data, video::EPixel_R8G8B8A8);//ifo->format);
        	    
    }
}

void BlitImage(const ImageInfo* ifo, void* _TextureNativePtr, int _UnityTextureWidth, int _UnityTextureHeight)
{
    
    if (!ifo || !_TextureNativePtr)
        return;
        
    if (ifo->tmpBuffer == 0)
        ((video::ImageInfo*)ifo)->tmpBuffer = new ImageInfo();      
    
    uchar* data = 0;
    int pitch = 0;
    int comps=1;

#if 0
#if SUPPORT_D3D9
    // D3D9 case
    if (g_GraphicsDeviceType == kGfxRendererD3D9)
    {
        // Update native texture from code
        if (_TextureNativePtr)
        {
            IDirect3DTexture9* d3dtex = (IDirect3DTexture9*)_TextureNativePtr;
            D3DSURFACE_DESC desc;
            d3dtex->GetLevelDesc(0, &desc);
            D3DLOCKED_RECT lr;
            d3dtex->LockRect(0, &lr, nullptr, 0);
            
            //uchar* data = new uchar[desc.Width*desc.Height * 4];
            
            //memcpy((unsigned char*)lr.pBits, ifo->imageData, desc.Width*desc.Height * 3);
            CopyToTexture(ifo, (uchar*)lr.pBits,ifo->format);
            
            d3dtex->UnlockRect(0);
            //delete [] data;
        }
    }
#endif
    
#if SUPPORT_D3D11
    // D3D11 case
    if (g_GraphicsDeviceType == kGfxRendererD3D11)
    {
        ID3D11DeviceContext* ctx = nullptr;
        g_D3D11GraphicsDevice->GetImmediateContext(&ctx);
        
        // update native texture from code
        if (_TextureNativePtr)
        {
            ID3D11Texture2D* d3dtex = (ID3D11Texture2D*)_TextureNativePtr;
            D3D11_TEXTURE2D_DESC desc;
            d3dtex->GetDesc(&desc);
            //ctx->UpdateSubresource(d3dtex, 0, nullptr, ifo->imageData, desc.Width * 3, 0);
            
            CheckData(ifo,_UnityTextureWidth,_UnityTextureWidth,&data,&pitch);
            ctx->UpdateSubresource(d3dtex, 0, nullptr, data, pitch, 0);
            //delete[] data;
            
        }
        
        ctx->Release();
    }
#endif
#endif
    
        if (_TextureNativePtr)
        {          
             
            CheckData(ifo,_UnityTextureWidth,_UnityTextureHeight,&data,&pitch,&comps);
       
            GetRenderer()->BeginModifyTexture(_TextureNativePtr, _UnityTextureWidth, _UnityTextureHeight, &pitch);
            GetRenderer()->EndModifyTexture(_TextureNativePtr, _UnityTextureWidth, _UnityTextureHeight,comps, pitch, data);
            
        }

}

