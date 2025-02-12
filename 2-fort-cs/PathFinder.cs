using System.Numerics;

namespace _2_fort_cs;

public static class PathFinder
{
    private static List<Path> _pathQueue = new List<Path>();
    
    private class PathNode : IComparable<PathNode>
    {
        public Int2D Pos;
        public double Weight;
        //public float TotalWeight;
        public PathNode? PrevNode;
        
        public PathNode(Int2D pos)
        {
            Pos = pos;
        }

        public int CompareTo(PathNode? other)
        {
            return Weight.CompareTo(other?.Weight);
        }
    }

    public static void RequestPath(Path path)
    {
        _pathQueue.Add(path);
    }

    public static void ServeQueue(int max)
    {
        for (int i = 0; i < max; i++)
        {
            if (_pathQueue.Count == 0)  return;
            FindPath(_pathQueue[0]);
            _pathQueue.RemoveAt(0);
        }
    }
    
    public static void FindPath(Path path)
    {
        if (path.Found)
        {
            Console.WriteLine($"FindPath called on a path that's already found, something's wrong.");
            return;
        }
        
        //Console.WriteLine($"{Minion.Template.Name} pathing from {World.PosToTilePos(Minion.Position)} to {DesiredTarget}");
        
        PathNode?[,] nodeGrid = new PathNode[World.BoardWidth,World.BoardHeight];
        
        List<PathNode> nodesToConsider = new List<PathNode>();
        
        PathNode n = new PathNode(path.Start);
        nodesToConsider.Add(n);
        
        int count = 0;
        while (true)
        {
            count++;
            
            // Abort if we ran out of options.
            if (nodesToConsider.Count == 0)
            {
                n = new PathNode(path.Start);
                Console.WriteLine("Couldn't find a path!");
                break;
            }
            
            double minWeight = double.MaxValue;
            int mindex = 0;
            for (int i = 0; i < nodesToConsider.Count; i++)
            {
                if (nodesToConsider[i].Weight < minWeight)
                {
                    minWeight = nodesToConsider[i].Weight;
                    mindex = i;
                }
            }
            n = nodesToConsider[mindex];
            nodesToConsider.RemoveAt(mindex);
            
            // Break if we're done
            if (n.Pos == path.Destination) break;
            
            // set cheapest node into grid
            nodeGrid[n.Pos.X,n.Pos.Y] = n;
            
            // cull other nodes that point to same tile
            nodesToConsider.RemoveAll(a => a.Pos == n.Pos);
            
            // add new nodes for consideration
            // left
            if (n.Pos.X-1 >= 0 && nodeGrid[n.Pos.X-1,n.Pos.Y] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X-1, n.Pos.Y, 1, path.Team);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // right
            if (n.Pos.X+1 < World.BoardWidth && nodeGrid[n.Pos.X+1,n.Pos.Y] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X+1, n.Pos.Y, 1, path.Team);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // up
            if (n.Pos.Y-1 >= 0 && nodeGrid[n.Pos.X,n.Pos.Y-1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X, n.Pos.Y-1, 1, path.Team);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // down
            if (n.Pos.Y+1 < World.BoardHeight && nodeGrid[n.Pos.X,n.Pos.Y+1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X, n.Pos.Y+1, 1, path.Team);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // top left
            if (   n.Pos.X - 1 > 0 
                && n.Pos.Y - 1 > 0 
                && !(World.GetTile(n.Pos.X-1, n.Pos.Y)?.IsSolid() ?? false)
                && !(World.GetTile(n.Pos.X, n.Pos.Y-1)?.IsSolid() ?? false)
                && nodeGrid[n.Pos.X - 1, n.Pos.Y - 1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X-1, n.Pos.Y-1, 1.5f, path.Team);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // top right
            if (   n.Pos.X + 1 < World.BoardWidth 
                && n.Pos.Y - 1 > 0 
                && !(World.GetTile(n.Pos.X+1, n.Pos.Y)?.IsSolid() ?? false)
                && !(World.GetTile(n.Pos.X, n.Pos.Y-1)?.IsSolid() ?? false)
                && nodeGrid[n.Pos.X + 1, n.Pos.Y - 1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X+1, n.Pos.Y-1, 1.5f, path.Team);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // bottom left
            if (   n.Pos.X - 1 > 0 
                && n.Pos.Y + 1 < World.BoardHeight 
                && !(World.GetTile(n.Pos.X-1, n.Pos.Y)?.IsSolid() ?? false)
                && !(World.GetTile(n.Pos.X, n.Pos.Y+1)?.IsSolid() ?? false)
                && nodeGrid[n.Pos.X - 1, n.Pos.Y + 1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X-1, n.Pos.Y+1, 1.5f, path.Team);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // bottom right
            if (   n.Pos.X + 1 < World.BoardWidth 
                && n.Pos.Y + 1 < World.BoardHeight
                && !(World.GetTile(n.Pos.X+1, n.Pos.Y)?.IsSolid() ?? false)
                && !(World.GetTile(n.Pos.X, n.Pos.Y+1)?.IsSolid() ?? false)
                && nodeGrid[n.Pos.X + 1, n.Pos.Y + 1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X+1, n.Pos.Y+1, 1.5f, path.Team);
                if (nn != null) nodesToConsider.Add(nn);
            }
        }
        
        //Console.WriteLine($"Found path in {count} loops, final node weight {n.Weight}");

        while (true)
        {
            path.Waypoints.Insert(0, n.Pos);
            if (n.PrevNode == null) break;
            n = n.PrevNode;
        }

        path.Found = true;
    }

    private static PathNode? WeightedNode(PathNode prevNode, int x, int y, double weight, TeamName team)
    {
        PathNode n = new PathNode(new Int2D(x, y));
        n.PrevNode = prevNode;
        n.Weight = prevNode.Weight;
        n.Weight += weight;
        Structure? structure = World.GetTile(x, y);
        if (structure != null && !(structure is Door && structure.Team == team) &&
            !(structure is Spawner && structure.Team == team))
        {
            n.Weight += structure.Health;
            if (structure.Team == team) return null;
        }

        return n;
    }

    public static int GetQueueLength()
    {
        return _pathQueue.Count;
    }
}

public class Path
{
    public bool Found = false; 
    public Int2D Start;
    public Int2D Destination;
    public TeamName Team;
    public List<Int2D> Waypoints = new List<Int2D>();

    public Path(Int2D start, Int2D destination, TeamName team)
    {
        Start = start;
        Destination = destination;
        Team = team;
    }

    public Int2D NextTile(Vector2 position)
    {
        if (Waypoints.Count == 0) { return Destination; }
        if (World.PosToTilePos(position) == Waypoints[0])
        {
            Waypoints.RemoveAt(0);
        }
        if (Waypoints.Count == 0) { return Destination; }

        return Waypoints[0];
    }
    
    public bool TargetReached(Vector2 position)
    {
        if (Waypoints.Count == 0) return true;
        return World.PosToTilePos(position) == Destination;
    }
    
    public Path Clone()
    {
        Path p = new Path(Start, Destination, Team);
        p.Found = Found;
        p.Waypoints = new List<Int2D>(Waypoints);
        return p;
    }

    public void Reset()
    {
        Destination = Start;
        Found = false;
        Waypoints.Clear();
    }
}