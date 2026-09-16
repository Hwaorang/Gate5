using UnityEngine;

/// <summary>
/// Gate 효과를 처리하는 Strategy.
/// </summary>
public interface IGateOperationStrategy
{
    void Apply(
        IGateReceiver receiver,
        int value
    );


    int ModifyValueByBullet(
        int currentValue,
        int changePerHit
    );


    string GetDisplayText(
        int value
    );
}


// ========================================================
// Add
// ========================================================

public class AddGateStrategy :
    IGateOperationStrategy
{
    public void Apply(
        IGateReceiver receiver,
        int value)
    {
        receiver.AddUnits(
            value
        );
    }


    public int ModifyValueByBullet(
        int currentValue,
        int changePerHit)
    {
        return currentValue +
               changePerHit;
    }


    public string GetDisplayText(
        int value)
    {
        return $"+{value}";
    }
}


// ========================================================
// Subtract
// ========================================================

public class SubtractGateStrategy :
    IGateOperationStrategy
{
    public void Apply(
        IGateReceiver receiver,
        int value)
    {
        receiver.RemoveUnits(
            value
        );
    }


    public int ModifyValueByBullet(
        int currentValue,
        int changePerHit)
    {
        return Mathf.Max(
            0,
            currentValue - changePerHit
        );
    }


    public string GetDisplayText(
        int value)
    {
        return $"-{value}";
    }
}


// ========================================================
// Multiply
// ========================================================

public class MultiplyGateStrategy :
    IGateOperationStrategy
{
    public void Apply(
        IGateReceiver receiver,
        int value)
    {
        if (value <= 1)
        {
            return;
        }


        int currentCount =
            receiver.CurrentUnitCount;


        // ×2라면 현재 인원의
        // 1배를 추가한다.
        //
        // 10명 ×2
        // → 기존 10 + 추가 10
        int addAmount =
            currentCount *
            (value - 1);


        receiver.AddUnits(
            addAmount
        );
    }


    public int ModifyValueByBullet(
        int currentValue,
        int changePerHit)
    {
        return Mathf.Max(
            1,
            currentValue + changePerHit
        );
    }


    public string GetDisplayText(
        int value)
    {
        return $"×{value}";
    }
}


// ========================================================
// Factory
// ========================================================

public static class GateStrategyFactory
{
    private static readonly
        AddGateStrategy addStrategy =
            new AddGateStrategy();

    private static readonly
        SubtractGateStrategy subtractStrategy =
            new SubtractGateStrategy();

    private static readonly
        MultiplyGateStrategy multiplyStrategy =
            new MultiplyGateStrategy();


    public static IGateOperationStrategy GetStrategy(
        GateOperationType type)
    {
        switch (type)
        {
            case GateOperationType.Add:

                return addStrategy;


            case GateOperationType.Subtract:

                return subtractStrategy;


            case GateOperationType.Multiply:

                return multiplyStrategy;


            default:

                Debug.LogError(
                    $"지원하지 않는 Gate 타입 : {type}"
                );

                return addStrategy;
        }
    }
}