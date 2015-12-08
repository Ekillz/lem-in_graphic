using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System;


public class visualize : MonoBehaviour {


	public class Room
	{
		public string name;
		public Vector2 coord;
	}
	public GameObject door;
	public string[] lines;
	public int		nb_ants;
	public int		nb_rooms;
	public Room[]		rooms;	
	// Use this for initialization
	
	void Start () {
		nb_rooms = 0;
		lines = File.ReadAllLines("/nfs/zfs-student-3/users/emammadz/unity_file/file.txt").ToArray();
		for (int i = 0; i < lines.Length; i++) {
			lines[i].Trim();
		}
		nb_ants = lines [0] [0] - 0;
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i][0] != 'L')
				nb_rooms++;
		}
		rooms = new Room[nb_rooms - 1];		
		for (int i = 1; i  < nb_rooms; i++) {
			string[] tmp = lines[i].Split(' ');
			rooms[i - 1] = new Room();
			rooms[i - 1].name = tmp[0];
			rooms[i - 1].coord = new Vector2(Int32.Parse(tmp[1]), Int32.Parse(tmp[2]));
		}
		Debug.Log (rooms [2].coord);
		for (int i = 0; i < rooms.Length; i++) {
			GameObject.Instantiate(door, rooms[i].coord, Quaternion.identity);
		 }
		Debug.Log (nb_rooms);
	}

	void Update () {
		
	}
}
