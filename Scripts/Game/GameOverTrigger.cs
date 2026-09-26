using UnityEngine;

public class GameOverTrigger : MonoBehaviour
{
    GameManager gameManager;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Pinata pinata = other.GetComponent<Pinata>();
        if (pinata != null)
            gameManager.TriggerGameOver();
    }
}