using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilder{

	System.Random r;

	public float WallThickness{ get; set; }

	public float MinWallHeight{ get; set; }
	public float MaxWallHeight{ get; set; }

	public float FloorThickness{ get; set; }
	public float DoorWidth{ get; set; }
	public float RoomHeight{ get; set; }
	public float DoorHeight{ get; set; }
	public static float MinRoomSize{ get; set; }
	public static float MaxRoomSize{ get; set; }

	public string floorMat{ get; set; }
	public string wallMat{ get; set; }
	public string ceilingMat{ get; set; }

	public float MaterialTiling{get;set;}

	ProceduralMesh pm;

	public List<GameObject> floorSegs;
	public List<GameObject> ceilingSegs;
	public List<GameObject> doorSegs;
	public List<GameObject> doorUpperSegs;
	public List<GameObject> wallSegs;
	public List<Room> rooms;
	public List<Door> doors;
	public List<Wall> walls;
	public List<GameObject> dummy;

	public HouseBuilder(string fMat, string wMat, string cMat, float matTile,float minWallHeight, float maxWallHeight){

		r = new System.Random ();
		pm = new ProceduralMesh ();
		floorSegs = new List<GameObject> ();
		ceilingSegs = new List<GameObject> ();
		doorSegs = new List<GameObject> ();
		doorUpperSegs = new List<GameObject> ();
		wallSegs = new List<GameObject> ();
		rooms = new List<Room> ();
		doors = new List<Door> ();
		walls = new List<Wall> ();
		dummy = new List<GameObject> ();
		floorMat = fMat;
		wallMat = wMat;
		ceilingMat = cMat;
		MaterialTiling = matTile;
		MinWallHeight = minWallHeight;
		MaxWallHeight = maxWallHeight;
	}

	public Room AddMainRoom(Vector3 pos, Vector3 scale){  // Create main room that all others will branch from

		Room room = new Room (pos,scale, DoorWidth, DoorHeight);

		room.disabledEdge = Helpers.EDGES.NONE;
		room.connectingEdge = Helpers.EDGES.NONE;
		room.axisLock = Room.AXIS_LOCK.NONE;
		room.WallHeight = Helpers.RandRange (MinWallHeight, MaxWallHeight);

		rooms.Add (room);

		return room;
	}

	public void CreateLevel(int depth, float minRSize,float maxRSize, float wallThickness,float floorThickness, float doorWidth,float doorHeight, Vector3 center){ // Iterative process that creates entire floor plan
		ProceduralMesh p = new ProceduralMesh ();
		WallThickness = wallThickness;
		FloorThickness = floorThickness;
		DoorWidth = doorWidth;
		DoorHeight = doorHeight;
		MinRoomSize = minRSize;
		MaxRoomSize = maxRSize;

		int currRoomIndex = 0;
		int currRoomCount = 0;

		AddMainRoom (center, SetRandomRoomSize());

		for (int i = 0; i < depth; i++) {
			currRoomIndex = currRoomCount;
			currRoomCount = rooms.Count;
			for (int n = currRoomIndex; n < currRoomCount; n++) {
				AddRooms (rooms [n]);
				AdjustRooms ();
				RemoveSmallRooms ();
				RemoveFloatingRooms ();
				RemoveOverAdjustedRooms ();

			}
		}

		for (int i = 0; i < rooms.Count; i++) {
			if (rooms [i].isParent) {
				CreateDoors (rooms [i]);
			}
			CreateWalls (rooms [i]);
		}

		//CreateWalls (rooms [3]);
			
		BuildAllRooms ();
		BuildAllDoors ();
		BuildAllWalls ();
	}

	// Game Object Rooms
	public void BuildAllRooms(){ // Create Game Objects for floor and ceiling
		GameObject[] gRooms = new GameObject[rooms.Count];
		GameObject[] gCeiling = new GameObject[rooms.Count];

		for (int i = 0; i < rooms.Count; i++) {
			Vector3 ceilingPos = new Vector3 (rooms [i].Pos.x, rooms [i].Pos.y + (rooms[i].WallHeight * 2f), rooms [i].Pos.z);
			Vector3 ceilingScale = new Vector3 (rooms [i].Scale.x, rooms [i].Scale.y, rooms [i].Scale.z);
			gRooms [i] = pm.BoxShape (rooms [i].Pos, rooms [i].Scale, true, floorMat, MaterialTiling, true, "Room");
			gCeiling [i] = pm.BoxShape (ceilingPos, ceilingScale, true, ceilingMat, MaterialTiling, true, "Ceiling");
			floorSegs.Add (gRooms [i]);
			ceilingSegs.Add (gCeiling [i]);

		}
	}

	public void BuildAllDoors(){
		
		GameObject[] gDoors = new GameObject[doors.Count];
		GameObject[] gUpperDoors = new GameObject[doors.Count];

		for (int i = 0; i < doors.Count; i++) {
			Vector3 tempScale = new Vector3();
			Vector3 tempUpperScale = new Vector3();
			float doorUheight = doors[i].parentRoom.WallHeight - DoorHeight;

			if (doors [i].orient == Door.ORIENT.AXIS_X) {
				tempScale = new Vector3 (DoorWidth/2f, FloorThickness, WallThickness/2f);
				tempUpperScale = new Vector3 (DoorWidth/2f, doorUheight, WallThickness/2f);
			} else if (doors [i].orient == Door.ORIENT.AXIS_Z) {
				tempScale = new Vector3 (WallThickness/2f, FloorThickness, DoorWidth/2f);
				tempUpperScale = new Vector3 (WallThickness/2f, doorUheight, DoorWidth/2f);
			}

			Vector3 tempUpperPos = new Vector3 (doors [i].Pos.x, doors [i].Pos.y + (doors[i].parentRoom.WallHeight*2f) - doorUheight, doors [i].Pos.z);

			gDoors [i] = pm.BoxShape (doors [i].Pos, tempScale, true, floorMat, MaterialTiling, true, "Door Bottom");
			gUpperDoors [i] = pm.BoxShape (tempUpperPos, tempUpperScale, true, wallMat, MaterialTiling, true, "Door Top");
			doorSegs.Add (gDoors[i]);
			doorUpperSegs.Add (gUpperDoors[i]);
		}
	}

	public void BuildAllWalls(){

		GameObject[] gWalls = new GameObject[walls.Count];

		for (int i = 0; i < walls.Count; i++) {

			gWalls [i] = pm.BoxShape (walls [i].Pos, walls[i].Scale, true, wallMat, MaterialTiling, true, "Wall");
			wallSegs.Add (gWalls[i]);
		}
	}

	public void RefreshRooms(){ // Refresh Gameobjects after alteration
		for (int i = 0; i < floorSegs.Count; i++) {
			MonoBehaviour.Destroy (floorSegs [i].gameObject);
			floorSegs.Clear ();
		}
		BuildAllRooms ();
	}
	////// 

	public void AddRooms(Room parentRoom){ // Create up to four rooms given a parent room

		if (parentRoom.disabledEdge != Helpers.EDGES.TOP) {
			Room r1 = AddRoom (parentRoom, Helpers.EDGES.TOP);
			r1.disabledEdge = Helpers.EDGES.BOTTOM;
			r1.connectingEdge = Helpers.EDGES.TOP;
			parentRoom.childRooms.Add (r1);
			r1.WallHeight = Helpers.RandRange (MinWallHeight, MaxWallHeight);
			rooms.Add (r1);
		}

		if (parentRoom.disabledEdge != Helpers.EDGES.BOTTOM) {
			Room r2 = AddRoom (parentRoom, Helpers.EDGES.BOTTOM);
			r2.disabledEdge = Helpers.EDGES.TOP;
			r2.connectingEdge = Helpers.EDGES.BOTTOM;
			parentRoom.childRooms.Add (r2);
			r2.WallHeight = Helpers.RandRange (MinWallHeight, MaxWallHeight);
			rooms.Add (r2);
		}

		if (parentRoom.disabledEdge != Helpers.EDGES.LEFT) {
			Room r3 = AddRoom (parentRoom, Helpers.EDGES.LEFT);
			r3.disabledEdge = Helpers.EDGES.RIGHT;
			r3.connectingEdge = Helpers.EDGES.LEFT;
			parentRoom.childRooms.Add (r3);
			r3.WallHeight = Helpers.RandRange (MinWallHeight, MaxWallHeight);
			rooms.Add (r3);
		}

		if (parentRoom.disabledEdge != Helpers.EDGES.RIGHT) {
			Room r4 = AddRoom (parentRoom, Helpers.EDGES.RIGHT);
			r4.disabledEdge = Helpers.EDGES.LEFT;
			r4.connectingEdge = Helpers.EDGES.RIGHT;
			parentRoom.childRooms.Add (r4);
			r4.WallHeight = Helpers.RandRange (MinWallHeight, MaxWallHeight);
			rooms.Add (r4);
		}
	}

	public Room AddRoom(Room pRoom, Helpers.EDGES edge){ // Add a single room
		Vector3 pos;
		Room tempRoom = new Room ();
		tempRoom.Scale = SetRandomRoomSize ();
		switch (edge) {
		case Helpers.EDGES.TOP:
			pos = new Vector3 (pRoom.Pos.x + GetRandRoomPos (pRoom.Scale.x, tempRoom.Scale.x), pRoom.Pos.y, pRoom.Pos.z + SetFixedRoomPos (pRoom.Scale.z, tempRoom.Scale.z));
			tempRoom.Pos = pos;
			tempRoom.axisLock = Room.AXIS_LOCK.AXIS_Z;
			break;
		case Helpers.EDGES.BOTTOM:
			pos = new Vector3 (pRoom.Pos.x + GetRandRoomPos (pRoom.Scale.x, tempRoom.Scale.x), pRoom.Pos.y, pRoom.Pos.z - SetFixedRoomPos (pRoom.Scale.z, tempRoom.Scale.z));
			tempRoom.Pos = pos;
			tempRoom.axisLock = Room.AXIS_LOCK.AXIS_Z;
			break;
		case Helpers.EDGES.RIGHT:
			pos = new Vector3 (pRoom.Pos.x + SetFixedRoomPos (pRoom.Scale.x, tempRoom.Scale.x), pRoom.Pos.y, pRoom.Pos.z + GetRandRoomPos (pRoom.Scale.z, tempRoom.Scale.z));
			tempRoom.Pos = pos;
			tempRoom.axisLock = Room.AXIS_LOCK.AXIS_X;
			break;
		case Helpers.EDGES.LEFT:
			pos = new Vector3 (pRoom.Pos.x - SetFixedRoomPos (pRoom.Scale.x, tempRoom.Scale.x), pRoom.Pos.y, pRoom.Pos.z + GetRandRoomPos (pRoom.Scale.z, tempRoom.Scale.z));
			tempRoom.Pos = pos;
			tempRoom.axisLock = Room.AXIS_LOCK.AXIS_X;
			break;
		default:
			break;
		}

		tempRoom.parentRoom = pRoom;
		pRoom.isParent = true;

		return tempRoom;
	}

	public void AdjustRooms(){ // check for collisions and contract edges
		Collision col = new Collision ();
		for (int i = 0; i < rooms.Count; i++) {
			for (int n = i; n < rooms.Count; n++) {
				if (i != n) {
					if (rooms [i] == rooms [n].parentRoom || rooms [n] == rooms [i].parentRoom) {
						col = Helpers.CheckRoomCollision (rooms [i], rooms [n]);
					} else {
						col = Helpers.CheckRoomCollision (rooms [i], rooms [n],WallThickness * 2f); // if rooms are unconnected ensure gap between walls
					}
						
					if (col.collision == true) {
						SetRoomAdjustParams (rooms [i], rooms [n], col);
					}
				}
			}
		}
	}

	public void SetRoomAdjustParams(Room r1, Room r2, Collision c){ // calculate how much to contract each edge
		float tempOLX = 1000f;
		float tempOLZ = 1000f;
		Helpers.EDGES edgeX = Helpers.EDGES.RIGHT;
		Helpers.EDGES edgeZ = Helpers.EDGES.TOP;

		if (c.topOverlap < tempOLZ) {
			tempOLZ = c.topOverlap;
			edgeZ = Helpers.EDGES.TOP;
		}
		if (c.bottomOverlap < tempOLZ) {
			tempOLZ = c.bottomOverlap;
			edgeZ = Helpers.EDGES.BOTTOM;
		}
		if (c.leftOverlap < tempOLX) {
			tempOLX = c.leftOverlap;
			edgeX = Helpers.EDGES.LEFT;
		}
		if (c.rightOverlap < tempOLX) {
			tempOLX = c.rightOverlap;
			edgeX = Helpers.EDGES.RIGHT;
		}

		if (r2.axisLock == Room.AXIS_LOCK.AXIS_X) {
			if (edgeZ == Helpers.EDGES.TOP) {
				Helpers.ContractEdge (r2, Helpers.EDGES.TOP, tempOLZ + WallThickness);
			} else {
				Helpers.ContractEdge (r2, Helpers.EDGES.BOTTOM, tempOLZ + WallThickness);
			}
		} else if (r2.axisLock == Room.AXIS_LOCK.AXIS_Z) {
			if (edgeZ == Helpers.EDGES.LEFT) {
				Helpers.ContractEdge (r2, Helpers.EDGES.LEFT, tempOLX + WallThickness);
			} else {
				Helpers.ContractEdge (r2, Helpers.EDGES.RIGHT, tempOLX + WallThickness);
			}
		}
	}

	public float GetRandRoomPos(float parRoomScale, float newRoomScale){  // used to slide room along length of parent room to random location
		return Helpers.RandRange ((-parRoomScale + DoorWidth) - newRoomScale, (parRoomScale - DoorWidth) + newRoomScale);
	}

	public float SetFixedRoomPos(float parRoomScale, float newRoomScale){  // used to fix room beside parent room
		return parRoomScale + WallThickness + newRoomScale;
	}

	public Vector3 SetRandomRoomSize(){ //sets room to random size
		return new Vector3 (Helpers.RandRange (MinRoomSize, MaxRoomSize), FloorThickness, Helpers.RandRange (MinRoomSize, MaxRoomSize));
	}

	public void RemoveSmallRooms(){ // if room adjustment causes contraction beyond min size of room, remove room
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms [i].Scale.x < MinRoomSize || rooms [i].Scale.z < MinRoomSize) {
				rooms [i].active = false;
			}
		}

		for (int i = 0; i < rooms.Count; i++) {
			if (rooms[i].active == false){
				rooms.RemoveAt (i);
			}
		}

	}

	public void RemoveFloatingRooms(){ // remove rooms that lack parents
		for (int i = 1; i < rooms.Count; i++) {
			if (rooms [i].parentRoom.active == false) {
				rooms [i].active = false;
			}
		}

		for (int i = 0; i < rooms.Count; i++) {
			if (rooms [i].active == false) {
				rooms.RemoveAt (i);
			}
		}
	}

	public void RemoveOverAdjustedRooms(){
		for (int i = 0; i < rooms.Count; i++) {
			if (rooms [i].isParent == true) {
				for (int n = 0; n < rooms [i].childRooms.Count; n++) {
					//TOP BOTTOM
					if (!CheckRoomConnected (rooms [i].Pos.x, rooms [i].childRooms [n].Pos.x, rooms [i].Scale.x, rooms [i].childRooms [n].Scale.x) &&
						!CheckRoomConnected (rooms [i].Pos.z, rooms [i].childRooms [n].Pos.z, rooms [i].Scale.z, rooms [i].childRooms [n].Scale.z)) {
						rooms [i].childRooms [n].active = false;
					}
				}
			}
		}

		for (int i = 0; i < rooms.Count; i++) {
			if (rooms [i].active == false) {
				rooms.RemoveAt (i);
			}
		}
	}

	public bool CheckRoomConnected(float pRoomPos, float cRoomPos, float pRoomSize, float cRoomSize){
		float pLeft = pRoomPos - pRoomSize;
		float pRight = pRoomPos + pRoomSize;
		float cLeft = cRoomPos - cRoomSize;
		float cRight = cRoomPos + cRoomSize;

		if ((cRight - pLeft) < DoorWidth || (pRight - cLeft) < DoorWidth) {
			return false;
		} else {
			return true;
		}
	}

	public void CreateDoors(Room room){
		for (int i = 0; i < room.childRooms.Count; i++) {
			if (room.childRooms [i].active == true) {
				switch (room.childRooms [i].connectingEdge) {
				case Helpers.EDGES.TOP:
					Door door1 = new Door ();
					door1.Pos = new Vector3 (RandDoorLocation (room.Pos.x, room.childRooms [i].Pos.x, room.Scale.x, room.childRooms [i].Scale.x), room.Pos.y, room.Pos.z + room.Scale.z + (WallThickness / 2f));
					door1.orient = Door.ORIENT.AXIS_X;
					door1.edge = Helpers.EDGES.TOP;
					door1.parentRoom = room;
					room.doors.Add (door1);
					doors.Add (door1);
					break;
				case Helpers.EDGES.BOTTOM:
					Door door2 = new Door ();
					door2.Pos = new Vector3 (RandDoorLocation (room.Pos.x, room.childRooms [i].Pos.x, room.Scale.x, room.childRooms [i].Scale.x), room.Pos.y, room.Pos.z - room.Scale.z - (WallThickness / 2f));
					door2.orient = Door.ORIENT.AXIS_X;
					door2.edge = Helpers.EDGES.BOTTOM;
					door2.parentRoom = room;
					room.doors.Add (door2);
					doors.Add (door2);
					break;
				case Helpers.EDGES.LEFT:
					Door door3 = new Door ();
					door3.Pos = new Vector3 (room.Pos.x - room.Scale.x - (WallThickness / 2f), room.Pos.y, RandDoorLocation (room.Pos.z, room.childRooms [i].Pos.z, room.Scale.z, room.childRooms [i].Scale.z));
					door3.orient = Door.ORIENT.AXIS_Z;
					door3.edge = Helpers.EDGES.LEFT;
					door3.parentRoom = room;
					room.doors.Add (door3);
					doors.Add (door3);
					break;
				case Helpers.EDGES.RIGHT:
					Door door4 = new Door ();
					door4.Pos = new Vector3 (room.Pos.x + room.Scale.x + (WallThickness / 2f), room.Pos.y, RandDoorLocation (room.Pos.z, room.childRooms [i].Pos.z, room.Scale.z, room.childRooms [i].Scale.z));
					door4.orient = Door.ORIENT.AXIS_Z;
					door4.edge = Helpers.EDGES.RIGHT;
					door4.parentRoom = room;
					room.doors.Add (door4);
					doors.Add (door4);
					break;
				default:
					break;
				}
			}
		}
	}

	public float RandDoorLocation(float pRoomPos, float cRoomPos, float pRoomSize, float cRoomSize){
		float pLeft = pRoomPos - pRoomSize;
		float pRight = pRoomPos + pRoomSize;
		float cLeft = cRoomPos - cRoomSize;
		float cRight = cRoomPos + cRoomSize;

		float minVal = Mathf.Max (pLeft, cLeft) + DoorWidth;
		float maxVal = Mathf.Min (pRight, cRight) - DoorWidth;

		return Helpers.RandRange (minVal, maxVal);
	}

	public void CreateWalls(Room room){

		bool TopHasChild = false, BottomHasChild = false, LeftHasChild = false, RightHasChild = false;

		for (int i = 0; i < room.childRooms.Count; i++) {
			if (room.childRooms [i].connectingEdge == Helpers.EDGES.TOP && room.childRooms[i].active == true)
				TopHasChild = true;

			if (room.childRooms [i].connectingEdge == Helpers.EDGES.BOTTOM && room.childRooms[i].active == true)
				BottomHasChild = true;

			if (room.childRooms [i].connectingEdge == Helpers.EDGES.LEFT && room.childRooms[i].active == true)
				LeftHasChild = true;

			if (room.childRooms [i].connectingEdge == Helpers.EDGES.RIGHT && room.childRooms[i].active == true)
				RightHasChild = true;
		}
			
	//Top Wall
		if (room.disabledEdge != Helpers.EDGES.TOP) {
			if (TopHasChild) {
				for (int i = 0; i < room.doors.Count; i++) {
					if (room.doors [i].edge == Helpers.EDGES.TOP) {
						Wall tWall1 = new Wall ();
						Wall tWall2 = new Wall ();

						GenericScalePos[] wallParams = GetWallParams (room.Pos.x, room.Scale.x, room.doors [i].Pos.x);

						tWall1.Pos = new Vector3 (wallParams [0].Pos, room.Pos.y + room.WallHeight, room.doors [i].Pos.z);
						tWall1.Scale = new Vector3 (wallParams [0].Scale, room.WallHeight, WallThickness / 2f);
						room.walls.Add (tWall1);
						walls.Add (tWall1);

						tWall2.Pos = new Vector3 (wallParams [1].Pos, room.Pos.y + room.WallHeight, room.doors [i].Pos.z);
						tWall2.Scale = new Vector3 (wallParams [1].Scale, room.WallHeight, WallThickness / 2f);
						room.walls.Add (tWall2);
						walls.Add (tWall2);
					}
				}
			} else {
				Wall tempWall = new Wall ();
				tempWall.Pos = new Vector3 (room.Pos.x, room.Pos.y + room.WallHeight, room.Pos.z + room.Scale.z + (WallThickness / 2f));
				tempWall.Scale = new Vector3 (room.Scale.x, room.WallHeight, WallThickness / 2f);
				room.walls.Add (tempWall);
				walls.Add (tempWall);
			}
		} else {
			Wall remWall1 = new Wall ();
			Wall remWall2 = new Wall ();
			GenericScalePos[] wallp = GetWallRemainder (room.parentRoom.Pos.x, room.Pos.x, room.parentRoom.Scale.x, room.Scale.x);

			if (wallp [0].Scale != 0) {
				remWall1.Pos = new Vector3 (wallp [0].Pos, room.Pos.y + room.WallHeight, room.Pos.z + room.Scale.z + (WallThickness / 2f));
				remWall1.Scale = new Vector3 (wallp [0].Scale, room.WallHeight, WallThickness / 2f);
				room.walls.Add (remWall1);
				walls.Add (remWall1);
			}

			if (wallp [1].Scale != 0) {
				remWall2.Pos = new Vector3 (wallp [1].Pos, room.Pos.y + room.WallHeight, room.Pos.z + room.Scale.z + (WallThickness / 2f));
				remWall2.Scale = new Vector3 (wallp [1].Scale, room.WallHeight, WallThickness / 2f);
				room.walls.Add (remWall2);
				walls.Add (remWall2);
			}

			if (room.WallHeight > room.parentRoom.WallHeight) {
				Wall topWall = new Wall ();
				float upperLeft = room.Pos.x - room.Scale.x;
				float upperRight = room.Pos.x + room.Scale.x;

				if (wallp [0].Scale != 0) {
					upperLeft = wallp [0].Pos + wallp [0].Scale;
				}

				if (wallp [1].Scale != 0) {
					upperRight = wallp [1].Pos - wallp [1].Scale;
				}

				float tempPos = (upperLeft + upperRight) / 2f;
				float tempScale = (upperRight - upperLeft) / 2f;

				topWall.Pos = new Vector3 (tempPos, room.Pos.y + room.WallHeight + room.parentRoom.WallHeight, room.Pos.z + room.Scale.z + (WallThickness / 2f));
				topWall.Scale = new Vector3 (tempScale, room.WallHeight - room.parentRoom.WallHeight, WallThickness / 2f);
				room.walls.Add (topWall);
				walls.Add (topWall);
			}
		}


		//Bottom Walls
		if (room.disabledEdge != Helpers.EDGES.BOTTOM) {
			if (BottomHasChild) {
			for (int i = 0; i < room.doors.Count; i++) {
				if (room.doors [i].edge == Helpers.EDGES.BOTTOM) {
					Wall tWall1 = new Wall ();
					Wall tWall2 = new Wall ();

					GenericScalePos[] wallParams = GetWallParams (room.Pos.x, room.Scale.x, room.doors [i].Pos.x);

					tWall1.Pos = new Vector3 (wallParams [0].Pos, room.Pos.y + room.WallHeight, room.doors [i].Pos.z);
					tWall1.Scale = new Vector3 (wallParams [0].Scale, room.WallHeight, WallThickness / 2f);
					room.walls.Add (tWall1);
					walls.Add (tWall1);

					tWall2.Pos = new Vector3 (wallParams [1].Pos, room.Pos.y + room.WallHeight, room.doors [i].Pos.z);
					tWall2.Scale = new Vector3 (wallParams [1].Scale, room.WallHeight, WallThickness / 2f);
					room.walls.Add (tWall2);
					walls.Add (tWall2);
				}
			}

			} else {
				Wall tempWall = new Wall ();
				tempWall.Pos = new Vector3 (room.Pos.x, room.Pos.y + room.WallHeight, room.Pos.z - room.Scale.z - (WallThickness / 2f));
				tempWall.Scale = new Vector3 (room.Scale.x, room.WallHeight, WallThickness / 2f);
				room.walls.Add (tempWall);
				walls.Add (tempWall);
			}

		} else {
			Wall remWall1 = new Wall ();
			Wall remWall2 = new Wall ();
			GenericScalePos[] wallp = GetWallRemainder (room.parentRoom.Pos.x, room.Pos.x, room.parentRoom.Scale.x, room.Scale.x);

			if (wallp [0].Scale != 0) {
				remWall1.Pos = new Vector3 (wallp [0].Pos, room.Pos.y + room.WallHeight, room.Pos.z - room.Scale.z - (WallThickness / 2f));
				remWall1.Scale = new Vector3 (wallp [0].Scale, room.WallHeight, WallThickness / 2f);
				room.walls.Add (remWall1);
				walls.Add (remWall1);
			}

			if (wallp [1].Scale != 0) {
				remWall2.Pos = new Vector3 (wallp [1].Pos, room.Pos.y + room.WallHeight, room.Pos.z - room.Scale.z - (WallThickness / 2f));
				remWall2.Scale = new Vector3 (wallp [1].Scale, room.WallHeight, WallThickness / 2f);
				room.walls.Add (remWall2);
				walls.Add (remWall2);
			}

			if (room.WallHeight > room.parentRoom.WallHeight) {
				Wall topWall = new Wall ();
				float upperLeft = room.Pos.x - room.Scale.x;
				float upperRight = room.Pos.x + room.Scale.x;

				if (wallp [0].Scale != 0) {
					upperLeft = wallp [0].Pos + wallp [0].Scale;
				}

				if (wallp [1].Scale != 0) {
					upperRight = wallp [1].Pos - wallp [1].Scale;
				}

				float tempPos = (upperLeft + upperRight) / 2f;
				float tempScale = (upperRight - upperLeft) / 2f;
				topWall.Pos = new Vector3 (tempPos, room.Pos.y + room.WallHeight + room.parentRoom.WallHeight, room.Pos.z - room.Scale.z - (WallThickness / 2f));
				topWall.Scale = new Vector3 (tempScale, room.WallHeight - room.parentRoom.WallHeight, WallThickness / 2f);
				room.walls.Add (topWall);
				walls.Add (topWall);
			}
		}

		//Left Walls
		if (room.disabledEdge != Helpers.EDGES.LEFT) {
			if (LeftHasChild) {
				for (int i = 0; i < room.doors.Count; i++) {
					if (room.doors [i].edge == Helpers.EDGES.LEFT) {
						Wall tWall1 = new Wall ();
						Wall tWall2 = new Wall ();

						GenericScalePos[] wallParams = GetWallParams (room.Pos.z, room.Scale.z, room.doors [i].Pos.z);

						tWall1.Pos = new Vector3 (room.doors [i].Pos.x, room.Pos.y + room.WallHeight, wallParams [0].Pos);
						tWall1.Scale = new Vector3 (WallThickness / 2f, room.WallHeight, wallParams [0].Scale);
						room.walls.Add (tWall1);
						walls.Add (tWall1);

						tWall2.Pos = new Vector3 (room.doors [i].Pos.x, room.Pos.y + room.WallHeight, wallParams [1].Pos);
						tWall2.Scale = new Vector3 (WallThickness / 2f, room.WallHeight, wallParams [1].Scale);
						room.walls.Add (tWall2);
						walls.Add (tWall2);
					}
				}

			} else {
				Wall tempWall = new Wall ();
				tempWall.Pos = new Vector3 (room.Pos.x - room.Scale.x - (WallThickness / 2f), room.Pos.y + room.WallHeight, room.Pos.z);
				tempWall.Scale = new Vector3 (WallThickness / 2f, room.WallHeight, room.Scale.z);
				room.walls.Add (tempWall);
				walls.Add (tempWall);
			}
		} else {
			Wall remWall1 = new Wall ();
			Wall remWall2 = new Wall ();
			GenericScalePos[] wallp = GetWallRemainder (room.parentRoom.Pos.z, room.Pos.z, room.parentRoom.Scale.z, room.Scale.z);

			if (wallp [0].Scale != 0) {
				remWall1.Pos = new Vector3 (room.Pos.x - room.Scale.x - (WallThickness / 2f), room.Pos.y + room.WallHeight, wallp[0].Pos);
				remWall1.Scale = new Vector3 (WallThickness/2f, room.WallHeight, wallp [0].Scale);
				room.walls.Add (remWall1);
				walls.Add (remWall1);
			}

			if (wallp [1].Scale != 0) {
				remWall2.Pos = new Vector3 (room.Pos.x - room.Scale.x - (WallThickness / 2f), room.Pos.y + room.WallHeight, wallp[1].Pos);
				remWall2.Scale = new Vector3 (WallThickness/2f, room.WallHeight, wallp [1].Scale);
				room.walls.Add (remWall2);
				walls.Add (remWall2);
			}

			if (room.WallHeight > room.parentRoom.WallHeight) {
				Wall topWall = new Wall ();
				float upperBottom = room.Pos.z - room.Scale.z;
				float upperTop = room.Pos.z + room.Scale.z;

				if (wallp [0].Scale != 0) {
					upperBottom = wallp [0].Pos + wallp [0].Scale;
				}

				if (wallp [1].Scale != 0) {
					upperTop = wallp [1].Pos - wallp [1].Scale;
				}

				float tempPos = (upperBottom + upperTop) / 2f;
				float tempScale = (upperTop - upperBottom) / 2f;
				topWall.Pos = new Vector3 (room.Pos.x - room.Scale.x - (WallThickness / 2f), room.Pos.y + room.WallHeight + room.parentRoom.WallHeight, tempPos);
				topWall.Scale = new Vector3 (WallThickness / 2f, room.WallHeight - room.parentRoom.WallHeight, tempScale);
				room.walls.Add (topWall);
				walls.Add (topWall);
			}
		}

		//Right Walls

		if (room.disabledEdge != Helpers.EDGES.RIGHT) {
			if (RightHasChild) {
				for (int i = 0; i < room.doors.Count; i++) {
					if (room.doors [i].edge == Helpers.EDGES.RIGHT) {
						Wall tWall1 = new Wall ();
						Wall tWall2 = new Wall ();

						GenericScalePos[] wallParams = GetWallParams (room.Pos.z, room.Scale.z, room.doors [i].Pos.z);

						tWall1.Pos = new Vector3 (room.doors [i].Pos.x, room.Pos.y + room.WallHeight, wallParams [0].Pos);
						tWall1.Scale = new Vector3 (WallThickness / 2f, room.WallHeight, wallParams [0].Scale);
						room.walls.Add (tWall1);
						walls.Add (tWall1);

						tWall2.Pos = new Vector3 (room.doors [i].Pos.x, room.Pos.y + room.WallHeight, wallParams [1].Pos);
						tWall2.Scale = new Vector3 (WallThickness / 2f, room.WallHeight, wallParams [1].Scale);
						room.walls.Add (tWall2);
						walls.Add (tWall2);
					}
				}
			} else {
				Wall tempWall = new Wall ();
				tempWall.Pos = new Vector3 (room.Pos.x + room.Scale.x + (WallThickness / 2f), room.Pos.y + room.WallHeight, room.Pos.z);
				tempWall.Scale = new Vector3 (WallThickness / 2f, room.WallHeight, room.Scale.z);
				room.walls.Add (tempWall);
				walls.Add (tempWall);
			}
		} else {
			Wall remWall1 = new Wall ();
			Wall remWall2 = new Wall ();
			GenericScalePos[] wallp = GetWallRemainder (room.parentRoom.Pos.z, room.Pos.z, room.parentRoom.Scale.z, room.Scale.z);

			if (wallp [0].Scale != 0) {
				remWall1.Pos = new Vector3 (room.Pos.x + room.Scale.x + (WallThickness / 2f), room.Pos.y + room.WallHeight, wallp[0].Pos);
				remWall1.Scale = new Vector3 (WallThickness/2f, room.WallHeight, wallp [0].Scale);
				room.walls.Add (remWall1);
				walls.Add (remWall1);
			}

			if (wallp [1].Scale != 0) {
				remWall2.Pos = new Vector3 (room.Pos.x + room.Scale.x + (WallThickness / 2f), room.Pos.y + room.WallHeight, wallp[1].Pos);
				remWall2.Scale = new Vector3 (WallThickness/2f, room.WallHeight, wallp [1].Scale);
				room.walls.Add (remWall2);
				walls.Add (remWall2);
			}

			if (room.WallHeight > room.parentRoom.WallHeight) {
				Wall topWall = new Wall ();
				float upperBottom = room.Pos.z - room.Scale.z;
				float upperTop = room.Pos.z + room.Scale.z;

				if (wallp [0].Scale != 0) {
					upperBottom = wallp [0].Pos + wallp [0].Scale;
				}

				if (wallp [1].Scale != 0) {
					upperTop = wallp [1].Pos - wallp [1].Scale;
				}

				float tempPos = (upperBottom + upperTop) / 2f;
				float tempScale = (upperTop - upperBottom) / 2f;
				topWall.Pos = new Vector3 (room.Pos.x + room.Scale.x + (WallThickness / 2f), room.Pos.y + room.WallHeight + room.parentRoom.WallHeight, tempPos);
				topWall.Scale = new Vector3 (WallThickness / 2f,room.WallHeight - room.parentRoom.WallHeight, tempScale);
				room.walls.Add (topWall);
				walls.Add (topWall);
			}
		}
			
	}

	public GenericScalePos[] GetWallParams(float pRoomPos, float pRoomScale, float doorPos){
		GenericScalePos[] gTemp = new GenericScalePos[2];

		gTemp [0] = new GenericScalePos ();
		gTemp [1] = new GenericScalePos ();

		float pRoomLeft = pRoomPos - pRoomScale;
		float pRoomRight = pRoomPos + pRoomScale;
		float doorLeft = doorPos - (DoorWidth /2f);
		float doorRight = doorPos + (DoorWidth / 2f);

		gTemp [0].Pos = (pRoomLeft + doorLeft) / 2f;
		gTemp [0].Scale = (doorLeft - pRoomLeft) / 2f;

		gTemp [1].Pos = (pRoomRight + doorRight) / 2f;
		gTemp [1].Scale = (pRoomRight - doorRight) / 2f;

		return gTemp;

	}

	public GenericScalePos[] GetWallRemainder(float pRoomPos, float cRoomPos, float pRoomScale, float cRoomScale){
		GenericScalePos[] gTemp = new GenericScalePos[2];

		gTemp [0] = new GenericScalePos ();
		gTemp [1] = new GenericScalePos ();

		float pRoomLeft = pRoomPos - pRoomScale;
		float pRoomRight = pRoomPos + pRoomScale;
		float cRoomLeft = cRoomPos - cRoomScale;
		float cRoomRight = cRoomPos + cRoomScale;

		if (pRoomLeft > cRoomLeft) {
			gTemp [0].Pos = (pRoomLeft + cRoomLeft) / 2f;
			gTemp [0].Scale = (pRoomLeft - cRoomLeft) / 2f;
		} else {
			gTemp [0].Pos = 0;
			gTemp [0].Scale = 0;
		}

		if (pRoomRight < cRoomRight) {
			gTemp [1].Pos = (pRoomRight + cRoomRight) / 2f;
			gTemp [1].Scale = (cRoomRight - pRoomRight) / 2f;
		} else {
			gTemp [1].Pos = 0;
			gTemp [1].Scale = 0;
		}

		return gTemp;
	}
		

	public void AddDummy(Vector3 pos){
		GameObject g;

		g = pm.BoxShape (pos, new Vector3 (1, 1, 1), true, floorMat, 1f, true, "Dummy");

		dummy.Add (g);
	}
}
