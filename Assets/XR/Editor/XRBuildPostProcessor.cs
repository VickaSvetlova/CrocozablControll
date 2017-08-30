using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class XRBuildPostProcessor {
  [PostProcessBuild]
  public static void XcodeProjectSettings(BuildTarget buildTarget, string pathToBuiltProject) {
    if (buildTarget == BuildTarget.iOS) {
      string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
      PBXProject proj = new PBXProject();
      proj.ReadFromFile(projPath);
      string unityTarget = proj.TargetGuidByName("Unity-iPhone");
      proj.AddFrameworkToProject(unityTarget, "Accelerate.framework", true);
      proj.AddFrameworkToProject(unityTarget, "ARKit.framework", true);
      proj.AddFrameworkToProject(unityTarget, "CoreVideo.framework", true);
      proj.AddFrameworkToProject(unityTarget, "AVFoundation.framework", true);
      proj.AddFrameworkToProject(unityTarget, "CoreMotion.framework", true);
      proj.AddFrameworkToProject(unityTarget, "UIKit.framework", true);
      proj.AddFrameworkToProject(unityTarget, "CoreGraphics.framework", true);
      proj.AddFrameworkToProject(unityTarget, "CoreImage.framework", true);
      proj.AddFrameworkToProject(unityTarget, "Metal.framework", true);
      proj.AddFrameworkToProject(unityTarget, "CoreMedia.framework", true);
      proj.AddFrameworkToProject(unityTarget, "OpenGLES.framework", true);
      proj.WriteToFile(projPath);
    }
  }
}
