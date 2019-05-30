using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AICheckers
{
    class Move
    {
        private Point source = new Point(-1, -1);
        private Point destination = new Point(-1, -1);
        private List<Point> captures = new List<Point>();
        private int score = 0;

        public Move()
        {
        }
        /// <summary>
        /// Konstruktor parametryczny Move
        /// Pobiera dwa punkty z następującymi informacjami:
        /// Gdzie dany warcab się znajduje teraz
        /// Gdzie znajdzie się po ruchu
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public Move(Point source, Point destination)
        {
            this.source = source;
            this.destination = destination;
        }

        /// <summary>
        /// Konstruktor parametryczny klasy Move
        /// Zamienia dane położenie x i y w zmienną typu Point, która je posiada
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destinationX"></param>
        /// <param name="destinationY"></param>
        public Move(Point source, int destinationX, int destinationY)
            : this(source, new Point(destinationX, destinationY))
        {
        }
        /// <summary>
        /// Konsturktor parametryczny klasy Move
        /// Zamienia 4 inty mówiące o położeniu początku i końca, zamienia na dwie zmienne typu Point
        /// </summary>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <param name="destinationX"></param>
        /// <param name="destinationY"></param>
        public Move(int sourceX, int sourceY, int destinationX, int destinationY)
            : this(new Point(sourceX, sourceY), new Point(destinationX, destinationY))
        {
        }

        /// <summary>
        /// Wprowadzenie DI do programu
        /// Zapobiegamy zależności i tworzymy lepsze zabezpieczenia dla pól klasy
        /// </summary>
        public Point Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        public Point Destination
        {
            get { return this.destination; }
            set { this.destination = value; }
        }

        public List<Point> Captures
        {
            get { return captures; }
        }

        public int Score
        {
            get { return this.score; }
            set { this.score = value; }
        }

        /// <summary>
        /// Przeciążenie wirtualnej funkcji ToString
        /// Pomaga nam przedstawić położenie warcaba przez jego ruch
        /// Pokazuje gdzie się znajdował, a gdzie znajduje się teraz
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Source: {0}, Dest: {1}", source, destination);
        }
    }
}
