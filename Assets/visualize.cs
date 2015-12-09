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
	public class Move
	{
		public List<Dictionary<int, string>> moves = new List<Dictionary<int, string>> ();
	}

	public class Link
	{
		public string a;
		public string b;
	}
	public GameObject door;
	public GameObject ant;
	public string[] lines;
	public int		nb_ants;
	public int		nb_rooms;
	public Room[]		rooms;
	public GameObject[] doors;
	public GameObject[] ants;
	public int			move_index;
	public int			nb_moves;
	public Move[]		Moves;
	public Room			start_room;
	public List<Link>	links;
	public LineRenderer render;

	Vector2 find_door(string str)
	{
		for (int i = 0; i < rooms.Length; i++) {
			if (rooms[i].name == str)
				return rooms[i].coord;
		}
		return Vector2.zero;
	}

	void create_links()
	{
		render = GetComponent<LineRenderer>();
		int i = 0;
		for (int e = 0; e < links.Count; e++) {
			Vector2 a = find_door(links[e].a);
			Vector2 b = find_door(links[e].b);
			render.SetPosition(0, a);
			render.SetPosition(1, b);
			Debug.Log (i);
		}
	}

	void Start () {

		links = new List<Link> ();
		string[] link_lines = File.ReadAllLines("/nfs/zfs-student-3/users/emammadz/unity_file/file2.txt").ToArray();
		for (int i = 0; i < link_lines.Length; i++) {
			string[] tmp_links = link_lines[i].Split('-');
			Link tmp_class = new Link();
			tmp_class.a = tmp_links[0];
			tmp_class.b = tmp_links[1];
			links.Add(tmp_class);
		}
		move_index = 0;
		nb_rooms = 0;
		lines = File.ReadAllLines("/nfs/zfs-student-3/users/emammadz/unity_file/file.txt").ToArray();
		for (int i = 0; i < lines.Length; i++) {
			lines[i] = lines[i].Trim();
		}

		nb_ants = Int32.Parse(lines [1]);;
		start_room = new Room ();
		string[] tmp_line = lines[0].Split(' ');
		start_room.name = tmp_line [0];
		start_room.coord = new Vector2(Int32.Parse (tmp_line [1]), Int32.Parse (tmp_line [2]));


		ants = new GameObject[nb_ants];
		for (int i = 0; i < nb_ants; i++)
			ants[i] = (GameObject)GameObject.Instantiate(ant, start_room.coord, Quaternion.identity);

		for (int i = 2; i < lines.Length; i++) {
			if (lines[i][0] != 'L')
				nb_rooms++;
			else
			{
				if (move_index == 0)
					move_index = i;
			}
			if (i + 1 >= lines.Length)
				nb_moves = i;
		}
		rooms = new Room[nb_rooms];		

		for (int i = 2; i <= nb_rooms + 1; i++) {
			string[] tmp = lines[i].Split(' ');
			rooms[i - 2] = new Room();
			rooms[i - 2].name = tmp[0];
			rooms[i - 2].coord = new Vector2(Int32.Parse(tmp[1]), Int32.Parse(tmp[2]));
		}

		doors = new GameObject[rooms.Length];

		for (int i = 0; i < rooms.Length; i++) {
			doors[i] = (GameObject)GameObject.Instantiate(door, rooms[i].coord, Quaternion.identity);
		 }

		Moves = new Move[nb_moves - move_index + 1];
		int index = 0;

		for (; move_index < lines.Length; move_index++) {
			string[] tmp = lines[move_index].Split(' ');
			Moves[index] = new Move();
			for (int e = 0; e < tmp.Length; ++e)
			{
				Dictionary<int, string> tmp_dic = new Dictionary<int, string>();
				string[] move_split = tmp[e].Split('-');
				move_split[0] = move_split[0][1].ToString();
				tmp_dic.Add(Int32.Parse(move_split[0]) - 1, move_split[1]);
				Moves[index].moves.Add(tmp_dic);
			}
			index++;
		}

		create_links ();

		StartCoroutine (loop ());
	}

	void Update () {
		
	}

	IEnumerator loop()
	{
		for (int e = 0; e < Moves.Length; e++)
		{
			foreach (Dictionary<int, string> dic in Moves[e].moves)
			{
				for (int s = 0; s < nb_ants; s++)
				{
					if (dic.ContainsKey(s))
					{
						Debug.Log ("L" + s + "-" + dic[s] + "\n");
						for (int t = 0; t < rooms.Length; t++)
						{
							if (rooms[t].name == dic[s])
							{
								ants[s].gameObject.transform.position = new Vector3(rooms[t].coord.x, rooms[t].coord.y, 0);
								yield return new WaitForSeconds(2);
								break;
							}
						}
					}
				}
			}
			yield return 0;
		}
	}
}