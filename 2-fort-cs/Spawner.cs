using ZeroElectric.Vinculum;

namespace _2_fort_cs;

public class SpawnerTemplate : StructureTemplate
{
    public MinionTemplate Minion;
    public int WaveSize;
    public double WaveGrowth;
    public double TimeBetweenSpawns;
    
    public SpawnerTemplate(string name, Texture texture, double maxHealth, double price, int levelRequirement, MinionTemplate minion, int waveSize, double waveGrowth, double timeBetweenSpawns) : base(name, texture, maxHealth, price, levelRequirement)
    {
        Minion = minion;
        WaveSize = waveSize;
        WaveGrowth = waveGrowth;
        TimeBetweenSpawns = timeBetweenSpawns;
    }

    public override Spawner Instantiate(int x, int y)
    {
        return new Spawner(this, x, y);
    }
}

public class Spawner : Structure
{
    private SpawnerTemplate _template;
    private double _lastSpawnTime;
    private int _spawnsRemaining;
    private Int2D _targetTile;
    private Path _path;
    
    public Spawner(SpawnerTemplate template, int x, int y) : base(template, x, y)
    {
        _template = template;
        _path = new Path(new Int2D(x, y), new Int2D(x, y), Team);
    }
    
    public override void Update()
    {
        base.Update();
        if (_spawnsRemaining > 0 && Time.Scaled - _lastSpawnTime > _template.TimeBetweenSpawns)
        {
            _template.Minion.Instantiate(position, Team, _path.Clone());
            _spawnsRemaining--;
            _lastSpawnTime = Time.Scaled;
        }
    }

    public override void PreWaveEffect()
    {
        Retarget();
        _path.Reset();
        _path.Destination = _targetTile;
        PathFinder.RequestPath(_path);
    }
    
    public override void WaveEffect()
    {
        _spawnsRemaining += _template.WaveSize + (int)(World.Wave * _template.WaveGrowth);
    }

    public override bool IsSolid()
    {
        return true;
    }

    private void Retarget()
    {
        List<Int2D> targets = new List<Int2D>();
        
        for (int x = 0; x < World.BoardWidth; ++x)
        {
            for (int y = 0; y < World.BoardHeight; ++y)
            {
                if (World.GetTile(x,y) != null && World.GetTile(x,y).Team != Team)
                {
                    targets.Add(new Int2D(x,y));
                }
            }
        }

        if (targets.Count == 0)
        {
            return;
        }

        _targetTile = targets[Random.Shared.Next(targets.Count)];
    }
}