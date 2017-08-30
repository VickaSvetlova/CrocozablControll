using UnityEngine;

// Controller for Unity Light objects, adjusting the illumination based on observations from the scene.
public class XRLightController : MonoBehaviour {
  private XRController xr;
  private Light sceneLight;

  void Start() {
    sceneLight = GetComponent<Light>();
    xr = GameObject.FindWithTag("XRController").GetComponent<XRController>();
  }

  void Update () {
    // Update the light exposure.
    float exposure = xr.GetLightExposure();

    // Exposure ranges from -1 to 1 in XR, adjust to 0-2 for Unity.
    sceneLight.intensity = exposure + 1.0f;
  }
}
