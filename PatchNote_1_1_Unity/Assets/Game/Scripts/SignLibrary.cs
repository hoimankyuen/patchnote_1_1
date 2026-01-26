using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Sign Library", fileName = "SignLibrary")]
public class SignLibrary : ScriptableObject
{
    [SerializeField] private List<Material> materials;

    public Material GetRandom()
    {
        return materials[Random.Range(0, materials.Count)];
    }
}
