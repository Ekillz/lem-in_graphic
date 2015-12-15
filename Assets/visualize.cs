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
	public Room			end_room;
	public List<Link>	links;
	public LineRenderer render;
	public GameObject	end_door;
	public int			finish_walk;
	public bool			thick;

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
		if (thick)
			render.SetWidth (0.4f, 0.4f);
		render.SetVertexCount (links.Count * 2);
		int i = 0;
		for (int e = 0; e < links.Count; e++) {
			Vector2 a = find_door(links[e].a);
			Vector2 b = find_door(links[e].b);
			render.SetPosition(i, a); 
			render.SetPosition(i + 1, b);
			i += 2;
		}
	}

	void Start () {
		thick = false;
		links = new List<Link> ();
		string[] link_lines = File.ReadAllLines ("/nfs/zfs-student-3/users/emammadz/unity_file/file2.txt").ToArray ();
		for (int i = 0; i < link_lines.Length; i++) {
			if (link_lines[i] == "thick")
				thick = true;
			else
			{
				string[] tmp_links = link_lines [i].Split ('-');
				Link tmp_class = new Link ();
				tmp_class.a = tmp_links [0];
				tmp_class.b = tmp_links [1];
				links.Add (tmp_class);
			}
		}
		move_index = 0;
		nb_rooms = 0;
		lines = File.ReadAllLines ("/nfs/zfs-student-3/users/emammadz/unity_file/file.txt").ToArray ();
		for (int i = 0; i < lines.Length; i++) {
			lines [i] = lines [i].Trim ();
		}

		nb_ants = Int32.Parse (lines [2]);
		start_room = new Room ();
		string[] tmp_line = lines [0].Split (' ');
		start_room.name = tmp_line [0];
		start_room.coord = new Vector2 (Int32.Parse (tmp_line [1]), Int32.Parse (tmp_line [2]));
		end_room = new Room ();
		string[] tmp_line2 = lines [1].Split (' ');
		end_room.name = tmp_line2 [0];
		end_room.coord = new Vector2 (Int32.Parse (tmp_line2 [1]), Int32.Parse (tmp_line2 [2]));


		ants = new GameObject[nb_ants];
		for (int i = 0; i < nb_ants; i++)
		{
			ants [i] = (GameObject)GameObject.Instantiate (ant, start_room.coord, Quaternion.identity);
			ants [i].GetComponent<SpriteRenderer>().sortingOrder = i + 1;
		}
		for (int i = 3; i < lines.Length; i++) {
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

		for (int i = 3; i <= nb_rooms + 2; i++) {
			string[] tmp = lines[i].Split(' ');
			rooms[i - 3] = new Room();
			rooms[i - 3].name = tmp[0];
			rooms[i - 3].coord = new Vector2(Int32.Parse(tmp[1]), Int32.Parse(tmp[2]));
		}

		doors = new GameObject[rooms.Length];

		for (int i = 0; i < rooms.Length; i++) {
			if (rooms[i].coord == end_room.coord)
				doors[i] = (GameObject)GameObject.Instantiate(end_door, rooms[i].coord, Quaternion.identity);
			else
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
				move_split[0] = move_split[0].Remove(0, 1);
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

	IEnumerator move_ants(int s, int t)
	{
		float journeyLength = Vector3.Distance(ants[s].gameObject.transform.position, new Vector3(rooms[t].coord.x, rooms[t].coord.y, 0));
		float distCovered = 1f;
		float fracJourney = distCovered / journeyLength;
		while (fracJourney <= 1)
		{
			distCovered = 1.0f;
			fracJourney = distCovered / journeyLength;
			journeyLength = Vector3.Distance(ants[s].gameObject.transform.position, new Vector3(rooms[t].coord.x, rooms[t].coord.y, 0));
			ants[s].gameObject.transform.position = Vector3.Lerp(ants[s].gameObject.transform.position, new Vector3(rooms[t].coord.x, rooms[t].coord.y, 0), fracJourney);
			yield return new WaitForSeconds(1f / nb_ants);
		}
		finish_walk++;
	}

	IEnumerator loop()
	{
		int counter = 0;
		finish_walk = 0;
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
								StartCoroutine(move_ants(s, t));
								break;
							}
						}
					}
				}
				counter++;
			}
			while (finish_walk != counter)
				yield return new WaitForSeconds(0.1f);
			finish_walk = 0;
			counter = 0;
		}
	}
}