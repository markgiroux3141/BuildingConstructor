using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Lighting{

	public static void AddLights(List<Room> rooms){
		for (int i = 0; i < rooms.Count; i++) {
			GameObject light = new GameObject ("Light");
			Light lightComp = light.AddComponent<Light> ();
			lightComp.color = Color.yellow;
			lightComp.transform.position = new Vector3 (rooms [i].Pos.x, rooms [i].Pos.y + rooms[i].WallHeight, rooms [i].Pos.z);
			lightComp.range = 20f;
		}
	}
}
