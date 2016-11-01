using UnityEngine;
using Entitas;
using System.Collections.Generic;

public class GridSystem : IExecuteSystem, IInitializeSystem, ISetPool {

    private Pool pool;
    private Entity[][][] gridEntities;
    private Group positionEntitiesGroup;

    private float cellLength = 10;
    private float gridRange = 100;
    private int numberOfCellsPerDimension = 20;

    public void SetDimensions(float range, int numberOfCellsOnThatRange)
    {
        gridRange = range;
        cellLength = gridRange / (float)numberOfCellsOnThatRange;
        numberOfCellsPerDimension = numberOfCellsOnThatRange * 2;
    }

    public void SetPool(Pool _pool)
    {
        pool = _pool;
        positionEntitiesGroup = pool.GetGroup(BoidsMatcher.Position);
    }

    public void Initialize()
    {
        gridEntities = new Entity[numberOfCellsPerDimension][][];
        for (int xIndex = 0; xIndex < numberOfCellsPerDimension; ++xIndex)
        {
            gridEntities[xIndex] = new Entity[numberOfCellsPerDimension][];
            for (int yIndex = 0; yIndex < numberOfCellsPerDimension; ++yIndex)
            {
                gridEntities[xIndex][yIndex] = new Entity[numberOfCellsPerDimension];
                for (int zIndex = 0; zIndex < numberOfCellsPerDimension; ++zIndex)
                {
                    var gridEntity = pool.CreateEntity();
                    gridEntity.AddGridCell(new List<Entity>(), null);
                    gridEntities[xIndex][yIndex][zIndex] = gridEntity;
                }
            }
        }
        
        for (int xIndex = 0; xIndex < numberOfCellsPerDimension; ++xIndex)
        {
            for (int yIndex = 0; yIndex < numberOfCellsPerDimension; ++yIndex)
            {
                for (int zIndex = 0; zIndex < numberOfCellsPerDimension; ++zIndex)
                {
                    gridEntities[xIndex][yIndex][zIndex].gridCell.NeighboringCells = GetNeighboringCells(xIndex, yIndex, zIndex);
                }
            }
        }
    }

    public void Execute()
    {
        // put all entities in the right cell
        var posEntities = positionEntitiesGroup.GetEntities();
        for (int posIndex = 0; posIndex < posEntities.Length; ++posIndex)
        {
            Vector3 position = posEntities[posIndex].position.Position;
            
            int xIndex;
            int yIndex;
            int zIndex;
            GetCoordinates(position, out xIndex, out yIndex, out zIndex);

            if( xIndex < 0 || xIndex >= numberOfCellsPerDimension ||
                yIndex < 0 || yIndex >= numberOfCellsPerDimension ||
                zIndex < 0 || zIndex >= numberOfCellsPerDimension)
            {
                Debug.LogError("[GridSystem]: Entity has left the grid range!!");
                continue;
            }

            // if the entity was in a cell before, remove it from that cells list!
            // ...also detect if the cell didn't change for this entity and do nothing.
            if(posEntities[posIndex].hasContainedInCell)
            {
                var containedInCell = posEntities[posIndex].containedInCell;
                if (containedInCell.ContainedInCell == gridEntities[xIndex][yIndex][zIndex])
                    // cells didn't change, so:
                    continue;
                containedInCell.ContainedInCell.gridCell.ContainsEntities.Remove(posEntities[posIndex]);
            }

            gridEntities[xIndex][yIndex][zIndex].gridCell.ContainsEntities.Add(posEntities[posIndex]);

            posEntities[posIndex].ReplaceContainedInCell(
                newContainedInCell: gridEntities[xIndex][yIndex][zIndex]);
        }
    }

    private int[] numberOfNeigborCellsOnEdgeCoordinates = { 26, 17, 11, 7 };

    public Entity[] GetNeighboringCells(int xIndex, int yIndex, int zIndex)
    {
        int numberOfEdgeCoordinates = 0;
        if (xIndex == 0 || xIndex == numberOfCellsPerDimension - 1)
            numberOfEdgeCoordinates += 1;
        if (yIndex == 0 || yIndex == numberOfCellsPerDimension - 1)
            numberOfEdgeCoordinates += 1;
        if (zIndex == 0 || zIndex == numberOfCellsPerDimension - 1)
            numberOfEdgeCoordinates += 1;

        int numberOfNeighborCells = numberOfNeigborCellsOnEdgeCoordinates[numberOfEdgeCoordinates];
        var neighborEntities = new Entity[numberOfNeighborCells];
        int neighborArrayIndex = 0;
        for (int xOffset = 0; xOffset < 3; ++xOffset)
        {
            int xNeighbor = xIndex + xOffset - 1;
            if (xNeighbor >= 0 && xNeighbor < numberOfCellsPerDimension)
            {
                for (int yOffset = 0; yOffset < 3; ++yOffset)
                {
                    int yNeighbor = yIndex + yOffset - 1;
                    if (yNeighbor >= 0 && yNeighbor < numberOfCellsPerDimension)
                    {
                        for (int zOffset = 0; zOffset < 3; ++zOffset)
                        {
                            int zNeighbor = zIndex + zOffset - 1;
                            if(zNeighbor >= 0 && zNeighbor < numberOfCellsPerDimension &&
                                !(xIndex == xNeighbor && yIndex == yNeighbor && zIndex == zNeighbor))
                            {
                                neighborEntities[neighborArrayIndex] = gridEntities[xNeighbor][yNeighbor][zNeighbor];
                                neighborArrayIndex += 1;
                            }
                        }
                    }
                }
            }
        }

        return neighborEntities;
    }

    public void GetCoordinates(Vector3 position, out int xIndex, out int yIndex, out int zIndex)
    {
        xIndex = Mathf.FloorToInt((gridRange + position.x) / cellLength);
        yIndex = Mathf.FloorToInt((gridRange + position.y) / cellLength);
        zIndex = Mathf.FloorToInt((gridRange + position.z) / cellLength);
    }

    public Entity[][][] GridEntities { get { return gridEntities; } }

    public Entity GetGridCellEntity(int xIndex, int yIndex, int zIndex)
    {
        return gridEntities[xIndex][yIndex][zIndex];
    }

    public float CellLength { get { return cellLength; } }
}
