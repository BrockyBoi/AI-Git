using System;
using UnityEngine;

public struct Room
{
	public int l, t, r, b; // Left, Top, Right, Bottom

	public void Clear()
	{
		l = Map.width;
		r = 0;
		t = Map.height;
		b = 0;
	}

	public Room(int l1, int t1, int r1, int b1)
	{
		l = l1;
		t = t1;
		r = r1;
		b = b1;
	}

	public bool Empty()
	{
		return l >= r || t <= b;
	}

	public override string ToString()
	{
		return "[(" + l + ", " + t + "), (" + r + ", " + b + ")]";
	}

	public override bool Equals(object obj) {
		return base.Equals(obj) && l == ((Room)obj).l &&  t == ((Room)obj).t
			&& r == ((Room)obj).r &&  b == ((Room)obj).b;
	}

	public static Room operator+(Room a, Room b)
	{
		return new Room (Mathf.Min (a.l, b.l), Mathf.Max (a.t, b.t), Math.Max (a.r, b.r), Mathf.Min (a.b, b.b));
	}

	public static bool operator ==(Room a, Room b) {
		return (a.l == b.l) && (a.t == b.t) && (a.r == b.r) && (a.b == b.b);
	}

	public static bool operator !=(Room a, Room b) {
		return !(a == b);
	}

	public override int GetHashCode() {
		return l + (r << 8) + (t << 16) + (b << 24);
	}

}
