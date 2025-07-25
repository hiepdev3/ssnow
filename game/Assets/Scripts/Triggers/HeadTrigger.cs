using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HeadTrigger : MonoBehaviour
{
    public BoolVariable IsAlive;

    [Tooltip("Event invoked when collision occurs.")]
    public UnityEvent HeadCollisionEvent;

    [Tooltip("GameObjects to interact with.")]
    public GameObject[] TriggerCandidates;

    private HashSet<GameObject> triggerCandidates;

    private void Awake()
    {
        this.triggerCandidates = new HashSet<GameObject>(this.TriggerCandidates);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.triggerCandidates.Contains(other.gameObject) && this.IsAlive.Value)
        {
            this.HeadCollisionEvent.Invoke();
            HUDTextController hud = FindObjectOfType<HUDTextController>();
            if (hud != null)
            {
                GameData.Distance = hud.GetDistance();
                GameData.Coins = hud.GetCoins();
            }
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
