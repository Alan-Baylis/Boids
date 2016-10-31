using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Entitas;
using NUnit.Framework.Constraints;

public class TestGridSystem
{

    [Test]
    public void CorrectGridCellsCreation()
    {
        //Arrange
        var pool = new Pool(BoidsComponentIds.TotalComponents);
        GridSystem gridSystem = new GridSystem();
        gridSystem.SetDimensions(100, 10);
        pool.CreateSystem(gridSystem);

        //Act
        gridSystem.Initialize();

        //Assert
        // as range is in every direction, there should be 20 cells per dimension!
        Assert.AreEqual(20, gridSystem.GridEntities.Length);
        Assert.AreEqual(20, gridSystem.GridEntities[0].Length);
        Assert.AreEqual(20, gridSystem.GridEntities[9].Length);
        Assert.AreEqual(20, gridSystem.GridEntities[0][0].Length);
        Assert.AreEqual(20, gridSystem.GridEntities[0][9].Length);
        Assert.AreEqual(20, gridSystem.GridEntities[9][9].Length);
    }

    [Test]
    public void PositionToCoordinatesCalculation()
    {
        //Arrange
        var gridSystem = new GridSystem();
        gridSystem.SetDimensions(100, 10);
        Vector3 test1Position = new Vector3(23, -15, 99);
        Vector3 test2Position = new Vector3(-100, -100, -100);
        Vector3 test3Position = new Vector3(99, 99, 99);
        Vector3 test4Position = new Vector3(0, 0, 0);

        //Act
        int[] test1coordinates = gridSystem.GetCoordinates(test1Position);
        int[] test2coordinates = gridSystem.GetCoordinates(test2Position);
        int[] test3coordinates = gridSystem.GetCoordinates(test3Position);
        int[] test4coordinates = gridSystem.GetCoordinates(test4Position);

        //Assert
        // as range is in every direction, there should be 20 cells per dimension!
        Assert.AreEqual(12, test1coordinates[0]);
        Assert.AreEqual(8, test1coordinates[1]);
        Assert.AreEqual(19, test1coordinates[2]);

        Assert.AreEqual(0, test2coordinates[0]);
        Assert.AreEqual(0, test2coordinates[1]);
        Assert.AreEqual(0, test2coordinates[2]);

        Assert.AreEqual(19, test3coordinates[0]);
        Assert.AreEqual(19, test3coordinates[1]);
        Assert.AreEqual(19, test3coordinates[2]);

        Assert.AreEqual(10, test4coordinates[0]);
        Assert.AreEqual(10, test4coordinates[1]);
        Assert.AreEqual(10, test4coordinates[2]);
    }

    [Test]
    public void NeigborCellCalculation()
    {
        //Arrange
        var pool = new Pool(BoidsComponentIds.TotalComponents);
        GridSystem gridSystem = new GridSystem();
        gridSystem.SetDimensions(100, 10);
        pool.CreateSystem(gridSystem);
        gridSystem.Initialize();
        int[] cell1 = new int[] { 10, 10, 10 };
        int[] cell2 = new int[] { 0, 10, 10 };
        int[] cell3 = new int[] { 0, 0, 10 };
        int[] cell4 = new int[] { 0, 0, 0 };
        int[] cell5 = new int[] { 19, 19, 19 };

        //Act
        var cell1Neighbors = gridSystem.GetNeighboringCells(cell1[0], cell1[1], cell1[2]);
        var cell2Neighbors = gridSystem.GetNeighboringCells(cell2[0], cell2[1], cell2[2]);
        var cell3Neighbors = gridSystem.GetNeighboringCells(cell3[0], cell3[1], cell3[2]);
        var cell4Neighbors = gridSystem.GetNeighboringCells(cell4[0], cell4[1], cell4[2]);
        var cell5Neighbors = gridSystem.GetNeighboringCells(cell5[0], cell5[1], cell5[2]);

        //Assert
        // cell1 doesn't have any edges, so should have 26 neighbors
        Assert.AreEqual(26, cell1Neighbors.Length);
        // lets test a few of them
        Assert.IsTrue(System.Array.IndexOf(cell1Neighbors, gridSystem.GridEntities[11][10][10]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell1Neighbors, gridSystem.GridEntities[11][9][10]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell1Neighbors, gridSystem.GridEntities[11][9][9]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell1Neighbors, gridSystem.GridEntities[9][9][9]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell1Neighbors, gridSystem.GridEntities[11][11][11]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell1Neighbors, gridSystem.GridEntities[9][10][11]) != -1);
        // the cell itself should not be part of the neighbors!
        Assert.IsTrue(System.Array.IndexOf(cell1Neighbors, gridSystem.GridEntities[10][10][10]) == -1);
        // and the array shouldn't contain any null!
        Assert.AreEqual(-1, System.Array.IndexOf(cell1Neighbors, null));

        // cell2 has 1 edge, so should have 17 neighbors
        Assert.AreEqual(17, cell2Neighbors.Length);
        Assert.IsTrue(System.Array.IndexOf(cell2Neighbors, gridSystem.GridEntities[0][11][11]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell2Neighbors, gridSystem.GridEntities[0][11][9]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell2Neighbors, gridSystem.GridEntities[1][10][10]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell2Neighbors, gridSystem.GridEntities[0][9][10]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell2Neighbors, gridSystem.GridEntities[1][9][9]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell2Neighbors, gridSystem.GridEntities[1][10][9]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell2Neighbors, gridSystem.GridEntities[0][10][10]) == -1);
        Assert.AreEqual(-1, System.Array.IndexOf(cell2Neighbors, null));

        // cell3 has 2 edges, so should have 11 neighbors
        Assert.AreEqual(11, cell3Neighbors.Length);
        Assert.IsTrue(System.Array.IndexOf(cell3Neighbors, gridSystem.GridEntities[0][1][11]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell3Neighbors, gridSystem.GridEntities[1][0][10]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell3Neighbors, gridSystem.GridEntities[0][1][10]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell3Neighbors, gridSystem.GridEntities[1][0][9]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell3Neighbors, gridSystem.GridEntities[1][1][9]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell3Neighbors, gridSystem.GridEntities[0][0][10]) == -1);
        Assert.AreEqual(-1, System.Array.IndexOf(cell3Neighbors, null));

        // cell4 has 3 edges, so should have 6 neighbors
        Assert.AreEqual(7, cell4Neighbors.Length);
        // lets test them all in this case:
        Assert.IsTrue(System.Array.IndexOf(cell4Neighbors, gridSystem.GridEntities[1][1][1]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell4Neighbors, gridSystem.GridEntities[1][1][0]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell4Neighbors, gridSystem.GridEntities[1][0][0]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell4Neighbors, gridSystem.GridEntities[0][1][1]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell4Neighbors, gridSystem.GridEntities[0][1][0]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell4Neighbors, gridSystem.GridEntities[0][0][1]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell4Neighbors, gridSystem.GridEntities[1][0][1]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell4Neighbors, gridSystem.GridEntities[0][0][0]) == -1);
        Assert.AreEqual(-1, System.Array.IndexOf(cell4Neighbors, null));

        // cell5 also has 3 edges, but at the other end of the grid
        Assert.AreEqual(7, cell5Neighbors.Length);
        Assert.IsTrue(System.Array.IndexOf(cell5Neighbors, gridSystem.GridEntities[18][18][18]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell5Neighbors, gridSystem.GridEntities[19][19][18]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell5Neighbors, gridSystem.GridEntities[19][18][18]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell5Neighbors, gridSystem.GridEntities[18][19][19]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell5Neighbors, gridSystem.GridEntities[18][19][18]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell5Neighbors, gridSystem.GridEntities[18][18][19]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell5Neighbors, gridSystem.GridEntities[19][18][19]) != -1);
        Assert.IsTrue(System.Array.IndexOf(cell5Neighbors, gridSystem.GridEntities[19][19][19]) == -1);
        Assert.AreEqual(-1, System.Array.IndexOf(cell5Neighbors, null));
    }

    [Test]
    public void EntitiesGetPutInRightCells()
    {
        //Arrange
        var pool = new Pool(BoidsComponentIds.TotalComponents);
        GridSystem gridSystem = new GridSystem();
        gridSystem.SetDimensions(100, 10);
        pool.CreateSystem(gridSystem);
        gridSystem.Initialize();

        var entity1 = pool.CreateEntity().AddPosition(new Vector3(0, 0, 0));
        var entity2 = pool.CreateEntity().AddPosition(new Vector3(50, 50, 50));
        var entity3 = pool.CreateEntity().AddPosition(new Vector3(0, 0, 20));

        //Act
        gridSystem.Execute();

        //Assert
        Assert.IsTrue(entity1.hasContainedInCell);
        Assert.IsTrue(entity2.hasContainedInCell);
        Assert.IsTrue(entity3.hasContainedInCell);
        Assert.AreEqual(gridSystem.GetGridCellEntity(10, 10, 10), entity1.containedInCell.ContainedInCell);
        Assert.AreEqual(gridSystem.GetGridCellEntity(15, 15, 15), entity2.containedInCell.ContainedInCell);
        Assert.AreEqual(gridSystem.GetGridCellEntity(10, 10, 12), entity3.containedInCell.ContainedInCell);

        for (int xIndex = 0; xIndex < 20; ++xIndex)
        {
            for (int yIndex = 0; yIndex < 20; ++yIndex)
            {
                for (int zIndex = 0; zIndex < 20; ++zIndex)
                {
                    var gridCellEntity = gridSystem.GetGridCellEntity(xIndex, yIndex, zIndex);
                    if (xIndex == 10 && yIndex == 10 && zIndex == 10)
                    {
                        Assert.AreEqual(1, gridCellEntity.gridCell.ContainsEntities.Count);
                        Assert.That(gridCellEntity.gridCell.ContainsEntities, Has.Member(entity1));
                    }
                    else if (xIndex == 15 && yIndex == 15 && zIndex == 15)
                    {
                        Assert.AreEqual(1, gridCellEntity.gridCell.ContainsEntities.Count);
                        Assert.That(gridCellEntity.gridCell.ContainsEntities, Has.Member(entity2));
                    }
                    else if (xIndex == 10 && yIndex == 10 && zIndex == 12)
                    {
                        Assert.AreEqual(1, gridCellEntity.gridCell.ContainsEntities.Count);
                        Assert.That(gridCellEntity.gridCell.ContainsEntities, Has.Member(entity3));
                    }
                    else
                    {
                        Assert.AreEqual(0, gridCellEntity.gridCell.ContainsEntities.Count);
                    }
                }
            }
        }
    }

    private void AssertGridCellContainment(GridSystem gridSystem, Entity entity, int targetXIndex, int targetYIndex, int targetZIndex)
    {
        for (int xIndex = 0; xIndex < 20; ++xIndex)
        {
            for (int yIndex = 0; yIndex < 20; ++yIndex)
            {
                for (int zIndex = 0; zIndex < 20; ++zIndex)
                {
                    var gridCellEntity = gridSystem.GetGridCellEntity(xIndex, yIndex, zIndex);
                    if (xIndex == targetXIndex && yIndex == targetYIndex && zIndex == targetZIndex)
                    {
                        Assert.That(gridCellEntity.gridCell.ContainsEntities, Has.Member(entity), 
                            "Entity expected at "+targetXIndex+" "+targetYIndex+" "+targetZIndex);
                    }
                    else
                    {
                        Assert.That(gridCellEntity.gridCell.ContainsEntities, Has.No.Member(entity),
                            "Entity NOT expected at "+targetXIndex+" "+targetYIndex+" "+targetZIndex);
                    }
                }
            }
        }
    }

    [Test]
    public void GridCellsGetUpdatedCorrectly()
    {
        //Arrange
        var pool = new Pool(BoidsComponentIds.TotalComponents);
        GridSystem gridSystem = new GridSystem();
        gridSystem.SetDimensions(100, 10);
        pool.CreateSystem(gridSystem);
        gridSystem.Initialize();

        var entity1 = pool.CreateEntity().AddPosition(new Vector3(0, 0, 0));
        var entity2 = pool.CreateEntity().AddPosition(new Vector3(50, 50, 50));
        var entity3 = pool.CreateEntity().AddPosition(new Vector3(0, 0, 20));

        //Act
        gridSystem.Execute();
        entity1.ReplacePosition(new Vector3(-15, 15, 33));
        entity2.ReplacePosition(new Vector3(-31, -51, 90));
        entity3.ReplacePosition(new Vector3(25, -25, 25));
        gridSystem.Execute();

        //Assert
        AssertGridCellContainment(gridSystem, entity1, 8, 11, 13);
        AssertGridCellContainment(gridSystem, entity2, 6, 4, 19);
        AssertGridCellContainment(gridSystem, entity3, 12, 7, 12);
    }
}
