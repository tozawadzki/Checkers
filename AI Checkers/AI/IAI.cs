namespace AICheckers
{
    interface IAI
    {
        CheckerColor Color { get; set; }
        Move Process(Square[,] Board);
    }
}
