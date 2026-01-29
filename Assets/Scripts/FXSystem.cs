using UnityEngine;

public class FXSystem : MonoBehaviour
{

    private static FXSystem instance;


    [Header("References")]
    [SerializeField] private GameObject[] effectPrefabs;

    public enum FxId
    {
        LASER, DOUBLE_LASER, TRIPLE_LASER, SHURIKENS, ANNIHILATE, SPLIT, ENERGY_BALL, FIRE_SHOT, MANABOMB, SHADOW_STRIKE, REPLICATE, FIRE_BALL, THROW_SLASH, LIFESTEAL
    }

    void Start()
    {
        if (instance)
        {
            Debug.Log("Destroy old FX System instance");
            Destroy(instance);
        }
        Debug.Log("Set new FX System instance");
        instance = this;
    }

    public static void SpawnEffect(FxId fx)
    {
        SpawnEffect(fx, Vector2.zero, false);
    }

    public static void SpawnEffect(FxId fx, Vector2 offset, bool mirrored)
    {
        if (instance == null)
        {
            Debug.LogError("FX System not available");
            return;
        }
        int idx = ((int)fx);
        if (idx >= 0 && idx < instance.effectPrefabs.Length)
        {
            GameObject instance = Instantiate(FXSystem.instance.effectPrefabs[idx]);
            float x = (mirrored ? -1*instance.transform.position.x : instance.transform.position.x) + offset.x;
            float y = instance.transform.position.y + offset.y;
            float z = instance.transform.position.z;
            instance.transform.position = new Vector3(x,y,z);
            instance.transform.rotation = mirrored ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Invalid FxId: " + fx);
        }

    }
}
