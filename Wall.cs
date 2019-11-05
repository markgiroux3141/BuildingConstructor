using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall{

	public enum ORIENT
	{
		AXIS_X,
		AXIS_Z
	}

	public Vector3 Pos;
	public Vector3 Scale;
	public ORIENT orient;
	public Helpers.EDGES edge;

	public Wall(Vector3 pos, ORIENT ornt){
		Pos = pos;
		orient = ornt;
	}

	public Wall(){

	}
}
