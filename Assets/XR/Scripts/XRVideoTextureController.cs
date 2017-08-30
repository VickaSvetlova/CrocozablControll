using UnityEngine;

public class XRVideoTextureController : MonoBehaviour {
  private XRController xr;

  void Start() {

    xr = GameObject.FindWithTag("XRController").GetComponent<XRController>();

    // Set reality texture onto our material. Make sure it's unlit to avoid appearing washed out.
    // Note that this requires Unlit/Texture to be included in the unity project.
    Renderer r = GetComponent<Renderer>();
    r.material.shader = Shader.Find("Unlit/Texture");
    r.material.mainTexture = xr.GetRealityTexture();
  }
}
