using UnityEngine;

public class EnemyTarget : MonoBehaviour
{
    private bool _isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (_isActive && other.CompareTag("Player"))
        {
            _isActive = false;
            GameManager.Instance.AddScore(1);
        }
    }
}