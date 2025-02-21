using System.Numerics;
using ZeroElectric.Vinculum;

namespace _2_fort_cs;

public class BroodMinionTemplate : MinionTemplate
{
    public double SpawnInterval;
    public int SpawnsOnDeath;
    public MinionTemplate SpawnedMinion;
    
    public BroodMinionTemplate(string id, string name, string description, Texture texture, double maxHealth, double armor, double damage, double speed, float physicsRadius, double spawnInterval, int spawnsOnDeath, MinionTemplate spawnedMinion, double attackCooldown = 1) 
        : base(id, name, description, texture, maxHealth, armor, damage, speed, physicsRadius, attackCooldown)
    {
        SpawnInterval = spawnInterval;
        SpawnsOnDeath = spawnsOnDeath;
        SpawnedMinion = spawnedMinion;
    }
    
    public override void Instantiate(Vector2 position, Team team, NavPath navPath)
    {
        Minion m = new BroodMinion(this, team, position, navPath);
        World.Minions.Add(m);
        World.Sprites.Add(m);
    }
}
    
public class BroodMinion : Minion
{
    private double _lastSpawnTime;
    private BroodMinionTemplate _template;
        
    public BroodMinion(BroodMinionTemplate template, Team team, Vector2 position, NavPath navPath) : base(template, team, position, navPath)
    {
        _lastSpawnTime = Time.Scaled;
        _template = template;
    }

    public override void Update()
    {
        base.Update();
        if (_template.SpawnInterval > 0 && Time.Scaled - _lastSpawnTime >= _template.SpawnInterval)
        {
            _lastSpawnTime = Time.Scaled;
            _template.SpawnedMinion.Instantiate(Position, Team, NavPath.Clone());
        }
    }

    public override void Die()
    {
        base.Die();
        for (int i = 0; i < _template.SpawnsOnDeath; i++)
        {
            // new Vector2((float)(Random.Shared.NextDouble()-0.5), (float)(Random.Shared.NextDouble()-0.5))
            _template.SpawnedMinion.Instantiate(Position, Team, null);
        }
    }
}