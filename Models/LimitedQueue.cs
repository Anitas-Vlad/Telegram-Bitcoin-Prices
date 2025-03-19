namespace TelegramBitcoinPrices.Models;

public class LimitedQueue : Queue<decimal>
{
    public int Limit { get; set; }

    public LimitedQueue(int limit)
    {
        Limit = limit;
    }

    public void Enqueue(decimal value)
    {
        if (Count >= Limit)
            Dequeue();

        base.Enqueue(value);
    }
}