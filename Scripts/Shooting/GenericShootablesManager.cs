using UnityEngine;

public class GenericShootablesManager : AShootablesManager
{
    [SerializeField] private GameObject[] shootablePrefabs;

    public override AShootable GenerateShootable()
    {
        GameObject shootablePrefab = shootablePrefabs[Random.Range(0, shootablePrefabs.Length)];
        GameObject shootableGameObject = ObjectPoolingManager.Instance.SpawnFromPool(shootablePrefab.tag, Vector3.zero, Quaternion.identity).gameObject;
        AShootable shootable = shootableGameObject.GetComponent<AShootable>();
        return shootable;
    }
}