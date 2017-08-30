using System;
using System.Runtime.InteropServices;

namespace XRInternal {

[StructLayout(LayoutKind.Sequential)]
public struct c8_XRResponse {
  // NOTE: This struct must be kept exactly in sync with its equivalent in xr-extern.h.

  // Always present.
  public long eventIdTimeMicros;

  // NOTE: This struct must be kept exactly in sync with its equivalent in xr-extern.h.

  // Output when "maskLighting" in configured.
  public float lightingGlobalExposure;

  // NOTE: This struct must be kept exactly in sync with its equivalent in xr-extern.h.

  // Output when "maskCamera" in configured.
  public float cameraExtrinsicPositionX;
  public float cameraExtrinsicPositionY;
  public float cameraExtrinsicPositionZ;
  public float cameraExtrinsicRotationW;
  public float cameraExtrinsicRotationX;
  public float cameraExtrinsicRotationY;
  public float cameraExtrinsicRotationZ;

  [MarshalAs(UnmanagedType.ByValArray, SizeConst=ApiLimits.MATRIX44)]
  public float[] cameraIntrinsicMatrix44f;

  // NOTE: This struct must be kept exactly in sync with its equivalent in xr-extern.h.

  // Output when "maskSurfaces" in configured.
  public Int32 surfacesSetSurfacesCount;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst=ApiLimits.MAX_SURFACES)]
  public Int64[] surfacesSetSurfacesIdTimeMicros;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst=ApiLimits.MAX_SURFACES)]
  public Int32[] surfacesSetSurfacesFacesBeginIndex;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst=ApiLimits.MAX_SURFACES)]
  public Int32[] surfacesSetSurfacesFacesEndIndex;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst=ApiLimits.MAX_SURFACES)]
  public Int32[] surfacesSetSurfacesVerticesBeginIndex;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst=ApiLimits.MAX_SURFACES)]
  public Int32[] surfacesSetSurfacesVerticesEndIndex;

  public Int32 surfacesSetFacesCount;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst=ApiLimits.MAX_SURFACE_FACES * 3)]
  public Int32[] surfacesSetFaces;

  public Int32 surfacesSetVerticesCount;
  [MarshalAs(UnmanagedType.ByValArray, SizeConst=ApiLimits.MAX_SURFACE_VERTICES * 3)]
  public float[] surfacesSetVertices;

  public Int64 surfacesActiveSurfaceIdTimeMicros;
  public float surfacesActiveSurfaceActivePointX;
  public float surfacesActiveSurfaceActivePointY;
  public float surfacesActiveSurfaceActivePointZ;

  // NOTE: This struct must be kept exactly in sync with its equivalent in xr-extern.h.

  // Default initialize arrays to expected length.
  public c8_XRResponse(int unused) : this() {
    cameraIntrinsicMatrix44f = new float[ApiLimits.MATRIX44];

    surfacesSetSurfacesIdTimeMicros = new long[ApiLimits.MAX_SURFACES];
    surfacesSetSurfacesFacesBeginIndex = new Int32[ApiLimits.MAX_SURFACES];
    surfacesSetSurfacesFacesEndIndex = new Int32[ApiLimits.MAX_SURFACES];
    surfacesSetSurfacesVerticesBeginIndex = new Int32[ApiLimits.MAX_SURFACES];
    surfacesSetSurfacesVerticesEndIndex = new Int32[ApiLimits.MAX_SURFACES];

    surfacesSetFaces =  new Int32[ApiLimits.MAX_SURFACE_FACES * 3];

    surfacesSetVertices = new float[ApiLimits.MAX_SURFACE_VERTICES * 3];
  }
};

[StructLayout(LayoutKind.Sequential)]
public struct c8_XRConfiguration {
  // NOTE: This struct must be kept exactly in sync with its equivalent in xr-extern.h.

  // Specifies data requested from XR.
  [MarshalAs(UnmanagedType.U1)] public bool maskLighting;
  [MarshalAs(UnmanagedType.U1)] public bool maskCamera;
  [MarshalAs(UnmanagedType.U1)] public bool maskSurfaces;

  // NOTE: This struct must be kept exactly in sync with its equivalent in xr-extern.h.

  // Specifies the graphics context for displaying XR data.
  public Int32 graphicsIntrinsicsTextureWidth;
  public Int32 graphicsIntrinsicsTextureHeight;
  public float graphicsIntrinsicsNearClip;
  public float graphicsIntrinsicsFarClip;
  public float graphicsIntrinsicsDigitalZoomHorizontal;
  public float graphicsIntrinsicsDigitalZoomVertical;
};

public class XRResponseRef {
  public c8_XRResponse ptr = new c8_XRResponse(0);
};

public class XRConfigurationRef {
  public c8_XRConfiguration ptr = new c8_XRConfiguration();
};

}  // namespace XRInternal
