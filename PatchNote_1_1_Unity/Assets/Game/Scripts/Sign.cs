using UnityEngine;

public class Sign : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SignLibrary m_signLibrary;
    [SerializeField] private Material m_itemMaterial;
    
    [Header("Components")]
    [SerializeField] private MeshRenderer m_meshRenderer;
    
    private void Start()
    {
        Material m = m_signLibrary.GetRandom();
        m_meshRenderer.materials = new Material[] { m_itemMaterial, m };
    }
}
