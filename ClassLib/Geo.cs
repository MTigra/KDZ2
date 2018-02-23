using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
