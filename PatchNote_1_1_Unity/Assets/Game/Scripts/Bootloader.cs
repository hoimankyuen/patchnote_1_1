using System.Collections;
using UnityEngine;

public class Bootloader : MonoBehaviour
{
    // ================ Unity Messages ================
    
    private IEnumerator Start()
    {
        yield return null;
        LevelManager.Instance.GotoMainMenu();
    }
}