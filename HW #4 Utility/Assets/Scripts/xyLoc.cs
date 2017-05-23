using System;

public struct xyLoc
{
	public int x, y;

	public xyLoc(int p1, int p2)
	{
		x = p1;
		y = p2;
	}

	public float SquaredDistance(float xf, float yf)
	{
		return (xf-x)*(xf-x)+(yf-y)*(yf-y);
	}

	public override string ToString()
	{
		return "(" + x + ", " + y + ")";
	}

	public override bool Equals(Object obj) {
		return base.Equals(obj) && x == ((xyLoc)obj).x &&  y == ((xyLoc)obj).y;
	}

	public static bool operator ==(xyLoc a, xyLoc b) {
		return (a.x == b.x) && (a.y == b.y);
	}

	public static bool operator !=(xyLoc a, xyLoc b) {
		return !((a.x == b.x) && (a.y == b.y));
	}

    public static xyLoc operator -(xyLoc a, xyLoc b)
    {
        return new xyLoc(a.x - b.x, a.y - b.y);
    }

    public static xyLoc operator +(xyLoc a, xyLoc b)
    {
        return new xyLoc(a.x + b.x, a.y + b.y);
    }

    public override int GetHashCode() {
		return x<<16+y;
	}

}
