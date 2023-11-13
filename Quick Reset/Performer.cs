using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reset
{
    struct Set
    {
        public string set;
        public int counts;

        public int privateID; //For indexing purposes

        public Set(string setName, int setCounts, int privID)
        {
            set = setName;
            counts = setCounts;
            privateID = privID;
        }

        public override string ToString()
        {
            return set + "(" + counts + ")";
        }
    }
    enum FieldReference
    {
        BACK_SIDELINE,
        BOTTOM_BACK_NUMBERS,
        TOP_BACK_NUMBERS,
        BACK_HASH,
        FRONT_HASH,
        TOP_FRONT_NUMBERS,
        BOTTOM_FRONT_NUMBERS,
        FRONT_SIDELINE
    }
    struct Dot
    {
        public float lefttoright; //0 is on the 50 yard line. Positive is towards side 2
        public float fronttoback; //0 is halfway in the field. Positive is away from the audience

        public static bool USE_NUMBERS_AS_REFERENCES = true; //for FrontToBackToString() and similar methods

        public static Dictionary<FieldReference, float> referencePositions = new Dictionary<FieldReference, float>()
        {
            { FieldReference.BACK_SIDELINE, 26.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.BOTTOM_BACK_NUMBERS, 19.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.TOP_BACK_NUMBERS, 17.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.BACK_HASH, 6.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.FRONT_HASH, -6.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.TOP_FRONT_NUMBERS, -17.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.BOTTOM_FRONT_NUMBERS, -19.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.FRONT_SIDELINE, -26.25f * Field.STANDARD_STEP_SIZE }
        };
        public static Dictionary<FieldReference, float> referencePositionsNoNumbers = new Dictionary<FieldReference, float>()
        {
            { FieldReference.BACK_SIDELINE, 26.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.BACK_HASH, 6.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.FRONT_HASH, -6.25f * Field.STANDARD_STEP_SIZE },
            { FieldReference.FRONT_SIDELINE, -26.25f * Field.STANDARD_STEP_SIZE }
        };

        public static float LeftToRightFromString(string lr)
        {
            float ret;
            int sideNum;
            int baseYardLine;
            float deviation;
            string[] lrArgs = lr.Split(' ').ToList().Where(x => !x.Contains("step") && !string.IsNullOrEmpty(x)).ToArray();
            lrArgs[1] = lrArgs[1].Replace(':', '\0');
            if (lrArgs[0] == "side" && int.TryParse(lrArgs[1].Remove(1), out sideNum))
            {
                if (sideNum != 1 && sideNum != 2)
                {
                    Console.WriteLine("INVALID LEFT TO RIGHT SIDE NUMBER: " + sideNum);
                    throw new Exception();
                }
                if (lrArgs[2] == "on")
                {
                    if (int.TryParse(lrArgs[3], out baseYardLine))
                    {
                        //ret = (10 - (baseYardLine / 5)) * 8;
                        ret = Field.STANDARD_STEP_SIZE * (50 - baseYardLine);
                        if (sideNum == 1) ret = -ret;
                        return ret;
                    }
                    else
                    {
                        Console.WriteLine("INVALID LEFT TO RIGHT YARD LINE: " + lr);
                        throw new Exception();
                    }
                }
                else if (float.TryParse(lrArgs[2], out deviation))
                {
                    if (int.TryParse(lrArgs[4], out baseYardLine))
                    {
                        //ret = (10 - (baseYardLine / 5)) * 8;
                        ret = Field.STANDARD_STEP_SIZE * (50 - baseYardLine);
                    }
                    else
                    {
                        Console.WriteLine("INVALID LEFT TO RIGHT YARD LINE: " + lr);
                        throw new Exception();
                    }

                    if (lrArgs[3] == "out" || lrArgs[3] == "outside")
                    {
                        ret += deviation;
                    }
                    else if (lrArgs[3] == "in" || lrArgs[3] == "inside")
                    {
                        ret -= deviation;
                    }
                    else
                    {
                        Console.WriteLine("INVALID LEFT TO RIGHT DEVIATION: " + lr);
                    }

                    if (sideNum == 1) ret = -ret;
                    return ret;
                }
                else
                {
                    Console.WriteLine("INVALID LEFT TO RIGHT YARD LINE: " + lr);
                    throw new Exception();
                }
            }
            else if (lr == "on 50 yard line")
            {
                return 0.0f;
            }
            else if (lr == "on 50")
            {
                return 0.0f;
            }
            else
            {
                Console.WriteLine("INVALID LEFT TO RIGHT SIDE: " + lr);
                throw new Exception();
            }
        }

        public static float FrontToBackFromString(string fb)
        {
            float ret;
            float deviation;
            string[] fbArgs = fb.Split(' ').ToList().Where(x => !x.Contains("step") && !string.IsNullOrEmpty(x)).ToArray();
            
            //Sometimes the front to back instruction has parentheses...
            //2.0 BEHIND FRONT HASH (TOP OF FRONT NUMBERS)
            //...This just means "2.0 BEHIND TOP OF FRONT NUMBERS". Why.
            if (fb.Contains('('))
            {
                //Just cut off the "original" reference
                int realIndex = Array.IndexOf(fbArgs, fbArgs.First(x => x.Contains('(')));

                int frontIdx = Array.IndexOf(fbArgs, "front");
                //Because "in front" exists...
                if (frontIdx != -1 && fbArgs[frontIdx - 1] == "in")
                {
                    frontIdx = Array.IndexOf(fbArgs, "front", frontIdx + 1);
                }
                int backIdx = Array.IndexOf(fbArgs, "back");
                int topIdx = Array.IndexOf(fbArgs, "top");
                int bottomIdx = Array.IndexOf(fbArgs, "bottom");
                int fakeIndex = (new int[] { frontIdx, backIdx, topIdx, bottomIdx }).Where(x => x != -1).Min();

                List<string> listFbArgs = fbArgs.ToList();
                int uselessEntries = realIndex - fakeIndex;
                while (uselessEntries > 0)
                {
                    listFbArgs.RemoveAt(fakeIndex);
                    uselessEntries--;
                }
                fbArgs = listFbArgs.ToArray();

                for (int i = 0; i < fbArgs.Length; i++)
                {
                    fbArgs[i] = fbArgs[i].Replace("(", "");
                    fbArgs[i] = fbArgs[i].Replace(")", "");
                }
            }

            if (fbArgs[0] == "on")
            {
                if (fbArgs[2] == "hash")
                {
                    if (fbArgs[1] == "front")
                    {
                        ret = referencePositions[FieldReference.FRONT_HASH];
                        return ret;
                    }
                    else if (fbArgs[1] == "back")
                    {
                        ret = referencePositions[FieldReference.BACK_HASH];
                        return ret;
                    }
                    else
                    {
                        Console.WriteLine("INVALID FRONT TO BACK HASH: " + fb);
                        throw new Exception();
                    }
                }
                else if (fbArgs[2] == "sideline" || (fbArgs[2] == "side" && fbArgs[3] == "line"))
                {
                    if (fbArgs[1] == "front")
                    {
                        ret = referencePositions[FieldReference.FRONT_SIDELINE];
                        return ret;
                    }
                    else if (fbArgs[1] == "back")
                    {
                        ret = referencePositions[FieldReference.BACK_SIDELINE];
                        return ret;
                    }
                    else
                    {
                        Console.WriteLine("INVALID FRONT TO BACK SIDELINE: " + fb);
                        throw new Exception();
                    }
                }
                else if (fbArgs[1] == "top" || fbArgs[1] == "bottom")
                {
                    if (fbArgs[3] == "front")
                    {
                        if (fbArgs[1] == "bottom")
                        {
                            ret = referencePositions[FieldReference.BOTTOM_FRONT_NUMBERS];
                            return ret;
                        }
                        else if (fbArgs[1] == "top")
                        {
                            ret = referencePositions[FieldReference.TOP_FRONT_NUMBERS];
                            return ret;
                        }
                        else
                        {
                            Console.WriteLine("INVALID FRONT TO BACK NUMBERS: " + fb);
                            throw new Exception();
                        }
                    }
                    else if (fbArgs[3] == "back")
                    {
                        if (fbArgs[1] == "bottom")
                        {
                            ret = referencePositions[FieldReference.BOTTOM_BACK_NUMBERS];
                            return ret;
                        }
                        else if (fbArgs[1] == "top")
                        {
                            ret = referencePositions[FieldReference.TOP_BACK_NUMBERS];
                            return ret;
                        }
                        else
                        {
                            Console.WriteLine("INVALID FRONT TO BACK NUMBERS: " + fb);
                            throw new Exception();
                        }
                    }
                    else
                    {
                        Console.WriteLine("INVALID FRONT TO BACK NUMBERS: " + fb);
                        throw new Exception();
                    }
                }
                else
                {
                    Console.WriteLine("INVALID FRONT TO BACK REFERENCE: " + fb);
                    throw new Exception();
                }
            }
            else if (float.TryParse(fbArgs[0], out deviation))
            {
                bool isInFront;
                int argBasePos;
                if (fbArgs[1] == "in" && fbArgs[2] == "front")
                {
                    isInFront = true;
                    argBasePos = 3;
                }
                else if (fbArgs[1] == "behind")
                {
                    isInFront = false;
                    argBasePos = 2;
                }
                else
                {
                    Console.WriteLine("INVALID FRONT TO BACK DEVIATION: " + fb);
                    throw new Exception();
                }

                float baseReference = 0;
                //Get base
                if (fbArgs[argBasePos + 1] == "hash")
                {
                    if (fbArgs[argBasePos] == "front")
                    {
                        baseReference = referencePositions[FieldReference.FRONT_HASH];
                    }
                    else if (fbArgs[argBasePos] == "back")
                    {
                        baseReference = referencePositions[FieldReference.BACK_HASH];
                    }
                    else
                    {
                        Console.WriteLine("INVALID FRONT TO BACK HASH: " + fb);
                        throw new Exception();
                    }
                }
                else if (fbArgs[argBasePos + 1] == "sideline" || (fbArgs[argBasePos + 1] == "side" && fbArgs[argBasePos + 2] == "line"))
                {
                    if (fbArgs[argBasePos] == "front")
                    {
                        baseReference = referencePositions[FieldReference.FRONT_SIDELINE];
                    }
                    else if (fbArgs[argBasePos] == "back")
                    {
                        baseReference = referencePositions[FieldReference.BACK_SIDELINE];
                    }
                    else
                    {
                        Console.WriteLine("INVALID FRONT TO BACK SIDELINE: " + fb);
                        throw new Exception();
                    }
                }
                else if (fbArgs[argBasePos] == "top" || fbArgs[argBasePos] == "bottom")
                {
                    if (fbArgs[argBasePos + 2] == "front")
                    {
                        if (fbArgs[argBasePos] == "bottom")
                        {
                            baseReference = referencePositions[FieldReference.BOTTOM_FRONT_NUMBERS];
                        }
                        else if (fbArgs[argBasePos] == "top")
                        {
                            baseReference = referencePositions[FieldReference.TOP_FRONT_NUMBERS];
                        }
                        else
                        {
                            Console.WriteLine("INVALID FRONT TO BACK NUMBERS: " + fb);
                            throw new Exception();
                        }
                    }
                    else if (fbArgs[argBasePos + 2] == "back")
                    {
                        if (fbArgs[argBasePos] == "bottom")
                        {
                            baseReference = referencePositions[FieldReference.BOTTOM_BACK_NUMBERS];
                        }
                        else if (fbArgs[argBasePos] == "top")
                        {
                            baseReference = referencePositions[FieldReference.TOP_BACK_NUMBERS];
                        }
                        else
                        {
                            Console.WriteLine("INVALID FRONT TO BACK NUMBERS: " + fb);
                            throw new Exception();
                        }
                    }
                    else
                    {
                        Console.WriteLine("INVALID FRONT TO BACK NUMBERS: " + fb);
                        throw new Exception();
                    }
                }
                else
                {
                    Console.WriteLine("INVALID FRONT TO BACK REFERENCE: " + fb);
                    throw new Exception();
                }

                if (isInFront)
                {
                    ret = baseReference - deviation;
                    return ret;
                }
                else
                {
                    ret = baseReference + deviation;
                    return ret;
                }
            }
            else
            {
                Console.WriteLine("INVALID FRONT TO BACK: " + fb);
                throw new Exception();
            }
        }

        public string LeftToRightToString()
        {
            float lr = lefttoright;

            string ret = "";
            if (lr <= 0)
            {
                ret += "SIDE 1: ";
                lr = -lr; //Make it positive
            }
            else
            {
                ret += "SIDE 2: ";
            }

            //float deviation = lr % 8;
            float deviation = lr % (Field.STANDARD_STEP_SIZE * 5);
            //int baseYardLineOut = 50 - ((int)((lr - deviation) / 8) * 5);
            int baseYardLineOut = 50 - (int)((lr - deviation) / Field.STANDARD_STEP_SIZE);
            //int baseYardLineIn = 50 - (((int)((lr - deviation) / 8) + 1) * 5);
            int baseYardLineIn = 50 - ((int)((lr - deviation) / Field.STANDARD_STEP_SIZE) + 5);

            if (deviation == 0)
            {
                ret += "On " + baseYardLineOut + " YARD LINE";
            }
            else if (deviation < (Field.STANDARD_STEP_SIZE * 5) / 2)
            {
                ret += deviation + " OUT " + baseYardLineOut + " YARD LINE";
            }
            else
            {
                ret += ((Field.STANDARD_STEP_SIZE * 5) - deviation) + " IN " + baseYardLineIn + " YARD LINE";
            }

            return ret;
        }
        /*
        public string FrontToBackToString()
        {
            float fb = fronttoback;

            string ret = "";
            string baseReference = "";
            string deviationDirection = "";
            float deviation;
            bool closerToAudience;

            float CENTERX_TO_HASH_DIST = Field.HASH_DIST / 2;

            if (fb <= 0)
            {
                baseReference += "FRONT ";
                fb = -fb; //Make positive
                closerToAudience = true;
            }
            else
            {
                baseReference += "BACK ";
                closerToAudience = false;
            }

            //Closer to hash or sideline?
            if (Math.Abs(fb - (Field.FIELD_WIDTH / 2)) < Math.Abs(fb - CENTERX_TO_HASH_DIST))
            {
                //The hash is farther away from here than the sideline is
                baseReference += "SIDELINE";
                deviation = Math.Abs(fb - (Field.FIELD_WIDTH / 2));

                if (deviation == 0)
                {
                    ret = "On " + baseReference;
                    return ret;
                }

                if (fb <= (Field.FIELD_WIDTH / 2))
                {
                    //If this is inside the field
                    deviationDirection = closerToAudience ? "BEHIND " : "IN FRONT ";
                }
                else
                {
                    //If this is outside the field
                    deviationDirection = closerToAudience ? "IN FRONT " : "BEHIND ";
                }
            }
            else
            {
                //The sideline is farther away (or the same distance as) the hash
                baseReference += "HASH";
                deviation = Math.Abs(fb - CENTERX_TO_HASH_DIST);

                if (deviation == 0)
                {
                    ret = "On " + baseReference;
                    return ret;
                }
                if (fb < CENTERX_TO_HASH_DIST)
                {
                    //Within the hashes
                    deviationDirection = closerToAudience ? "BEHIND " : "IN FRONT ";
                }
                else
                {
                    //Between the hash and the sideline
                    deviationDirection = closerToAudience ? "IN FRONT " : "BEHIND ";
                }
            }

            ret = deviation + " " + deviationDirection + baseReference;
            return ret;
        }*/
        /*
        public FieldReference GetNearestFrontToBackReference()
        {
            bool closerToAudience = fronttoback <= 0;
            float fb = closerToAudience ? -fronttoback : fronttoback;

            //Closer to hash or sideline?
            if (Math.Abs(fb - (Field.FIELD_WIDTH / 2)) < Math.Abs(fb - (Field.HASH_DIST / 2)))
            {
                //Sideline is closer
                return closerToAudience ? FieldReference.FRONT_SIDELINE : FieldReference.BACK_SIDELINE;
            }
            else
            {
                //Hash is closer
                return closerToAudience ? FieldReference.FRONT_HASH : FieldReference.BACK_HASH;
            }
        }*/
        /*
        public float GetDeviationFromFrontToBackReference()
        {
            bool closerToAudience = fronttoback <= 0;
            float fb = closerToAudience ? -fronttoback : fronttoback;
            //in front = negative deviation

            //Closer to hash or sideline>
            if (Math.Abs(fb - (Field.FIELD_WIDTH / 2)) < Math.Abs(fb - (Field.HASH_DIST / 2)))
            {
                //Sideline is closer
                float deviation = Math.Abs(fb - (Field.FIELD_WIDTH / 2));
                return closerToAudience ? deviation : -deviation;
            }
            else
            {
                //Hash is closer
                float deviation = Math.Abs(fb - (Field.HASH_DIST / 2));
                if (fb < (Field.HASH_DIST / 2))
                {
                    return closerToAudience ? deviation : -deviation;
                }
                else
                {
                    return closerToAudience ? -deviation : deviation;
                }
            }
        }*/
        public string FrontToBackToString()
        {
            FieldReference nearestRef = GetNearestFrontToBackReference();
            float deviation = GetDeviationFromFrontToBackReference();

            string ret = "";

            if (deviation == 0)
            {
                ret = "On ";
            }
            else if (deviation > 0)
            {
                ret = deviation + " IN FRONT ";
            }
            else
            {
                ret = -deviation + " BEHIND ";
            }

            switch (nearestRef)
            {
                case FieldReference.BACK_SIDELINE:
                    ret += "BACK SIDELINE";
                    break;
                case FieldReference.BOTTOM_BACK_NUMBERS:
                    ret += "BOTTOM OF BACK NUMBERS";
                    break;
                case FieldReference.TOP_BACK_NUMBERS:
                    ret += "TOP OF BACK NUMBERS";
                    break;
                case FieldReference.BACK_HASH:
                    ret += "BACK HASH";
                    break;
                case FieldReference.FRONT_HASH:
                    ret += "FRONT HASH";
                    break;
                case FieldReference.TOP_FRONT_NUMBERS:
                    ret += "TOP OF FRONT NUMBERS";
                    break;
                case FieldReference.BOTTOM_FRONT_NUMBERS:
                    ret += "BOTTOM OF FRONT NUMBERS";
                    break;
                case FieldReference.FRONT_SIDELINE:
                    ret += "FRONT SIDELINE";
                    break;
            }

            return ret;
        }
        public Dictionary<FieldReference, float> GetDistanceFromReferences()
        {
            Dictionary<FieldReference, float> ret = new Dictionary<FieldReference, float>(USE_NUMBERS_AS_REFERENCES ? referencePositions : referencePositionsNoNumbers);

            foreach (FieldReference key in ret.Keys.ToList())
            {
                ret[key] -= fronttoback;
            }

            return ret;
        }
        public FieldReference GetNearestFrontToBackReference()
        {
            return GetDistanceFromReferences().OrderBy(x => Math.Abs(x.Value)).First().Key;
        }
        public float GetDeviationFromFrontToBackReference()
        {
            return GetDistanceFromReferences()[GetNearestFrontToBackReference()];
        }

        public bool IsSide1()
        {
            return lefttoright <= 0;
        }

        public int GetNearestYardLine()
        {
            float lr = IsSide1() ? -lefttoright : lefttoright;

            float deviation = lr % (Field.STANDARD_STEP_SIZE * 5);
            //int baseYardLineOut = 50 - ((int)((lr - deviation) / 8) * 5);
            int baseYardLineOut = 50 - (int)((lr - deviation) / Field.STANDARD_STEP_SIZE);
            //int baseYardLineIn = 50 - (((int)((lr - deviation) / 8) + 1) * 5);
            int baseYardLineIn = 50 - ((int)((lr - deviation) / Field.STANDARD_STEP_SIZE) + 5);

            if (deviation == 0) return baseYardLineOut;
            else if (deviation < (2.5f * Field.STANDARD_STEP_SIZE)) return baseYardLineOut;
            else return baseYardLineIn;
        }

        public bool IsSame(Dot d)
        {
            return lefttoright == d.lefttoright && fronttoback == d.fronttoback;
        }

        public Dot MakeMidpointDot(Dot previousDot)
        {
            Dot ret;
            ret.lefttoright = (float)(previousDot.lefttoright + lefttoright) / 2.0f;
            ret.fronttoback = (float)(previousDot.fronttoback + fronttoback) / 2.0f;
            return ret;
        }
    }
    class Performer
    {
        public bool isHighlighted;
        public string label;
        public List<Dot> dots;
        public int firstActiveSet; //Sometimes, performers show up in certain movements. In this case, the "dots" field still has padded dots
        public int lastActiveSet; //If this was 2, then the performer would appear for set 1, then disappear

        public Performer(string name)
        {
            label = name;
            dots = new List<Dot>();
            lastActiveSet = -1;
        }

        public Dot GetInBetweenDot(int set, int count, float progress)
        {
            Dot ret = new Dot();

            float pcx = dots[set].lefttoright;
            float pcy = dots[set].fronttoback;
            float pnx = dots[set + 1].lefttoright;
            float pny = dots[set + 1].fronttoback;

            float psx = pcx + ((pnx - pcx) * ((float)(count - 1) / (float)Drill.instance.sets[set + 1].counts));
            float psy = pcy + ((pny - pcy) * ((float)(count - 1) / (float)Drill.instance.sets[set + 1].counts));
            float pex = pcx + ((pnx - pcx) * ((float)count / (float)Drill.instance.sets[set + 1].counts));
            float pey = pcy + ((pny - pcy) * ((float)count / (float)Drill.instance.sets[set + 1].counts));

            float px = psx + (pex - psx) * progress;
            float py = psy + (pey - psy) * progress;

            ret.lefttoright = px;
            ret.fronttoback = py;

            return ret;
        }
        
        public void ToggleHighlight()
        {
            isHighlighted = !isHighlighted;
        }

        public override string ToString()
        {
            return label + " (" + dots.Count + ")";
        }
    }
}
