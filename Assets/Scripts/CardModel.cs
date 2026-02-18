public class CardModel
{
    public int Id { get; private set; }
    public bool IsRevealed { get; private set; }
    public bool IsMatched { get; private set; }

    public CardModel(int id)
    {
        Id = id;
    }

    public void Reveal()
    {
        if (IsMatched) return;
        IsRevealed = true;
    }

    public void Hide()
    {
        if (IsMatched) return;
        IsRevealed = false;
    }

    public void Match()
    {
        IsMatched = true;
        IsRevealed = true;
    }
}