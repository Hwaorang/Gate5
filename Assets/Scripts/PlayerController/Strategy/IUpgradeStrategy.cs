public interface IUpgradeStrategy
{
    /// <summary>
    /// 전달받은 강화 값을 실제 시스템에 적용한다.
    /// </summary>
    void Apply(float value);
}