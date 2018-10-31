using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UHub.CoreLib.Extensions
{
    public static class IEnumerableExtensions
    {
        public static double Median(this IEnumerable<byte> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<short> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<int> set)
        {
            var list = set.ToList();

            if(list.Count == 0)
            {
                return 0;
            }
            if(list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<long> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<float> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }


        public static double Median(this IEnumerable<double> set)
        {
            var list = set.ToList();

            if (list.Count == 0)
            {
                return 0;
            }
            if (list.Count == 1)
            {
                return list[1];
            }

            double middle = (list.Count / 2.0);
            int floorMiddle = (int)middle;
            double diff = middle - floorMiddle;

            //even itm count
            if (diff < .01)
            {
                return (list[floorMiddle] + list[floorMiddle + 1]) / 2.0;
            }
            //odd itm count
            else
            {
                return list[floorMiddle];
            }
        }

    }
}
