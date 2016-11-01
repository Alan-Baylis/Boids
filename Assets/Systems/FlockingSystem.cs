using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class FlockingSystem : IExecuteSystem, ISetPool
{
    public float Deltatime = 1;
    public float MaxDistToCenter = 90;
    public float NormalVelocity = 10;
    public float TurningSpeed = 0.5f;
    public float MaxNeighborDistance = 10;
    public int MaxNeighbors = 10;


    // weights for the different direction rules
    // HAVE TO BE SET TO WORK!
    public float PreserveExistingDirectionWeight = 0;
    public float MaxDistToCenterWeight = 0;
    public float AlignWithNeighborsWeight = 0;

    

    private Group allBoidEntities;
    private Pool pool;
    private List<VelocityComponent> neighborList = new List<VelocityComponent>();

    public void SetPool(Pool _pool)
    {
        allBoidEntities = _pool.GetGroup(
            Matcher.AllOf(BoidsMatcher.Boid, BoidsMatcher.Position, BoidsMatcher.Velocity));
    }

    public void Execute()
    {
        foreach (var boidEntity in allBoidEntities.GetEntities())
        {
            // we could put each of those rules in different systems,
            // so that we could switch them off independently, but
            // that would probably limit the performance.

            // fixed velocity magnitude
            boidEntity.velocity.Magnitude = NormalVelocity;

            // accumulate all direction changes
            Vector3 newDirAccumulator = Vector3.zero;


            // don't fly too far off the center
            float distToCenterSq = boidEntity.position.Position.sqrMagnitude;
            if(distToCenterSq > MaxDistToCenter * MaxDistToCenter * (2f/3f) && MaxDistToCenterWeight > 0){
                float distToCenter = boidEntity.position.Position.magnitude;
                Vector3 dirToCenter = -boidEntity.position.Position / distToCenter;
                float dirCorrectionFactor = Mathf.InverseLerp(MaxDistToCenter * (2f/3f), MaxDistToCenter, distToCenter);
                dirCorrectionFactor *= MaxDistToCenterWeight;
                newDirAccumulator += dirCorrectionFactor * dirToCenter;
            } 

            if(AlignWithNeighborsWeight > 0){
                var boidEntityCell = boidEntity.containedInCell.ContainedInCell.gridCell;
                CollectNeighbors(boidEntity.position.Position, boidEntityCell, ref neighborList);
                for(int neighborCellIndex=0; neighborCellIndex < boidEntityCell.NeighboringCells.Length; ++neighborCellIndex){
                    var neighborCell = boidEntityCell.NeighboringCells[neighborCellIndex];
                    CollectNeighbors(boidEntity.position.Position, neighborCell.gridCell, ref neighborList);
                    if(neighborList.Count > MaxNeighbors)
                        break;
                }
                if(neighborList.Count > 0){
                    Vector3 averageNeigborVel = Vector3.zero;
                    for(int neighborIndex=0; neighborIndex < neighborList.Count; ++neighborIndex){
                        averageNeigborVel += neighborList[neighborIndex].Direction;
                    }
                    averageNeigborVel /= neighborList.Count;
                    newDirAccumulator += averageNeigborVel * AlignWithNeighborsWeight;
                }
                neighborList.Clear();
            }

            // preserve existing direction
            newDirAccumulator += PreserveExistingDirectionWeight * boidEntity.velocity.Direction;

            // normalize accumulated directions
            newDirAccumulator.Normalize();
            Vector3 directionDiff = newDirAccumulator - boidEntity.velocity.Direction;
            if(directionDiff.sqrMagnitude > 0.01f){
                directionDiff.Normalize(); 
                boidEntity.velocity.Direction += directionDiff * TurningSpeed * Deltatime;
            }

            // update position
            boidEntity.position.Position += 
                boidEntity.velocity.Direction * boidEntity.velocity.Magnitude * Deltatime;
        }
    }

    private float DistanceSq(Vector3 p1, Vector3 p2){
        return (p1-p2).sqrMagnitude;
    }

    private void CollectNeighbors(Vector3 position, GridCellComponent cell, ref List<VelocityComponent> fillList){
        for(int entityIndex=0; entityIndex < cell.ContainsEntities.Count; ++entityIndex){
            var entity = cell.ContainsEntities[entityIndex];
            if(entity.hasVelocity && DistanceSq(entity.position.Position, position) < MaxNeighborDistance){
                fillList.Add(entity.velocity);
                if(fillList.Count >= MaxNeighbors)
                    break;
            }
        }
    }
}