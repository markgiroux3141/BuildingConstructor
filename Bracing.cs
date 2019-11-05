using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bracing{
	static ProceduralMesh pm = new ProceduralMesh ();
	public static void AddBracing(List<Room> rooms, string mat, float DoorWidth, float DoorHeight){
		for (int i = 0; i < rooms.Count; i++) {
			float bBoardSize = 0.1f;
			float cBeamSize = 0.2f;
			GameObject[] baseBoards = new GameObject[4];
			GameObject[] cornerBeams = new GameObject[4];


			baseBoards [0] = pm.BoxShape (new Vector3 (rooms [i].Pos.x - rooms [i].Scale.x + bBoardSize, rooms [i].Pos.y + bBoardSize, rooms [i].Pos.z), new Vector3 (bBoardSize, bBoardSize, rooms [i].Scale.z), true, mat, 0.3f, false, "BaseBoards");
			baseBoards [1] = pm.BoxShape (new Vector3 (rooms [i].Pos.x + rooms [i].Scale.x - bBoardSize, rooms [i].Pos.y + bBoardSize, rooms [i].Pos.z), new Vector3 (bBoardSize, bBoardSize, rooms [i].Scale.z), true, mat, 0.3f, false, "BaseBoards");
			baseBoards [2] = pm.BoxShape (new Vector3 (rooms [i].Pos.x, rooms [i].Pos.y + bBoardSize, rooms [i].Pos.z - rooms[i].Scale.z + bBoardSize), new Vector3 (rooms [i].Scale.x, bBoardSize, bBoardSize), true, mat, 0.3f, false, "BaseBoards");
			baseBoards [3] = pm.BoxShape (new Vector3 (rooms [i].Pos.x, rooms [i].Pos.y + bBoardSize, rooms [i].Pos.z + rooms[i].Scale.z - bBoardSize), new Vector3 (rooms [i].Scale.x, bBoardSize, bBoardSize), true, mat, 0.3f, false, "BaseBoards");

			cornerBeams [0] = pm.BoxShape (new Vector3 (rooms [i].Pos.x - rooms [i].Scale.x + cBeamSize, rooms [i].Pos.y + rooms[i].WallHeight, rooms [i].Pos.z - rooms[i].Scale.z + cBeamSize), new Vector3 (cBeamSize, rooms[i].WallHeight, cBeamSize), true, mat, 0.3f, false, "BaseBoards");
			cornerBeams [1] = pm.BoxShape (new Vector3 (rooms [i].Pos.x + rooms [i].Scale.x - cBeamSize, rooms [i].Pos.y + rooms[i].WallHeight, rooms [i].Pos.z - rooms[i].Scale.z + cBeamSize), new Vector3 (cBeamSize, rooms[i].WallHeight, cBeamSize), true, mat, 0.3f, false, "BaseBoards");
			cornerBeams [2] = pm.BoxShape (new Vector3 (rooms [i].Pos.x - rooms [i].Scale.x + cBeamSize, rooms [i].Pos.y + rooms[i].WallHeight, rooms [i].Pos.z + rooms[i].Scale.z - cBeamSize), new Vector3 (cBeamSize, rooms[i].WallHeight, cBeamSize), true, mat, 0.3f, false, "BaseBoards");
			cornerBeams [3] = pm.BoxShape (new Vector3 (rooms [i].Pos.x + rooms [i].Scale.x - cBeamSize, rooms [i].Pos.y + rooms[i].WallHeight, rooms [i].Pos.z + rooms[i].Scale.z - cBeamSize), new Vector3 (cBeamSize, rooms[i].WallHeight, cBeamSize), true, mat, 0.3f, false, "BaseBoards");

			for (int n = 0; n < rooms [i].doors.Count; n++) {
				GameObject[] doorFrame = new GameObject[4];
				switch (rooms [i].doors [n].edge) {
				case Helpers.EDGES.TOP:
					doorFrame [0] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x - DoorWidth/2f, rooms [i].Pos.y + DoorHeight, rooms [i].doors [n].Pos.z), new Vector3 (bBoardSize, DoorHeight, bBoardSize* 2f), true, mat, 0.3f, false, "Door Frame");
					doorFrame [1] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x + DoorWidth/2f, rooms [i].Pos.y + DoorHeight, rooms [i].doors [n].Pos.z), new Vector3 (bBoardSize, DoorHeight, bBoardSize* 2f), true, mat, 0.3f, false, "Door Frame");
					doorFrame [2] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + DoorHeight * 2f, rooms [i].doors [n].Pos.z), new Vector3 (DoorWidth/2f + bBoardSize, bBoardSize, bBoardSize* 3f), true, mat, 0.3f, false, "Door Frame");
					doorFrame [3] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + bBoardSize, rooms [i].doors [n].Pos.z), new Vector3 (DoorWidth/2f + bBoardSize, bBoardSize, bBoardSize), true, mat, 0.3f, false, "Door Frame");
					break;
				case Helpers.EDGES.BOTTOM:
					doorFrame [0] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x - DoorWidth/2f, rooms [i].Pos.y + DoorHeight, rooms [i].doors [n].Pos.z), new Vector3 (bBoardSize, DoorHeight, bBoardSize* 2f), true, mat, 0.3f, false, "Door Frame");
					doorFrame [1] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x + DoorWidth/2f, rooms [i].Pos.y + DoorHeight, rooms [i].doors [n].Pos.z), new Vector3 (bBoardSize, DoorHeight, bBoardSize* 2f), true, mat, 0.3f, false, "Door Frame");
					doorFrame [2] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + DoorHeight * 2f, rooms [i].doors [n].Pos.z), new Vector3 (DoorWidth/2f + bBoardSize, bBoardSize, bBoardSize* 3f), true, mat, 0.3f, false, "Door Frame");
					doorFrame [3] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + bBoardSize, rooms [i].doors [n].Pos.z), new Vector3 (DoorWidth/2f + bBoardSize, bBoardSize, bBoardSize), true, mat, 0.3f, false, "Door Frame");
					break;
				case Helpers.EDGES.LEFT:
					doorFrame [0] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + DoorHeight, rooms [i].doors [n].Pos.z - DoorWidth/2f), new Vector3 (bBoardSize * 2f, DoorHeight, bBoardSize), true, mat, 0.3f, false, "Door Frame");
					doorFrame [1] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + DoorHeight, rooms [i].doors [n].Pos.z + DoorWidth/2f), new Vector3 (bBoardSize * 2f, DoorHeight, bBoardSize), true, mat, 0.3f, false, "Door Frame");
					doorFrame [2] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + DoorHeight * 2f, rooms [i].doors [n].Pos.z), new Vector3 (bBoardSize * 3f, bBoardSize, DoorWidth/2f + bBoardSize), true, mat, 0.3f, false, "Door Frame");
					doorFrame [3] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + bBoardSize, rooms [i].doors [n].Pos.z), new Vector3 (bBoardSize, bBoardSize, DoorWidth/2f + bBoardSize), true, mat, 0.3f, false, "Door Frame");
					break;
				case Helpers.EDGES.RIGHT:
					doorFrame [0] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + DoorHeight, rooms [i].doors [n].Pos.z - DoorWidth/2f), new Vector3 (bBoardSize * 2f, DoorHeight, bBoardSize), true, mat, 0.3f, false, "Door Frame");
					doorFrame [1] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + DoorHeight, rooms [i].doors [n].Pos.z + DoorWidth/2f), new Vector3 (bBoardSize * 2f, DoorHeight, bBoardSize), true, mat, 0.3f, false, "Door Frame");
					doorFrame [2] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + DoorHeight * 2f, rooms [i].doors [n].Pos.z), new Vector3 (bBoardSize * 3f, bBoardSize, DoorWidth/2f + bBoardSize), true, mat, 0.3f, false, "Door Frame");
					doorFrame [3] = pm.BoxShape (new Vector3 (rooms [i].doors [n].Pos.x, rooms [i].Pos.y + bBoardSize, rooms [i].doors [n].Pos.z), new Vector3 (bBoardSize, bBoardSize, DoorWidth/2f + bBoardSize), true, mat, 0.3f, false, "Door Frame");
					break;
				}

			}

		}
	}
}
