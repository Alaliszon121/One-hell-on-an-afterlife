using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public PlayerStatus playerStatus =  new PlayerStatus();

    void Awake()
    {
        instance = this;
    }
}
