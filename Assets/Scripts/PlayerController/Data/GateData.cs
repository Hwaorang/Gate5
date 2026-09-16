using UnityEngine;

[CreateAssetMenu(
    fileName = "GateData",
    menuName = "Game/Gate/Gate Data"
)]
public class GateData : ScriptableObject
{
    [Header("Gate Operation")]

    [SerializeField]
    private GateOperationType operationType;


    [Header("Value")]

    [SerializeField]
    [Min(0)]
    private int initialValue = 1;

    [SerializeField]
    [Min(0)]
    private int changePerHit = 1;


    public GateOperationType OperationType =>
        operationType;

    public int InitialValue =>
        initialValue;

    public int ChangePerHit =>
        changePerHit;
}