using UnityEngine;

public class XRCameraController : MonoBehaviour {
  public const float METERS_SCALE = 1.0f;
  public const float FEET_SCALE = 3.28084f;

  private XRController xr;
  private Camera sceneCamera;

  // XRCameraController.scale allows for scaling the effective units of a scene. For example, if
  // feet is a more natural unit for a scene than meters, set scale to 3.28084f.
  public float scale = METERS_SCALE;

  void Start() {
    xr = GameObject.FindWithTag("XRController").GetComponent<XRController>();
    sceneCamera = GetComponent<Camera>();
    xr.UpdateCameraProjectionMatrix(sceneCamera, transform.position, scale);
  }

  void Update () {
    transform.position = xr.GetCameraPosition();
    transform.rotation = xr.GetCameraRotation();
    sceneCamera.projectionMatrix = xr.GetCameraIntrinsics();
  }
}
