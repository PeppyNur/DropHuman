using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorMaterials", menuName = "Scriptable Objects/ColorMaterials")]
public class ColorMaterialsSO : ScriptableObject
{
    public List<Material> blockColors;
}
