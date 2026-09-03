using System;

[Serializable]
public class PlayerData
{
    // =========================
    // ��ȭ
    // =========================

    public int gold = 1000;


    // =========================
    // ������ ��Ų
    // =========================

    public int selectedSkin = 0;


    // =========================
    // ���̵�
    // =========================

    // 0 = Easy
    // 1 = Normal
    // 2 = Hard

    public int selectedDifficulty = 1;


    // =========================
    // �÷��̾� ��ȭ
    // =========================

    // ���ݷ�
    public int attackLevel = 0;

    // �̵��ӵ�
    public int speedLevel = 0;

    // ���ݼӵ�
    public int attackSpeedLevel = 0;

    // �������� Ŭ���� ��� ȹ�淮
    public int goldRewardLevel = 0;
}