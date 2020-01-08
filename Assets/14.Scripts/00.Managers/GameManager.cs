using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public PlayerController player;
    public Inventory inventory;
    [Header("블록 순서에 일치되게 파티클오브젝트도 배열시킨다")]
    public Particle[] paticles;
    public static GameManager Instance {
        get {
            return instance;
        }
    }
    private static GameManager instance;


    [Header("아이템 코드에 따라 게임 블록들을 할당한다")]
    public GameObject[] itemCodes;
    void Awake() {
        if (GameManager.Instance != null)
        {
            DestroyImmediate(this);
        }
        else
            instance = this;
    }
    
}
