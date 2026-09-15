public interface IGateReceiver
{
    /// <summary>
    /// 현재 살아있는 유닛 수.
    /// Multiply Gate 계산에 사용한다.
    /// </summary>
    int CurrentUnitCount
    {
        get;
    }


    /// <summary>
    /// 유닛 추가.
    /// </summary>
    void AddUnits(
        int amount
    );


    /// <summary>
    /// 유닛 제거.
    /// </summary>
    void RemoveUnits(
        int amount
    );
}