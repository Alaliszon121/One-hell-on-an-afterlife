using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Movement przeciwników")]
    [SerializeField] public float walkSpeed = 1f;
    [SerializeField] public float runSpeed = 3f;
    [SerializeField] public Transform spawnPoint;
    
    public static GameManager instance;
    
    public PlayerStatus playerStatus =  new PlayerStatus();

    void Awake()
    {
        instance = this;
    }
}
