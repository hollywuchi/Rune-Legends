using UnityEngine;

public abstract class BtBaseNode : ScriptableObject
{
    [HideInInspector] public BtLifeTimeState State = BtLifeTimeState.Running;
    [HideInInspector] public bool Started = false;
    [HideInInspector] public string Guid;
    [HideInInspector] public EnemyContext context;
    [HideInInspector] public Blackboard blackBoard;
    [HideInInspector] public Vector2 position;

    [TextArea] public string descrpition;

    public BtLifeTimeState Evalucate()
    {
        if (!Started)
        {
            OnStart();
            Started = true;
        }

        State = OnUpdate();
        if (State == BtLifeTimeState.Failure || State == BtLifeTimeState.Success)
        {
            OnStop();
            Started = false;
        }

        return State;
    }
    
    /// <summary>
    /// 防止重复创建SO文件
    /// </summary>
    /// <returns></returns>
    public virtual BtBaseNode Clone()
    {
        return Instantiate(this);
    }
    
    protected abstract void OnStart();
    protected abstract BtLifeTimeState OnUpdate();
    protected abstract void OnStop();
}