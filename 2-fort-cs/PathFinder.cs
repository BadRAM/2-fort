namespace _2_fort_cs;

public class PathFinder
{
    public Int2D DesiredTarget;
    public Minion Minion;
    public List<Int2D> Path = new List<Int2D>();
    
    public PathFinder(Minion minion)
    {
        Minion = minion;
    }
    
    private class PathNode : IComparable<PathNode>
    {
        public Int2D Pos;
        public float Weight;
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
    
    public void FindPath(Int2D Target)
    {
        DesiredTarget = Target;
        Path.Clear();
        //Console.WriteLine($"{Minion.Template.Name} pathing from {World.PosToTilePos(Minion.Position)} to {DesiredTarget}");
        
        PathNode?[,] nodeGrid = new PathNode[World.BoardWidth,World.BoardHeight];
        
        List<PathNode> nodesToConsider = new List<PathNode>();
        
        PathNode n = new PathNode(World.PosToTilePos(Minion.Position));
        nodesToConsider.Add(n);
        
        int count = 0;
        while (true)
        {
            count++;
            
            // Abort if we ran out of options.
            if (nodesToConsider.Count == 0)
            {
                n = new PathNode(World.PosToTilePos(Minion.Position));
                Console.WriteLine("Couldn't find a path!");
                break;
            }
            
            float minWeight = float.MaxValue;
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
            if (n.Pos == DesiredTarget) break;
            
            // set cheapest node into grid
            nodeGrid[n.Pos.X,n.Pos.Y] = n;
            
            // cull other nodes that point to same tile
            nodesToConsider.RemoveAll(a => a.Pos == n.Pos);
            
            // add new nodes for consideration
            // left
            if (n.Pos.X-1 >= 0 && nodeGrid[n.Pos.X-1,n.Pos.Y] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X-1, n.Pos.Y, 1);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // right
            if (n.Pos.X+1 < World.BoardWidth && nodeGrid[n.Pos.X+1,n.Pos.Y] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X+1, n.Pos.Y, 1);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // up
            if (n.Pos.Y-1 >= 0 && nodeGrid[n.Pos.X,n.Pos.Y-1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X, n.Pos.Y-1, 1);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // down
            if (n.Pos.Y+1 < World.BoardHeight && nodeGrid[n.Pos.X,n.Pos.Y+1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X, n.Pos.Y+1, 1);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // top left
            if (   n.Pos.X - 1 > 0 
                && n.Pos.Y - 1 > 0 
                && !(World.GetTile(n.Pos.X-1, n.Pos.Y)?.IsSolid() ?? false)
                && !(World.GetTile(n.Pos.X, n.Pos.Y-1)?.IsSolid() ?? false)
                && nodeGrid[n.Pos.X - 1, n.Pos.Y - 1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X-1, n.Pos.Y-1, 1.5f);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // top right
            if (   n.Pos.X + 1 < World.BoardWidth 
                && n.Pos.Y - 1 > 0 
                && !(World.GetTile(n.Pos.X+1, n.Pos.Y)?.IsSolid() ?? false)
                && !(World.GetTile(n.Pos.X, n.Pos.Y-1)?.IsSolid() ?? false)
                && nodeGrid[n.Pos.X + 1, n.Pos.Y - 1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X+1, n.Pos.Y-1, 1.5f);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // bottom left
            if (   n.Pos.X - 1 > 0 
                && n.Pos.Y + 1 < World.BoardHeight 
                && !(World.GetTile(n.Pos.X-1, n.Pos.Y)?.IsSolid() ?? false)
                && !(World.GetTile(n.Pos.X, n.Pos.Y+1)?.IsSolid() ?? false)
                && nodeGrid[n.Pos.X - 1, n.Pos.Y + 1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X-1, n.Pos.Y+1, 1.5f);
                if (nn != null) nodesToConsider.Add(nn);
            }
            
            // bottom right
            if (   n.Pos.X + 1 < World.BoardWidth 
                && n.Pos.Y + 1 < World.BoardHeight
                && !(World.GetTile(n.Pos.X+1, n.Pos.Y)?.IsSolid() ?? false)
                && !(World.GetTile(n.Pos.X, n.Pos.Y+1)?.IsSolid() ?? false)
                && nodeGrid[n.Pos.X + 1, n.Pos.Y + 1] == null)
            {
                PathNode? nn = WeightedNode(n, n.Pos.X+1, n.Pos.Y+1, 1.5f);
                if (nn != null) nodesToConsider.Add(nn);
            }
        }
        
        //Console.WriteLine($"Found path in {count} loops, final node weight {n.Weight}");

        while (true)
        {
            Path.Insert(0, n.Pos);
            if (n.PrevNode == null) break;
            n = n.PrevNode;
        }
    }

    private PathNode? WeightedNode(PathNode prevNode, int x, int y, float weight)
    {
        PathNode n = new PathNode(new Int2D(x,y));
        n.PrevNode = prevNode;
        n.Weight = prevNode.Weight;
        n.Weight += weight;
        Structure? structure = World.GetTile(x,y);
        if (structure != null && !(structure is Door && structure.Team == Minion.Team) && !(structure is Spawner && structure.Team == Minion.Team))
        {
            n.Weight += structure.Health;
            if (structure.Team == Minion.Team) return null;
        }
        return n;
    }
    
    
    public bool TargetReached()
    {
        if (Path.Count == 0) return true;
        return World.PosToTilePos(Minion.Position) == DesiredTarget;
    }

    public Int2D NextTile()
    {
        if (Path.Count == 0) { return DesiredTarget; }
        if (World.PosToTilePos(Minion.Position) == Path[0])
        {
            Path.RemoveAt(0);
        }
        if (Path.Count == 0) { return DesiredTarget; }

        return Path[0];
    }
}