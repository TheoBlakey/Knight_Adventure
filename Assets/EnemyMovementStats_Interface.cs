public interface EnemyMovementStats_Interface
{
    float MovementSpeed
    { get; set; }

    float PercentageOfSpeedLostWhenHit
    { get; set; }

    int AmountOfTimeStoppedAfterHit
    { get; set; }

    bool HitCurrentlyCantMove
    { get; set; }

    void ActivateTimerOnBool(float timeAmount, string boolName)
    { }
}
