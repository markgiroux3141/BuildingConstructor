using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMesh{

	public ProceduralMesh(){
		
	}

	public GameObject BoxShape(Vector3 pos, Vector3 scale, bool tileUvs, string material, float matTileSize, bool collision, string name){
		
		GameObject go = new GameObject (name);

		MeshFilter mf;
		MeshRenderer mr;
		Material mat;

		Vector3[] vertices;
		int[] indices;
		Vector2[] uvs;

		mf = go.AddComponent<MeshFilter> ();
		mr = go.AddComponent<MeshRenderer> ();
		vertices = new Vector3[24];
		indices = new int[36];
		uvs = new Vector2[24];

		SetBoxArraysDefault (vertices, indices, uvs);

		for (int i = 0; i < vertices.Length; i++) {
			vertices [i].Scale (scale);
		}

		if (tileUvs) {
			for (int i = 0; i < 4; i++) {
				Vector2 xy = new Vector2 (scale.x, scale.y);
				Vector2 yz = new Vector2 (scale.z, scale.y);
				Vector2 xz = new Vector2 (scale.x, scale.z);
				Vector2 zx = new Vector2 (scale.z, scale.x);

				uvs [i].Scale (xz * matTileSize);
				uvs [i + 4].Scale (xy * matTileSize);
				uvs [i + 8].Scale (yz * matTileSize);
				uvs [i + 12].Scale (xy * matTileSize);
				uvs [i + 16].Scale (yz * matTileSize);
				uvs [i + 20].Scale (zx * matTileSize);
			}
		}

		mf.mesh.vertices = vertices;
		mf.mesh.triangles = indices;
		mf.mesh.uv = uvs;

		mf.mesh.RecalculateNormals ();

		mr.material = (Material)Resources.Load ("Materials/" + material, typeof(Material));

		if (collision) {
			go.AddComponent<BoxCollider> ();
		}
	
		go.transform.position = pos;

		return go;

	}

	private void SetBoxArraysDefault(Vector3[] vertices, int[] indices, Vector2[] uvs){
		
		vertices [0] = new Vector3 (-1, 1, 1);vertices [1] = new Vector3 (1, 1, 1);
		vertices [2] = new Vector3 (1, 1, -1);vertices [3] = new Vector3 (-1, 1, -1);
		vertices [4] = new Vector3 (-1, 1, 1);vertices [5] = new Vector3 (1, 1, 1);
		vertices [6] = new Vector3 (1, -1, 1);vertices [7] = new Vector3 (-1, -1, 1);
		vertices [8] = new Vector3 (1, 1, 1);vertices [9] = new Vector3 (1, 1, -1);
		vertices [10] = new Vector3 (1, -1, -1);vertices [11] = new Vector3 (1, -1, 1);
		vertices [12] = new Vector3 (-1, 1, -1);vertices [13] = new Vector3 (1, 1, -1);
		vertices [14] = new Vector3 (1, -1, -1);vertices [15] = new Vector3 (-1, -1, -1);
		vertices [16] = new Vector3 (-1, 1, 1);vertices [17] = new Vector3 (-1, 1, -1);
		vertices [18] = new Vector3 (-1, -1, -1);vertices [19] = new Vector3 (-1, -1, 1);
		vertices [20] = new Vector3 (-1, -1, 1);vertices [21] = new Vector3 (-1, -1, -1);
		vertices [22] = new Vector3 (1, -1, -1);vertices [23] = new Vector3 (1, -1, 1);

		indices [0] = 0;indices [1] = 1;indices [2] = 3;indices [3] = 1;indices [4] = 2;indices [5] = 3;
		indices [6] = 4;indices [7] = 7;indices [8] = 5;indices [9] = 7;indices [10] = 6;indices [11] = 5;
		indices [12] = 8;indices [13] = 10;indices [14] = 9;indices [15] = 8;indices [16] = 11;indices [17] = 10;
		indices [18] = 14;indices [19] = 12;indices [20] = 13;indices [21] = 15;indices [22] = 12;indices [23] = 14;
		indices [24] = 18;indices [25] = 16;indices [26] = 17;indices [27] = 19;indices [28] = 16;indices [29] = 18;
		indices [30] = 23;indices [31] = 21;indices [32] = 22;indices [33] = 23;indices [34] = 20;indices [35] = 21;

		uvs [0] = new Vector2 (0, 0);uvs [1] = new Vector2 (1, 0);uvs [2] = new Vector2 (1, 1);uvs [3] = new Vector2 (0, 1);
		uvs [4] = new Vector2 (0, 0);uvs [5] = new Vector2 (1, 0);uvs [6] = new Vector2 (1, 1);uvs [7] = new Vector2 (0, 1);
		uvs [8] = new Vector2 (0, 0);uvs [9] = new Vector2 (1, 0);uvs [10] = new Vector2 (1, 1);uvs [11] = new Vector2 (0, 1);
		uvs [12] = new Vector2 (0, 0);uvs [13] = new Vector2 (1, 0);uvs [14] = new Vector2 (1, 1);uvs [15] = new Vector2 (0, 1);
		uvs [16] = new Vector2 (0, 0);uvs [17] = new Vector2 (1, 0);uvs [18] = new Vector2 (1, 1);uvs [19] = new Vector2 (0, 1);
		uvs [20] = new Vector2 (0, 0);uvs [21] = new Vector2 (1, 0);uvs [22] = new Vector2 (1, 1);uvs [23] = new Vector2 (0, 1);
	}
		
}
