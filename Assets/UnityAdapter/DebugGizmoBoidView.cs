using System.Collections;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class DebugGizmoBoidView : MonoBehaviour {

	public Group AllBoids;

	void OnDrawGizmos()
	{
		if(AllBoids != null){
			foreach(var boid in AllBoids.GetEntities()){
				Gizmos.DrawLine(boid.position.Position, boid.position.Position + boid.velocity.Direction);
			}
		}
	} 
	
}
