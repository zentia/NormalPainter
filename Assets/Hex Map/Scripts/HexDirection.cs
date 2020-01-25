public enum HexDirection
{
    NE, // 东北 
    E, // 东 
    SE, // 东南
    SW, // 西南 
    W, // 西
    NW, // 西北
}

public static class HexDirectionExtensions
{
    
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int) direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
}