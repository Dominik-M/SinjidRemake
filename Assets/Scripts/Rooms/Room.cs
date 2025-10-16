using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Scriptable Objects/Room")]
public class Room : ScriptableObject
{
    public RoomID id;
    public Room left, right, up, down;
    public GameObject prefab;
}
