[System.Serializable]
public class Status
{
    public float hp;
    public float fatigue;
    public float hungry;
    public float time;
    public Status()
    {
        fatigue = 0;
        hungry = 0;
        time = 0;
    }

    public Status(float hp, float fatigue, float hungry, float time = 0)
    {
        this.hp = hp;
        this.fatigue = fatigue;
        this.hungry = hungry;
        this.time = time;
    }

    public Status(Status other)
    {
        this.hp = other.hp;
        this.fatigue = other.fatigue;
        this.hungry = other.hungry;
        this.time = other.time;
    }

    public static Status operator +(Status stat1, Status stat2)
    {
        return new Status(
            stat1.hp + stat2.hp,
            stat1.fatigue + stat2.fatigue,
            stat1.hungry + stat2.hungry,
            stat1.time + stat2.time
        );
    }
    public static Status operator -(Status stat1, Status stat2)
    {
        return new Status(
            stat1.hp - stat2.hp,
            stat1.fatigue - stat2.fatigue,
            stat1.hungry - stat2.hungry,
            stat1.time - stat2.time
        );
    }
}