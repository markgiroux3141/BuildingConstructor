using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room{

	public enum AXIS_LOCK
	{
		AXIS_X,
		AXIS_Z,
		NONE
	}

	public Vector3 Pos;
	public Vector3 Scale;
	public AXIS_LOCK axisLock;
	public Helpers.EDGES disabledEdge;
	public Room parentRoom;
	public Helpers.EDGES connectingEdge;
	public List<Room> childRooms;
	public List<Door> doors;
	public List<Wall> walls;
	public float WallHeight;
	public bool isParent;
	public bool active;

	public Room(Vector3 pos,Vector3 scale, float doorWidth, float doorHeight){
		childRooms = new List<Room>();
		doors = new List<Door> ();
		walls = new List<Wall> ();
		Pos = pos;
		Scale = scale;
		isParent = false;
		active = true;
	}

	public Room(){
		isParent = false;
		childRooms = new List<Room>();
		doors = new List<Door> ();
		walls = new List<Wall> ();
		active = true;
	}
}
