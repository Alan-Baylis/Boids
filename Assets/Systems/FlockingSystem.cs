using Entitas;
using UnityEngine;

public class FlockingSystem : IExecuteSystem, ISetPool
{

    private Group allBoidEntities;

    public void SetPool(Pool pool)
    {
        allBoidEntities = pool.GetGroup(Matcher.AllOf(BoidsMatcher.Boid, BoidsMatcher.Position, BoidsMatcher.Velocity));
    }

    public void Execute()
    {
        foreach (var boidEntity in allBoidEntities.GetEntities())
        {
            // we could put each of those rules in different systems,
            // so that we could switch them off independently, but
            // that would probably limit the performance.
            boidEntity.position.Position += boidEntity.velocity.Direction * boidEntity.velocity.Magnitude * Time.deltaTime;
        }
    }

    
}