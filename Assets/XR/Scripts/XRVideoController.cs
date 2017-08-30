using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using XRInternal;

public class XRVideoController : MonoBehaviour {

  private XRController xr;

  private Material xrMat;
  private CommandBuffer buffer;
  private bool isCBInit;
  private Camera cam;

  public void Start() {
    xr = GameObject.FindWithTag("XRController").GetComponent<XRController>();
    cam = GetComponent<Camera>();
    cam.clearFlags = CameraClearFlags.Depth;
    isCBInit = false;
    xrMat = new Material(Shader.Find("Unlit/XRCameraShader"));
  }

  void OnDestroy() {
    GetComponent<Camera>().RemoveCommandBuffer(CameraEvent.BeforeForwardOpaque, buffer);
  }

  public void OnPreRender() {
    if (!isCBInit) {
      buffer = new CommandBuffer();
      buffer.Blit(null, BuiltinRenderTextureType.CurrentActive, xrMat);
      cam.AddCommandBuffer(CameraEvent.BeforeForwardOpaque, buffer);
      isCBInit = true;
    }

    xrMat.mainTexture = xr.GetRealityTexture();

    Resolution currentRes = Screen.currentResolution;
    Matrix4x4 mWarp = Matrix4x4.identity;

    float scaleFactor =
      ( (float)currentRes.width / (float)currentRes.height ) /
      ( (float)ApiLimits.IMAGE_PROCESSING_WIDTH / (float)ApiLimits.IMAGE_PROCESSING_HEIGHT );

    if ( scaleFactor > 1 ) {
      float invScaleFactor = 1.0f / scaleFactor;
      mWarp[1, 1] = invScaleFactor;
      mWarp[1, 3] = invScaleFactor * invScaleFactor * .25f;
    } else if ( scaleFactor <= 1 ) {
      mWarp[0, 0] = scaleFactor;
      mWarp[0, 3] = scaleFactor * scaleFactor * .25f;
    }

    xrMat.SetMatrix("_TextureWarp", mWarp);
  }
}
