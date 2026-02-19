public class ScoreService
{
    public int Score { get; private set; }
    public int Combo { get; private set; }

    private float _comboTimer;
    private const float ComboDuration = 3f;

    public void AddMatch()
    {
        Combo++;
        Score += 100 * Combo;
        _comboTimer = ComboDuration;
    }

    public void AddMismatch()
    {
        Score -= 10;
        Combo = 0;
    }

    public void Update(float deltaTime)
    {
        if (Combo <= 0) return;

        _comboTimer -= deltaTime;
        if (_comboTimer <= 0f)
            Combo = 0;
    }
}
