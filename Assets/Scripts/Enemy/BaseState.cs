public abstract class BaseState  
{
    public Enemy CurrentEnemy;
    public abstract void OnEnter(Enemy enemy);
    public abstract void LogicUpdate();
    public abstract void PhysicsUpdate();
    public abstract void OnExit();
}
