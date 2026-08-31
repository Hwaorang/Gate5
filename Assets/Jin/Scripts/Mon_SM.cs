using UnityEngine;

public interface MonState<Mon_Ctrl>
{
    public void Enter();

    public void Update();

    public void Exit();
}

public class Mon_SM<Mon_Ctrl>
{
    protected MonState<Mon_Ctrl> currentState;

    Mon_Ctrl obj;
    public Mon_SM(Mon_Ctrl _obj)
    {
        obj = _obj;
    }

    public void ChangeState(MonState<Mon_Ctrl> state)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = state;

        currentState.Enter();
    }
}
