using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Entitas;

public class TestFlockingSystem {

	[Test]
	public void TestUpdatePositionWithVelocity() {
		//Arrange
		var pool = new Pool(BoidsComponentIds.TotalComponents);
		FlockingSystem flockingSystem = new FlockingSystem();
		flockingSystem.PreserveExistingDirectionWeight = 1;
		flockingSystem.NormalVelocity = 7.6f;
		pool.CreateSystem(flockingSystem);

		Vector3 startingPosition = new Vector3(10, 15, -3);
		Vector3 direction = new Vector3(0.3f, -0.7f, 0.1f).normalized;
		float velocityMagnitude = flockingSystem.NormalVelocity;
		int numberOfUpdates = 3;

		var boid = pool.CreateEntity().
			AddPosition(startingPosition).
			AddVelocity(direction, velocityMagnitude).
			IsBoid(true);

		//Act
		for(int update=0; update < numberOfUpdates; ++update)
			flockingSystem.Execute();		

		//Assert
		Assert.That(Vector3.Distance(
			boid.position.Position,
			startingPosition + velocityMagnitude * direction * numberOfUpdates * flockingSystem.Deltatime),
			Is.LessThanOrEqualTo(0.1f));		
	}

	[Test]
	public void TestPreserveExistingDirection() {
		//Arrange
		var pool = new Pool(BoidsComponentIds.TotalComponents);
		FlockingSystem flockingSystem = new FlockingSystem();
		flockingSystem.PreserveExistingDirectionWeight = 1;
		pool.CreateSystem(flockingSystem);

		var boid = pool.CreateEntity().
			AddPosition(new Vector3(0,0,0)).
			AddVelocity(new Vector3(0,0,1), 10).
			IsBoid(true);

		//Act
		flockingSystem.Execute();
		flockingSystem.Execute();
		flockingSystem.Execute();		

		//Assert
		Assert.That(
			boid.velocity.Direction, Is.EqualTo(new Vector3(0,0,1)));		
	}

	[Test]
	public void TestMaxDistanceToCenter() {
		//Arrange
		var pool = new Pool(BoidsComponentIds.TotalComponents);
		FlockingSystem flockingSystem = new FlockingSystem();
		flockingSystem.PreserveExistingDirectionWeight = 1;
		flockingSystem.MaxDistToCenterWeight = 2;
		pool.CreateSystem(flockingSystem);

		Vector3 startingPosition = new Vector3(50, 50, 50);
		Vector3 direction = new Vector3(1f, 1f, 1f).normalized;
		float velocityMagnitude = flockingSystem.NormalVelocity;
		int numberOfUpdates = 200;

		var boid = pool.CreateEntity().
			AddPosition(startingPosition).
			AddVelocity(direction, velocityMagnitude).
			IsBoid(true);

		float[] distancesToCenter = new float[numberOfUpdates];
		
		//Act
		for(int update=0; update < numberOfUpdates; ++update){
			flockingSystem.Execute();
			distancesToCenter[update] = boid.position.Position.magnitude;		
		}

		//Assert
		for(int update=0; update < numberOfUpdates; ++update){
			Assert.That(
				distancesToCenter[update], Is.LessThan(flockingSystem.MaxDistToCenter));
		}
	}
}
