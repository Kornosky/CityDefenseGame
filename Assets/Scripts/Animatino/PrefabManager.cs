using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    // player
    [Header("player")]
    public GameObject[] playerBodyPrefabs;
    public GameObject[] playerHeadPrefabs;

    // enemy
    [Header("enemy")]
    public GameObject[] enemyBodyPrefabs;
    public GameObject[] enemyHeadPrefabs;

    // kid
    [Header("kid")]
    public GameObject[] kidBodyPrefabs;
    public GameObject[] kidHeadPrefabs;

    // particles
    [Header("particles")]
    public GameObject[] dustPrefabs;
    public GameObject[] wandParticlePrefabs;
    public GameObject[] dustSlidePrefabs;
    public GameObject[] whiteOrbPrefabs;

    // keys
    [Header("keys")]
    public GameObject[] keyPrefabs;

    // stars
    [Header("stars")]
    public GameObject[] starPrefabs;

    // wand
    [Header("wand")]
    public GameObject[] wandPrefabs;

    void Awake()
    {
        instance = this;
    }

    public void SpawnPrefab(GameObject _o, Vector3 _p, Quaternion _r, float _scl)
    {
        if (_o != null)
        {
            GameObject ret = Instantiate(_o, _p, _r) as GameObject;
            Transform tr = ret.transform;
            tr.localScale = Vector3.one * _scl;
        }
    }

    public GameObject SpawnPrefabAsGameObject ( GameObject _o, Vector3 _p, Quaternion _r, float _scl )
    {
        GameObject ret = null;

        if ( _o != null )
        {
            ret = Instantiate(_o,_p,_r) as GameObject;
            Transform tr = ret.transform;
            tr.localScale = Vector3.one * _scl;
        }

        return ret;
    }
}
