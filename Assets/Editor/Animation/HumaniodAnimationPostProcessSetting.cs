using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "HumaniodAnimationPostProcessSetting")]
public class HumaniodAnimationPostProcessSetting : ScriptableObject
{
    public Avatar referenceAvatar;
    public Object[] arrDir;
    public string[] loopAnims;
    public AvatarMask animMask;
    public string[] removePrev;
}
