using UnityEngine;

[CreateAssetMenu(fileName = "ItemContainer", menuName = "Scriptable Objects/ItemContainer")]
public class ItemContainer : ScriptableObject
{
    public Item[] items;
}
