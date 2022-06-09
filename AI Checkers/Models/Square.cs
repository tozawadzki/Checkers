namespace AICheckers
{
    /// <summary>
    /// enum przypisujący wartości liczbowe do "stanu" w jakim znajduje się kwadracik
    /// może być pusty, może mieć czarnego lub czerwonego warcaba
    /// </summary>
    public enum CheckerColor
    {
        Empty,
        Red,
        Black
    }

    /// <summary>
    /// Klasa kwadracik
    /// Posiada pole determinujące rodzaj warcaba w danym miejscu oraz czy dany kwadracik jest "królewski"
    /// </summary>
    class Square
    {
        public CheckerColor Color = CheckerColor.Empty;
        public bool King = false;
    }
}
