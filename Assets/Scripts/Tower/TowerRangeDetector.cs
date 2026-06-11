using UnityEngine;

public class TowerRangeDetector : MonoBehaviour
{
    [SerializeField]
    private TowerBase tower;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            tower.AddEnemy(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            tower.RemoveEnemy(enemy);
        }
    }
}