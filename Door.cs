using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door{

	public enum ORIENT
	{
		AXIS_X,
		AXIS_Z
	}

	public Vector3 Pos;
	public ORIENT orient;
	public Helpers.EDGES edge;
	public Room parentRoom;

	public Door(Vector3 pos, ORIENT ornt){
		Pos = pos;
		orient = ornt;
	}

	public Door(){

	}
}
