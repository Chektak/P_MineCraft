using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public static GameManager Instance {
        get {
            return instance;
        }
    }
    private static GameManager instance;

    void Awake() {
        if (GameManager.Instance != null)
        {
            DestroyImmediate(this);
        }
        else
            instance = this;
    }
    
}
