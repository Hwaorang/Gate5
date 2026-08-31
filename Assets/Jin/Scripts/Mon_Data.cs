using UnityEngine;

[CreateAssetMenu(fileName = "Mon_Data", menuName = "Scriptable Objects/Mon_Data")]
public class Mon_Data : ScriptableObject
{
    public string monName;

    public float maxHp;
    public float damage;
    public float walkSpeed;
}
