public class MoveSpeedUpgradeStrategy : IUpgradeStrategy
{
    private readonly PlayerController playerController;

    public MoveSpeedUpgradeStrategy(
        PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void Apply(float value)
    {
        playerController.UpgradeMoveSpeed(value);
    }
}