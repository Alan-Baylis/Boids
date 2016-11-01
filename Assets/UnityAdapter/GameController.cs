using Entitas;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Systems systems;

    private void Start()
    {
        var pools = Pools.sharedInstance;
        pools.SetAllPools();

        systems = CreateSystems(pools);
        systems.Initialize();
    }

    private void OnDestroy()
    {
        systems.TearDown();
    }

    private void FixedUpdate()
    {
        systems.Execute();
        systems.Cleanup();
    }

    private Systems CreateSystems(Pools pools)
    {
        return new Feature("Systems").
            Add(pools.boids.CreateSystem(new CreateBoidsSystem() {
                NumberOfBoids = 200})).
            Add(pools.boids.CreateSystem(new GridSystem())).

            Add(pools.boids.CreateSystem(new FlockingSystem() { 
                PreserveExistingDirectionWeight = 1,
                MaxDistToCenterWeight = 5,
                AlignWithNeighborsWeight = 5,
                Deltatime = Time.fixedDeltaTime})).

            Add(pools.boids.CreateSystem(new DebugGizmoBoidViewSystem()));
    }
}