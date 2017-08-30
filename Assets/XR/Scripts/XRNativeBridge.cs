using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace XRInternal {

public class XRNativeBridge {
  // Fallback interfaces for editor mode.
  void XRCreateEditor() { }
  void XRConfigureEditor() { }
  void XRResumeEditor() { }
  void XRPauseEditor() { }
  void XRDestroyEditor() { }
  void XRSetManagedCameraTextureEditor(System.IntPtr texHandle, int width, int height) { }
  void XRRenderFrameForDisplayEditor() { }

  void XRGetCurrentRealityEditor() {
    if (xrResponse == null) {
      xrResponse = new XRResponseRef();
    }
    xrResponse.ptr = new c8_XRResponse(0);
    xrResponse.ptr.eventIdTimeMicros = 600000000000;
    xrResponse.ptr.lightingGlobalExposure = 0.0f;
    xrResponse.ptr.cameraExtrinsicRotationW = 1.0f;
    xrResponse.ptr.cameraIntrinsicMatrix44f[0] = 2.92424f;
    xrResponse.ptr.cameraIntrinsicMatrix44f[5] = 1.64488f;
    xrResponse.ptr.cameraIntrinsicMatrix44f[9] = 0.0015625f;
    xrResponse.ptr.cameraIntrinsicMatrix44f[10] = -1.0006f;
    xrResponse.ptr.cameraIntrinsicMatrix44f[11] = -1.0f;
    xrResponse.ptr.cameraIntrinsicMatrix44f[14] = -0.60018f;
  }

  // iOS native interfaces.
 #if (UNITY_IPHONE && !UNITY_EDITOR)
  [DllImport("__Internal")]
  private static extern IntPtr c8XRIos_create();

  [DllImport("__Internal")]
  private static extern void c8XRIos_configureXR(
    IntPtr rEngine,
    ref c8_XRConfiguration config);

  [DllImport("__Internal")]
  private static extern void c8XRIos_resume(IntPtr rEngine);

  [DllImport("__Internal")]
  private static extern void c8XRIos_getCurrentRealityXR(IntPtr rEngine, ref c8_XRResponse reality);

  [DllImport("__Internal")]
  private static extern void c8XRIos_pause(IntPtr rEngine);

  [DllImport("__Internal")]
  private static extern void c8XRIos_destroy(IntPtr rEngine);

  [DllImport("__Internal")]
  private static extern void c8XRIos_setManagedCameraTexture(
     IntPtr rEngine, System.IntPtr texHandle, int width, int height);

  [DllImport("__Internal")]
  private static extern void c8XRIos_renderFrameForDisplay(IntPtr rEngine);

  private IntPtr realityEngine;

  void XRCreate() { realityEngine = c8XRIos_create(); }
  void XRConfigure() { c8XRIos_configureXR(realityEngine, ref xrConfig.ptr); }
  void XRResume() { c8XRIos_resume(realityEngine); }
  void XRGetCurrentReality() { c8XRIos_getCurrentRealityXR(realityEngine, ref xrResponse.ptr); }
  void XRPause() { c8XRIos_pause(realityEngine); }
  void XRDestroy() { c8XRIos_destroy(realityEngine);}
  void XRRenderFrameForDisplay() { c8XRIos_renderFrameForDisplay(realityEngine); }
  void XRSetManagedCameraTexture(System.IntPtr texHandle, int width, int height) {
    c8XRIos_setManagedCameraTexture(realityEngine, texHandle, width, height);
  }

// Android JNI interfaces.
#elif (UNITY_ANDROID && !UNITY_EDITOR)

  private class XRAndroid {
    AndroidJavaObject androidXR;

    public static XRAndroid create() { return new XRAndroid(); }

    private XRAndroid() {
      // Load the native library required by XRAndroid's JNI layer.
      c8_loadXRDll();

      // Get the UnityPlayer Activity.
      AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
      AndroidJavaObject currentActivity =
        unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

      // Create the XRAndroid.
      AndroidJavaClass realityEngineClass =
        new AndroidJavaClass("com.the8thwall.reality.app.xr.android.XRAndroid");
      androidXR = realityEngineClass.CallStatic<AndroidJavaObject>("create", currentActivity);
    }

    public void configureXR(XRConfigurationRef config) {
      // Get a ConfigBuilder object.
      AndroidJavaObject configBuilder = new AndroidJavaObject(
        "com.the8thwall.reality.app.xr.android.XRAndroid$XRConfiguration$Builder");

      // Set the configuration requested by this application.
      configBuilder.Call<AndroidJavaObject>(
        "setMask",
         config.ptr.maskLighting,
         config.ptr.maskCamera,
         config.ptr.maskSurfaces);

      configBuilder.Call<AndroidJavaObject>(
        "setGraphicsIntrinsics",
         config.ptr.graphicsIntrinsicsTextureWidth,
         config.ptr.graphicsIntrinsicsTextureHeight,
         config.ptr.graphicsIntrinsicsNearClip,
         config.ptr.graphicsIntrinsicsFarClip,
         config.ptr.graphicsIntrinsicsDigitalZoomHorizontal,
         config.ptr.graphicsIntrinsicsDigitalZoomVertical);

      // Configure the reality engine.
      androidXR.Call("configure", configBuilder.Call<AndroidJavaObject>("build"));
    }

    public void setManagedCameraTexture(System.IntPtr texHandle, int width, int height) {
      androidXR.Call("setUnityManagedCameraTexture", (long) texHandle, width, height);
    }

    public void resume() { androidXR.Call("resume"); }

    public void getCurrentRealityXR(ref XRResponseRef r) {
      AndroidJavaObject jr = androidXR.Call<AndroidJavaObject>("getCurrentRealityXR");
      r.ptr.eventIdTimeMicros = jr.Call<long>("eventIdTimeMicros");

      r.ptr.lightingGlobalExposure = jr.Call<float>("lightingGlobalExposure");

      r.ptr.cameraExtrinsicPositionX = jr.Call<float>("cameraExtrinsicPositionX");
      r.ptr.cameraExtrinsicPositionY = jr.Call<float>("cameraExtrinsicPositionY");
      r.ptr.cameraExtrinsicPositionZ = jr.Call<float>("cameraExtrinsicPositionZ");
      r.ptr.cameraExtrinsicRotationW = jr.Call<float>("cameraExtrinsicRotationW");
      r.ptr.cameraExtrinsicRotationX = jr.Call<float>("cameraExtrinsicRotationX");
      r.ptr.cameraExtrinsicRotationY = jr.Call<float>("cameraExtrinsicRotationY");
      r.ptr.cameraExtrinsicRotationZ = jr.Call<float>("cameraExtrinsicRotationZ");
      r.ptr.cameraIntrinsicMatrix44f = jr.Call<float[]>("cameraIntrinsicMatrix44f");

      r.ptr.surfacesSetSurfacesCount = jr.Call<Int32>("surfacesSetSurfacesCount");
      r.ptr.surfacesSetSurfacesIdTimeMicros = jr.Call<long[]>("surfacesSetSurfacesIdTimeMicros");
      r.ptr.surfacesSetSurfacesFacesBeginIndex =
        jr.Call<Int32[]>("surfacesSetSurfacesFacesBeginIndex");
      r.ptr.surfacesSetSurfacesFacesEndIndex = jr.Call<Int32[]>("surfacesSetSurfacesFacesEndIndex");
      r.ptr.surfacesSetSurfacesVerticesBeginIndex =
        jr.Call<Int32[]>("surfacesSetSurfacesVerticesBeginIndex");
      r.ptr.surfacesSetSurfacesVerticesEndIndex =
        jr.Call<Int32[]>("surfacesSetSurfacesVerticesEndIndex");

      r.ptr.surfacesSetFacesCount = jr.Call<Int32>("surfacesSetFacesCount");
      r.ptr.surfacesSetFaces = jr.Call<Int32[]>("surfacesSetFaces");

      r.ptr.surfacesSetVerticesCount = jr.Call<Int32>("surfacesSetVerticesCount");
      r.ptr.surfacesSetVertices = jr.Call<float[]>("surfacesSetVertices");

      r.ptr.surfacesActiveSurfaceIdTimeMicros = jr.Call<Int64>("surfacesActiveSurfaceIdTimeMicros");
      r.ptr.surfacesActiveSurfaceActivePointX = jr.Call<float>("surfacesActiveSurfaceActivePointX");
      r.ptr.surfacesActiveSurfaceActivePointX = jr.Call<float>("surfacesActiveSurfaceActivePointY");
      r.ptr.surfacesActiveSurfaceActivePointX = jr.Call<float>("surfacesActiveSurfaceActivePointZ");
    }

    public void pause() { androidXR.Call("pause"); }
    public void destroy() { androidXR.Call("destroy"); }
    public void renderFrameForDisplay() { androidXR.Call("renderFrameForDisplay"); }

  }

  private XRAndroid realityEngine;

  void XRCreate() { realityEngine = XRAndroid.create(); }
  void XRConfigure() { realityEngine.configureXR(xrConfig); }
  void XRResume() { realityEngine.resume(); }
  void XRGetCurrentReality() { realityEngine.getCurrentRealityXR(ref xrResponse); }
  void XRPause() { realityEngine.pause(); }
  void XRDestroy() { realityEngine.destroy(); }
  void XRRenderFrameForDisplay() { realityEngine.renderFrameForDisplay(); }

  void XRSetManagedCameraTexture(System.IntPtr texHandle, int width, int height) {
    realityEngine.setManagedCameraTexture(texHandle, width, height);
  }


  // We need these so that we can call DllImport.
  [DllImport ("XRPlugin")]
  private static extern void c8_loadXRDll();
#else

  void XRCreate() { XRCreateEditor(); }
  void XRConfigure() { XRConfigureEditor(); }
  void XRResume() { XRResumeEditor(); }
  void XRGetCurrentReality() { XRGetCurrentRealityEditor(); }
  void XRPause() { XRPauseEditor(); }
  void XRDestroy() { XRDestroyEditor(); }
  void XRSetManagedCameraTexture(System.IntPtr texHandle, int width, int height) {
    XRSetManagedCameraTextureEditor(texHandle, width, height);
  }
  void XRRenderFrameForDisplay() { XRRenderFrameForDisplayEditor(); }

#endif  // UNITY_IPHONE or UNITY_ANDROID

  private bool configured;
  private bool running;
  private XRResponseRef xrResponse;
  private XRConfigurationRef xrConfig;

  public void Create() {
    running = false;
    xrResponse = new XRResponseRef();
    xrConfig = new XRConfigurationRef();
    XRCreate();
  }

  public XRConfigurationRef GetMutableXRConfiguration() { return xrConfig; }

  public void CommitConfiguration() {
    if (running) {
      return;
    }
    XRConfigure();
  }

  public void SetManagedCameraTexture(System.IntPtr texHandle, int width, int height) {
    XRSetManagedCameraTexture(texHandle, width, height);
  }

  public void Resume() {
    if (running) {
      return;
    }
    running = true;
    XRResume();
  }

  public XRResponseRef GetCurrentRealityXR() {
    XRGetCurrentReality();
    return xrResponse;
  }

  public void RenderFrameForDisplay() { XRRenderFrameForDisplay(); }

  public void Pause() {
    if (!running) {
      return;
    }
    XRPause();
    running = false;
  }

  public void Destroy() { XRDestroy(); }
}

}  // namespace XRInternal
