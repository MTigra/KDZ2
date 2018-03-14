using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ClassLib
{
    /// <summary>
    /// Represents geographic position and include type of coordinates, coordinate-X and coordinate-Y 
    /// </summary>
    public class Geo
    {
        /// <summary>
        /// Represents type of coordinates for this <see cref="T:ClassLib.Geo"/>
        /// </summary>
        private string type;
        /// <summary>
        /// Represents coordinate-X of this <see cref="T:ClassLib.Geo"/>
        /// </summary>
        private float x;
        /// <summary>
        /// Represents coordinate-Y of this <see cref="T:ClassLib.Geo"/>
        /// </summary>
        private float y;

        /// <summary>
        /// Create new instance of <see cref="T:ClassLib.Geo"/> with parameters.
        /// </summary>
        /// <param name="type">Type of coordinates</param>
        /// <param name="x">Coordinate-X</param>
        /// <param name="y">Coordinate-Y</param>
        public Geo(string type, float x, float y)
        {
            this.type = type;
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            string x = X.ToString(nfi);
            string y = Y.ToString(nfi);

            return "{"+ String.Format($"type={Type}, coordinates=[{X}, {Y}]")+"}";
        }

        public Geo(string line)
        {
            string pattern = @"^{type=+([a-zA-Z]+), coordinates=[[](\d+?[\.,]\d+)?, (\d+[\.,]\d+)?]}$";
            Regex rex = new Regex(pattern);
            if (rex.IsMatch(line))
            {
                var group = rex.Match((line)).Groups;
                type = group[1].Value;
                x = float.Parse(group[2].Value, CultureInfo.InvariantCulture);
                y = float.Parse(group[3].Value, CultureInfo.InvariantCulture);
            }
            else if(string.IsNullOrWhiteSpace(line))
            {
                x = 0;
                y = 0;
            }
            else
            {
                throw new FormatException("Неверно введена строкагеографических данных.");
            }
        }

        /// <summary>
        /// Gets and sets type of cordinates of this <see cref="T:ClassLib.Geo"/>
        /// </summary>
        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        /// <summary>
        /// Gets  and sets coordinate-X of this <see cref="T:ClassLib.Geo"/>
        /// </summary>
        public float X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        /// <summary>
        /// Gets and sets coordinate-Y of this <see cref="T:ClassLib.Geo"/>
        /// </summary>
        public float Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }
        

    }
}
