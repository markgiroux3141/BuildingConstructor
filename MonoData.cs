using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoData : MonoBehaviour {

	HouseBuilder hb;
	ProceduralMesh p;

	public string FloorMat;
	public string WallMat;
	public string CeilingMat;
	public string BracingMat;
	public float MaterialTile;

	public int ComplexSize; // 3
	public float MinRoomSize; //3
	public float MaxRoomSize; // 15
	public float WallThickness; // 0.2
	public float MinWallHeight; // 3
	public float MaxWallHeight; // 20
	public float FloorThicknes; // 0.05
	public float DoorWidth; // 3
	public float DoorHeight; // 2
	public Vector3 Origin;


	// Use this for initialization
	void Start () {
		HouseBuilder hb = new HouseBuilder (FloorMat,WallMat,CeilingMat,MaterialTile,MinWallHeight,MaxWallHeight);
		hb.CreateLevel (ComplexSize, MinRoomSize, MaxRoomSize, WallThickness,FloorThicknes, DoorWidth,DoorHeight,Origin);
		Lighting.AddLights (hb.rooms);
		Bracing.AddBracing (hb.rooms, BracingMat,hb.DoorWidth,hb.DoorHeight);
		//Decorate.PopulateRooms (hb.rooms);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.X)){
			hb.AdjustRooms ();
			hb.RefreshRooms ();
		}
	}
}
