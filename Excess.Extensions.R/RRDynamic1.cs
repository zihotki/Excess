using System;
using System.Collections.Generic;

namespace Excess.Extensions.R
{
    public static partial class RR
    {
        public static object add(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return add((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return add((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return add((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return add((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return add((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return add((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return add((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return add((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return add((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return add((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return add((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return add((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return add((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return add((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return add((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return add((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return add((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return add((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return add((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return add((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return add((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return add((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return add((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return add((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return add((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return add((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return add((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return add((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return add((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return add((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return add((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return add((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return add((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return add((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return add((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return add((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return add((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return add((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return add((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return add((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return add((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return add((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return add((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return add((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return add((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return add((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return add((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return add((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return add((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return add((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return add((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return add((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return add((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return add((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return add((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return add((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return add((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return add((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return add((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return add((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return add((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return add((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return add((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return add((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator add does not support this type combination");
        }

        public static object sub(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return sub((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return sub((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return sub((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return sub((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return sub((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return sub((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return sub((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return sub((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return sub((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return sub((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return sub((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return sub((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return sub((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return sub((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return sub((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return sub((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return sub((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return sub((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return sub((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return sub((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return sub((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return sub((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return sub((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return sub((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return sub((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return sub((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return sub((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return sub((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return sub((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return sub((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return sub((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return sub((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return sub((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return sub((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return sub((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return sub((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return sub((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return sub((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return sub((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return sub((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return sub((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return sub((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return sub((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return sub((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return sub((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return sub((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return sub((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return sub((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return sub((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return sub((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return sub((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return sub((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return sub((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return sub((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return sub((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return sub((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return sub((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return sub((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return sub((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return sub((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return sub((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return sub((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return sub((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return sub((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator sub does not support this type combination");
        }

        public static object mul(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return mul((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return mul((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return mul((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return mul((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return mul((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return mul((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return mul((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return mul((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return mul((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return mul((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return mul((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return mul((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return mul((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return mul((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return mul((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return mul((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return mul((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return mul((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return mul((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return mul((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return mul((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return mul((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return mul((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return mul((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return mul((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return mul((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return mul((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return mul((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return mul((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return mul((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return mul((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return mul((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return mul((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return mul((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return mul((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return mul((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return mul((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return mul((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return mul((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return mul((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return mul((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return mul((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return mul((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return mul((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return mul((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return mul((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return mul((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return mul((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return mul((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return mul((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return mul((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return mul((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return mul((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return mul((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return mul((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return mul((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return mul((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return mul((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return mul((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return mul((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return mul((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return mul((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return mul((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return mul((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator mul does not support this type combination");
        }

        public static object div(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return div((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return div((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return div((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return div((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return div((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return div((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return div((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return div((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return div((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return div((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return div((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return div((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return div((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return div((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return div((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return div((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return div((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return div((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return div((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return div((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return div((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return div((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return div((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return div((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return div((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return div((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return div((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return div((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return div((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return div((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return div((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return div((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return div((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return div((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return div((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return div((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return div((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return div((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return div((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return div((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return div((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return div((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return div((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return div((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return div((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return div((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return div((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return div((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return div((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return div((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return div((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return div((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return div((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return div((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return div((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return div((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return div((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return div((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return div((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return div((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return div((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return div((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return div((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return div((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator div does not support this type combination");
        }

        public static object gt(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return gt((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return gt((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return gt((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return gt((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return gt((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return gt((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return gt((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return gt((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return gt((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return gt((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return gt((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return gt((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return gt((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return gt((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return gt((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return gt((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return gt((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return gt((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return gt((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return gt((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return gt((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return gt((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return gt((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return gt((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return gt((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return gt((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return gt((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return gt((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return gt((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return gt((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return gt((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return gt((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return gt((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return gt((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return gt((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return gt((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return gt((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return gt((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return gt((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return gt((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return gt((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return gt((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return gt((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return gt((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return gt((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return gt((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return gt((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return gt((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return gt((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return gt((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return gt((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return gt((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return gt((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return gt((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return gt((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return gt((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return gt((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return gt((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return gt((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return gt((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return gt((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return gt((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return gt((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return gt((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator gt does not support this type combination");
        }

        public static object ge(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return ge((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return ge((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return ge((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return ge((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return ge((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return ge((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return ge((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return ge((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return ge((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return ge((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return ge((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return ge((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return ge((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return ge((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return ge((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return ge((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return ge((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return ge((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return ge((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return ge((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return ge((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return ge((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return ge((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return ge((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return ge((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return ge((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return ge((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return ge((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return ge((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return ge((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return ge((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return ge((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return ge((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return ge((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return ge((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return ge((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return ge((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return ge((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return ge((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return ge((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return ge((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return ge((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return ge((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return ge((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return ge((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return ge((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return ge((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return ge((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return ge((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return ge((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return ge((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return ge((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return ge((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return ge((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return ge((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return ge((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return ge((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return ge((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return ge((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return ge((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return ge((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return ge((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return ge((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return ge((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator ge does not support this type combination");
        }

        public static object lt(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return lt((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return lt((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return lt((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return lt((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return lt((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return lt((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return lt((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return lt((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return lt((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return lt((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return lt((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return lt((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return lt((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return lt((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return lt((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return lt((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return lt((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return lt((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return lt((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return lt((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return lt((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return lt((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return lt((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return lt((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return lt((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return lt((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return lt((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return lt((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return lt((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return lt((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return lt((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return lt((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return lt((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return lt((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return lt((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return lt((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return lt((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return lt((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return lt((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return lt((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return lt((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return lt((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return lt((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return lt((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return lt((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return lt((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return lt((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return lt((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return lt((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return lt((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return lt((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return lt((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return lt((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return lt((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return lt((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return lt((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return lt((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return lt((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return lt((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return lt((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return lt((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return lt((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return lt((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return lt((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator lt does not support this type combination");
        }

        public static object le(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return le((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return le((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return le((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return le((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return le((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return le((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return le((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return le((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return le((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return le((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return le((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return le((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return le((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return le((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return le((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return le((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return le((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return le((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return le((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return le((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return le((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return le((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return le((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return le((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return le((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return le((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return le((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return le((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return le((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return le((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return le((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return le((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return le((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return le((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return le((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return le((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return le((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return le((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return le((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return le((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return le((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return le((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return le((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return le((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return le((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return le((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return le((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return le((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return le((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return le((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return le((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return le((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return le((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return le((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return le((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return le((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return le((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return le((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return le((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return le((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return le((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return le((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return le((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return le((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator le does not support this type combination");
        }

        public static object eq(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return eq((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return eq((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return eq((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return eq((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return eq((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return eq((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return eq((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return eq((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return eq((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return eq((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return eq((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return eq((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return eq((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return eq((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return eq((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return eq((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return eq((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return eq((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return eq((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return eq((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return eq((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return eq((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return eq((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return eq((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return eq((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return eq((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return eq((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return eq((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return eq((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return eq((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return eq((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return eq((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return eq((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return eq((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return eq((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return eq((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return eq((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return eq((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return eq((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return eq((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return eq((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return eq((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return eq((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return eq((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return eq((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return eq((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return eq((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return eq((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return eq((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return eq((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return eq((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return eq((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return eq((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return eq((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return eq((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return eq((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return eq((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return eq((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return eq((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return eq((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return eq((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return eq((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return eq((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return eq((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator eq does not support this type combination");
        }

        public static object neq(object val1, object val2)
        {
            if (val1 is double)
            {
                if (val2 is double)
                    return neq((double) val1, (double) val2);

                if (val2 is Vector<double>)
                    return neq((double) val1, (Vector<double>) val2);

                if (val2 is float)
                    return neq((double) val1, (float) val2);

                if (val2 is Vector<float>)
                    return neq((double) val1, (Vector<float>) val2);

                if (val2 is long)
                    return neq((double) val1, (long) val2);

                if (val2 is Vector<long>)
                    return neq((double) val1, (Vector<long>) val2);

                if (val2 is int)
                    return neq((double) val1, (int) val2);

                if (val2 is Vector<int>)
                    return neq((double) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<double>)
            {
                if (val2 is double)
                    return neq((Vector<double>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return neq((Vector<double>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return neq((Vector<double>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return neq((Vector<double>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return neq((Vector<double>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return neq((Vector<double>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return neq((Vector<double>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return neq((Vector<double>) val1, (Vector<int>) val2);
            }


            if (val1 is float)
            {
                if (val2 is double)
                    return neq((float) val1, (double) val2);

                if (val2 is Vector<double>)
                    return neq((float) val1, (Vector<double>) val2);

                if (val2 is float)
                    return neq((float) val1, (float) val2);

                if (val2 is Vector<float>)
                    return neq((float) val1, (Vector<float>) val2);

                if (val2 is long)
                    return neq((float) val1, (long) val2);

                if (val2 is Vector<long>)
                    return neq((float) val1, (Vector<long>) val2);

                if (val2 is int)
                    return neq((float) val1, (int) val2);

                if (val2 is Vector<int>)
                    return neq((float) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<float>)
            {
                if (val2 is double)
                    return neq((Vector<float>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return neq((Vector<float>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return neq((Vector<float>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return neq((Vector<float>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return neq((Vector<float>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return neq((Vector<float>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return neq((Vector<float>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return neq((Vector<float>) val1, (Vector<int>) val2);
            }


            if (val1 is long)
            {
                if (val2 is double)
                    return neq((long) val1, (double) val2);

                if (val2 is Vector<double>)
                    return neq((long) val1, (Vector<double>) val2);

                if (val2 is float)
                    return neq((long) val1, (float) val2);

                if (val2 is Vector<float>)
                    return neq((long) val1, (Vector<float>) val2);

                if (val2 is long)
                    return neq((long) val1, (long) val2);

                if (val2 is Vector<long>)
                    return neq((long) val1, (Vector<long>) val2);

                if (val2 is int)
                    return neq((long) val1, (int) val2);

                if (val2 is Vector<int>)
                    return neq((long) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<long>)
            {
                if (val2 is double)
                    return neq((Vector<long>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return neq((Vector<long>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return neq((Vector<long>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return neq((Vector<long>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return neq((Vector<long>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return neq((Vector<long>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return neq((Vector<long>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return neq((Vector<long>) val1, (Vector<int>) val2);
            }


            if (val1 is int)
            {
                if (val2 is double)
                    return neq((int) val1, (double) val2);

                if (val2 is Vector<double>)
                    return neq((int) val1, (Vector<double>) val2);

                if (val2 is float)
                    return neq((int) val1, (float) val2);

                if (val2 is Vector<float>)
                    return neq((int) val1, (Vector<float>) val2);

                if (val2 is long)
                    return neq((int) val1, (long) val2);

                if (val2 is Vector<long>)
                    return neq((int) val1, (Vector<long>) val2);

                if (val2 is int)
                    return neq((int) val1, (int) val2);

                if (val2 is Vector<int>)
                    return neq((int) val1, (Vector<int>) val2);
            }

            if (val1 is Vector<int>)
            {
                if (val2 is double)
                    return neq((Vector<int>) val1, (double) val2);

                if (val2 is Vector<double>)
                    return neq((Vector<int>) val1, (Vector<double>) val2);

                if (val2 is float)
                    return neq((Vector<int>) val1, (float) val2);

                if (val2 is Vector<float>)
                    return neq((Vector<int>) val1, (Vector<float>) val2);

                if (val2 is long)
                    return neq((Vector<int>) val1, (long) val2);

                if (val2 is Vector<long>)
                    return neq((Vector<int>) val1, (Vector<long>) val2);

                if (val2 is int)
                    return neq((Vector<int>) val1, (int) val2);

                if (val2 is Vector<int>)
                    return neq((Vector<int>) val1, (Vector<int>) val2);
            }

            throw new InvalidOperationException("operator neq does not support this type combination");
        }


        public static object and(object val1, object val2)
        {
            if (val1 is bool)
            {
                if (val2 is bool)
                    return and((bool) val1, (bool) val2);

                if (val2 is Vector<bool>)
                    return and((bool) val1, (Vector<bool>) val2);
            }

            if (val1 is Vector<bool>)
            {
                if (val2 is bool)
                    return and((Vector<bool>) val1, (bool) val2);

                if (val2 is Vector<bool>)
                    return and((Vector<bool>) val1, (Vector<bool>) val2);
            }
            throw new InvalidOperationException("operator and does not support this type combination");
        }

        public static object bnd(object val1, object val2)
        {
            if (val1 is bool)
            {
                if (val2 is bool)
                    return bnd((bool) val1, (bool) val2);

                if (val2 is Vector<bool>)
                    return bnd((bool) val1, (Vector<bool>) val2);
            }

            if (val1 is Vector<bool>)
            {
                if (val2 is bool)
                    return bnd((Vector<bool>) val1, (bool) val2);

                if (val2 is Vector<bool>)
                    return bnd((Vector<bool>) val1, (Vector<bool>) val2);
            }
            throw new InvalidOperationException("operator bnd does not support this type combination");
        }

        public static object or(object val1, object val2)
        {
            if (val1 is bool)
            {
                if (val2 is bool)
                    return or((bool) val1, (bool) val2);

                if (val2 is Vector<bool>)
                    return or((bool) val1, (Vector<bool>) val2);
            }

            if (val1 is Vector<bool>)
            {
                if (val2 is bool)
                    return or((Vector<bool>) val1, (bool) val2);

                if (val2 is Vector<bool>)
                    return or((Vector<bool>) val1, (Vector<bool>) val2);
            }
            throw new InvalidOperationException("operator or does not support this type combination");
        }

        public static object bor(object val1, object val2)
        {
            if (val1 is bool)
            {
                if (val2 is bool)
                    return bor((bool) val1, (bool) val2);

                if (val2 is Vector<bool>)
                    return bor((bool) val1, (Vector<bool>) val2);
            }

            if (val1 is Vector<bool>)
            {
                if (val2 is bool)
                    return bor((Vector<bool>) val1, (bool) val2);

                if (val2 is Vector<bool>)
                    return bor((Vector<bool>) val1, (Vector<bool>) val2);
            }
            throw new InvalidOperationException("operator bor does not support this type combination");
        }

        private static bool higher(Type type1, Type type2)
        {
            if (type1 == type2)
                return false;


            if (type1 == typeof(double))
            {
                if (type2 == typeof(float))
                    return true;

                if (type2 == typeof(long))
                    return true;

                if (type2 == typeof(int))
                    return true;
            }

            if (type1 == typeof(float))
            {
                if (type2 == typeof(long))
                    return true;

                if (type2 == typeof(int))
                    return true;
            }

            if (type1 == typeof(long))
            {
                if (type2 == typeof(int))
                    return true;
            }

            return false;
        }

        //dynamic operations
        public static object sum(object val)
        {
            if (val is Vector<double>)
                return sum(val as Vector<double>);

            if (val is Vector<float>)
                return sum(val as Vector<float>);

            if (val is Vector<long>)
                return sum(val as Vector<long>);

            if (val is Vector<int>)
                return sum(val as Vector<int>);

            throw new InvalidOperationException("sum expects numeric vectors");
        }

        public static object index(object val, object vec)
        {
            if (vec is Vector<bool>)
            {
                if (val is Vector<double>)
                    return index((Vector<double>) val, (Vector<bool>) vec);

                if (val is Vector<float>)
                    return index((Vector<float>) val, (Vector<bool>) vec);

                if (val is Vector<long>)
                    return index((Vector<long>) val, (Vector<bool>) vec);

                if (val is Vector<int>)
                    return index((Vector<int>) val, (Vector<bool>) vec);

                if (val is Vector<bool>)
                    return index((Vector<bool>) val, (Vector<bool>) vec);

                if (val is Vector<string>)
                    return index((Vector<string>) val, (Vector<bool>) vec);
            }
            else if (vec is Vector<int>)
            {
                if (val is Vector<double>)
                    return index((Vector<double>) val, (Vector<int>) vec);

                if (val is Vector<float>)
                    return index((Vector<float>) val, (Vector<int>) vec);

                if (val is Vector<long>)
                    return index((Vector<long>) val, (Vector<int>) vec);

                if (val is Vector<int>)
                    return index((Vector<int>) val, (Vector<int>) vec);

                if (val is Vector<bool>)
                    return index((Vector<bool>) val, (Vector<int>) vec);

                if (val is Vector<string>)
                    return index((Vector<string>) val, (Vector<int>) vec);
            }

            throw new InvalidOperationException("index vectors expects only boolean or int");
        }

        private static object concat(IEnumerable<IVector> values, int len, Type type)
        {
            if (type == typeof(double) || type == typeof(Vector<double>))
                return Vector<double>.create(len, EnumerateVectors<double>(values));

            if (type == typeof(float) || type == typeof(Vector<float>))
                return Vector<float>.create(len, EnumerateVectors<float>(values));

            if (type == typeof(long) || type == typeof(Vector<long>))
                return Vector<long>.create(len, EnumerateVectors<long>(values));

            if (type == typeof(int) || type == typeof(Vector<int>))
                return Vector<int>.create(len, EnumerateVectors<int>(values));

            if (type == typeof(bool) || type == typeof(Vector<bool>))
                return Vector<bool>.create(len, EnumerateVectors<bool>(values));

            if (type == typeof(string) || type == typeof(Vector<string>))
                return Vector<string>.create(len, EnumerateVectors<string>(values));

            throw new InvalidOperationException("cannont concatenate type: " + type.FullName);
        }
    }
}