using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reset
{
    class Field
    {
        //Represents the entire marching board
        public static float STANDARD_STEP_SIZE = 2f; //(1.6 for 8:5) (2 for 10:5) [How many steps to travel 1 yard]

        public static float FIELD_LENGTH = 100 * STANDARD_STEP_SIZE; //These units are yards * step size = steps
        //public static float FIELD_WIDTH = 160 * STANDARD_STEP_SIZE / 3; //Note rounding issues
        public static float FIELD_WIDTH = 52.5f * STANDARD_STEP_SIZE; //Note rounding issues
        public static float SIDELINE_TO_HASH_DIST = 20 * STANDARD_STEP_SIZE;
        //public static float HASH_DIST = 40 * STANDARD_STEP_SIZE / 3; //Note rounding issues
        public static float HASH_DIST = 12.5f * STANDARD_STEP_SIZE; //Note rounding issues
        public static int X_EXCESS = 0; //Steps beyond the endzone in any one side
        public static int Y_EXCESS = 0; //Steps beyond the sideline in any one side

        public static float DRAW_SCALE_FACTOR = 8.0f;

        public static void Recalculate()
        {
            FIELD_LENGTH = 100 * STANDARD_STEP_SIZE;
            FIELD_WIDTH = 52.5f * STANDARD_STEP_SIZE;
            SIDELINE_TO_HASH_DIST = 20 * STANDARD_STEP_SIZE;
            HASH_DIST = 12.5f * STANDARD_STEP_SIZE;
        }
    }
}
