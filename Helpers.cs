using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers{

	public static System.Random rand = new System.Random();

	public enum POINTS
	{
		FLT,
		FRT,
		BRT,
		BLT,
		FLB,
		FRB,
		BRB,
		BLB
	};

	public enum EDGES
	{
		TOP,
		BOTTOM,
		LEFT,
		RIGHT,
		NONE
	}

	public static Vector3 GetVertPos(Vector3 center, Vector3 scale, POINTS index){
		switch (index) {
		case POINTS.FLT:
			return new Vector3 (center.x - scale.x, center.y + scale.y, center.z + scale.z);
		case POINTS.FRT:
			return new Vector3 (center.x + scale.x, center.y + scale.y, center.z + scale.z);
		case POINTS.BRT:
			return new Vector3 (center.x + scale.x, center.y + scale.y, center.z - scale.z);
		case POINTS.BLT:
			return new Vector3 (center.x - scale.x, center.y + scale.y, center.z - scale.z);
		case POINTS.FLB:
			return new Vector3 (center.x - scale.x, center.y - scale.y, center.z + scale.z);
		case POINTS.FRB:
			return new Vector3 (center.x + scale.x, center.y - scale.y, center.z + scale.z);
		case POINTS.BRB:
			return new Vector3 (center.x + scale.x, center.y - scale.y, center.z - scale.z);
		case POINTS.BLB:
			return new Vector3 (center.x - scale.x, center.y - scale.y, center.z - scale.z);
		default:
			return Vector3.zero;
		}
	}

	public static Vector3 GetEdges(Vector3 center, Vector3 scale, EDGES index){
		switch (index) {
		case EDGES.TOP:
			return new Vector3 (center.x, center.y, center.z + scale.z);
		case EDGES.BOTTOM:
			return new Vector3 (center.x, center.y, center.z - scale.z);
		case EDGES.RIGHT:
			return new Vector3 (center.x + scale.x, center.y, center.z);
		case EDGES.LEFT:
			return new Vector3 (center.x - scale.x, center.y, center.z);
		default:
			return Vector3.zero;
		}
	}

	public static Collision CheckRoomCollision(Room room1, Room room2, float buffer = 0){

		Collision col = new Collision ();
		if (CheckDistThresh (room1, room2)) {

			Vector3 Top1 = GetEdges (room1.Pos, room1.Scale, EDGES.TOP);
			Vector3 Left1 = GetEdges (room1.Pos, room1.Scale, EDGES.LEFT);
			Vector3 Right1 = GetEdges (room1.Pos, room1.Scale, EDGES.RIGHT);
			Vector3 Bottom1 = GetEdges (room1.Pos, room1.Scale, EDGES.BOTTOM);

			Vector3 Top2 = GetEdges (room2.Pos, room2.Scale, EDGES.TOP);
			Vector3 Left2 = GetEdges (room2.Pos, room2.Scale, EDGES.LEFT);
			Vector3 Right2 = GetEdges (room2.Pos, room2.Scale, EDGES.RIGHT);
			Vector3 Bottom2 = GetEdges (room2.Pos, room2.Scale, EDGES.BOTTOM);

			if (((Top1.z + buffer) > Bottom2.z) && ((Left1.x - buffer) < Right2.x)
			    && ((Right1.x + buffer) > Left2.x) && ((Bottom1.z - buffer) < Top2.z)) {

				col.topOverlap = (Top1.z + buffer) - Bottom2.z;
				col.bottomOverlap = Top2.z - (Bottom1.z - buffer);
				col.rightOverlap = (Right1.x + buffer) - Left2.x;
				col.leftOverlap = Right2.x - (Left1.x - buffer);
				col.collision = true;

			} else {
				col.collision = false;
			}

			return col;
		} else {
			col.collision = false;
			return col;
		}
	}

	public static bool CheckDistThresh(Room r1, Room r2){
		if (Vector3.Distance (r1.Pos, r2.Pos) < HouseBuilder.MaxRoomSize * 2.5f) {
			return true;
		} else {
			return false;
		}
	}

	public static void ContractEdge(Room room, EDGES edge, float amount){
		switch (edge) {
		case EDGES.TOP:
			room.Scale.z -= (amount / 2f);
			room.Pos.z += (amount / 2f);
			break;
		case EDGES.BOTTOM:
			room.Scale.z -= (amount / 2f);
			room.Pos.z -= (amount / 2f);
			break;
		case EDGES.LEFT:
			room.Scale.x -= (amount / 2f);
			room.Pos.x += (amount / 2f);
			break;
		case EDGES.RIGHT:
			room.Scale.x -= (amount / 2f);
			room.Pos.x -= (amount / 2f);
			break;
		default:
			break;
		}
	}

	public static Vector3 GetCenter(Vector3 pos, Vector3 scale, POINTS index){
		switch (index) {
		case POINTS.FLT:
			return new Vector3 (pos.x + scale.x, pos.y - scale.y, pos.z - scale.z);
		case POINTS.FRT:
			return new Vector3 (pos.x - scale.x, pos.y - scale.y, pos.z - scale.z);
		case POINTS.BRT:
			return new Vector3 (pos.x - scale.x, pos.y - scale.y, pos.z + scale.z);
		case POINTS.BLT:
			return new Vector3 (pos.x + scale.x, pos.y - scale.y, pos.z + scale.z);
		case POINTS.FLB:
			return new Vector3 (pos.x + scale.x, pos.y + scale.y, pos.z - scale.z);
		case POINTS.FRB:
			return new Vector3 (pos.x - scale.x, pos.y + scale.y, pos.z - scale.z);
		case POINTS.BRB:
			return new Vector3 (pos.x - scale.x, pos.y + scale.y, pos.z + scale.z);
		case POINTS.BLB:
			return new Vector3 (pos.x + scale.x, pos.y + scale.y, pos.z + scale.z);
		default:
			return Vector3.zero;
		}
	}

	public static float RandRange(float min, float max){
		return ((float)(rand.NextDouble () * (max - min)) + min);
	}
}
	