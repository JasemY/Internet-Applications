using System;
using System.Web;
using System.Xml;
using System.IO;
using System.Text;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Punycode support class.
    /// </summary>
    /// <remarks> 
    /// Copyright © 2001-2015 Jasem Y. Al-Shamlan (info@ia.com.kw), Internet Applications - Kuwait. All Rights Reserved.
    ///
    /// This library is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
    /// the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
    ///
    /// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
    /// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
    /// 
    /// You should have received a copy of the GNU General Public License along with this library. If not, see http://www.gnu.org/licenses.
    /// 
    /// Copyright notice: This notice may not be removed or altered from any source distribution.
    /// </remarks> 
    public class Punycode
    {
        ////////////////////////////////////////////////////////////////////////////

        /*
         * $Id: punycode.cs,v 1.3 2003/03/30 23:28:41 Mayuki Sawatari Exp $
         *
         * Punycode (RFC 3492) encoder/decoder implementation in C# with .NET Framework
         *
         * RFC 3492: IDNA Punycode
         * http://www.ietf.org/rfc/rfc3492.txt
         *
         * Copyright (C) 2003 Mayuki Sawatari <mayuki@misuzilla.org>, All rights reserved.
         *
         * Redistribution and use in source and binary forms, with or without
         * modification, are permitted provided that the following conditions
         * are met:
         * 1. Redistributions of source code must retain the above copyright
         *    notice, this list of conditions and the following disclaimer.
         * 2. Redistributions in binary form must reproduce the above copyright
         *    notice, this list of conditions and the following disclaimer in the
         *    documentation and/or other materials provided with the distribution.
         *
         * THIS LIBRARY IS PROVIDED BY THE MISUZILLA.ORG ``AS IS'' AND
         * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
         * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
         * ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
         * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
         * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
         * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
         * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
         * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
         * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
         * SUCH DAMAGE.
         */

        /// <summary/>
        public const string Prefix = "xn--";

        /// <summary/>
        public static string Decode(string s)
        {
            if (s.StartsWith(Punycode.Prefix))
                return DecodeString(s.Substring(Punycode.Prefix.Length));
            else
                return DecodeString(s);
        }

        /// <summary/>
        public static string Encode(string s)
        {
            return EncodeString(s).Insert(0, Punycode.Prefix).ToString();
        }

        /// <summary/>
        public static string EncodeWithoutPrefix(string s)
        {
            return EncodeString(s).ToString();
        }

        /// <summary/>
        private sealed class PunyParams
        {
            public const Int32 Base = 36;
            public const Int32 Tmin = 1;
            public const Int32 Tmax = 26;
            public const Int32 Skew = 38;
            public const Int32 Damp = 700;
            public const Int32 InitialBias = 72;
            public const Int32 InitialN = 0x80;
            public const Int32 Delimiter = 0x2d;
        }

        /// <summary/>
        private static char DecodeDigit(Int32 cp)
        {
            return (char)(cp - 48 < 10 ? cp - 22 : cp - 65 < 26 ? cp - 65 :
                cp - 97 < 26 ? cp - 97 : PunyParams.Base);
        }

        /// <summary/>
        private static char EncodeDigit(Int32 d, Int32 flag)
        {
            return (char)(d + 22 + 75 * (d < 26 ? 1 : 0) - (flag << 5));
        }

        /// <summary/>
        private static StringBuilder EncodeString(string input)
        {
            StringBuilder output = new StringBuilder();
            Int32 delta, bias, maxOut;
            Int32 m, k, t, b, n, h, q, i;


            maxOut = Int32.MaxValue;
            n = PunyParams.InitialN;
            bias = PunyParams.InitialBias;
            delta = 0;

            for (i = 0; i < input.Length; i++)
            {
                if (input[i] < 0x80)
                {
                    if (maxOut - input.Length < 2) throw new PunycodeBigOutputException("punycode_big_output");
                    output.Append(input[i]);
                }
            }

            h = b = (Int32)output.Length;

            if (output.Length > 0)
                output.Append((char)PunyParams.Delimiter);

            /* Main encoding loop: */
            while (h < input.Length)
            {
                m = Int32.MaxValue;
                for (i = 0; i < input.Length; i++)
                {
                    if (input[i] >= n && input[i] < m)
                        m = input[i];
                }

                if (m - n > (Int32.MaxValue - delta) / (h + 1))
                    throw new PunycodeOverflowException("punycode_overflow");

                delta += (char)(m - n) * (h + 1);
                n = m;

                for (i = 0; i < input.Length; i++)
                {
                    /* Punycode does not need to check whether input[j] is basic: */
                    if (input[i] < n)
                    {
                        if (++delta == 0)
                            throw new PunycodeOverflowException("punycode_overflow");
                    }

                    if (input[i] == n)
                    {
                        /* Represent delta as a generalized variable-length integer: */
                        for (q = delta, k = PunyParams.Base; ; k += PunyParams.Base)
                        {
                            t = (k <= bias) ?
                                PunyParams.Tmin :
                                (k >= bias + PunyParams.Tmax) ?
                                    PunyParams.Tmax : k - bias;

                            if (q < t)
                                break;

                            output.Append(EncodeDigit(t + (q - t) % (PunyParams.Base - t), 0));
                            q = (q - t) / (PunyParams.Base - t);
                        }
                        //output[outlen++] = EncodeDigit(q, case_flags && case_flags[j]);
                        output.Append(EncodeDigit(q, 0)); // ignore case
                        bias = (char)Adapt(delta, h + 1, h == b);
                        delta = 0;
                        ++h;
                    }
                }
                ++delta; ++n;
            }

            return output;
        }

        private static Int32 Adapt(Int32 delta, Int32 numpoints, Boolean firsttime)
        {
            Int32 k;
            delta = firsttime ? delta / PunyParams.Damp : delta >> 1; /* delta >> 1 --> delta / 2 */
            delta += delta / numpoints;

            for (k = 0; delta > ((PunyParams.Base - PunyParams.Tmin) * PunyParams.Tmax) / 2; k += PunyParams.Base)
            {
                delta /= (PunyParams.Base - PunyParams.Tmin);
            }

            return k + ((PunyParams.Base - PunyParams.Tmin) + 1) * delta / (delta + PunyParams.Skew);
        }

        private static string DecodeString(string input)
        {
            StringBuilder output = new StringBuilder();
            Int32 n, outlen, i, bias, b, j, inl, oldi, w, k, digit, t;

            /* Initialize the state: */

            n = PunyParams.InitialN;
            outlen = i = 0;
            bias = PunyParams.InitialBias;

            /* Handle the basic code points:  Let b be the number of input code */
            /* points before the last delimiter, or 0 if there is none, then    */
            /* copy the first b code points to the output.                      */

            for (b = j = 0; j < input.Length; ++j)
                if (input[j] == PunyParams.Delimiter)
                    b = j;

            if (b > Int32.MaxValue)
                throw new PunycodeBigOutputException("punycode_big_output");

            for (j = 0; j < b; ++j)
            {
                //if (case_flags)
                //	case_flags[outlen] = flagged(input[j]);
                if (!(input[j] < 0x80))
                    throw new PunycodeBadInputException("punycode_bad_input");
                outlen++;
                output.Append(input[j]);
            }

            /* Main decoding loop:  Start just after the last delimiter if any  */
            /* basic code points were copied; start at the beginning otherwise. */
            for (inl = b > 0 ? b + 1 : 0; inl < input.Length; ++outlen)
            {

                for (oldi = i, w = 1, k = PunyParams.Base; ; k += PunyParams.Base)
                {
                    if (inl >= input.Length)
                        throw new PunycodeBadInputException(string.Format("{0} >= {1}", inl, input.Length));

                    digit = DecodeDigit(input[inl++]);

                    if (digit >= PunyParams.Base)
                        throw new PunycodeBadInputException(string.Format("{0} >= {1}", digit, PunyParams.Base));
                    if (digit > (Int32.MaxValue - i) / w)
                        throw new PunycodeOverflowException(string.Format("{0} > ({1} - {2}) / {3}", digit, Int32.MaxValue, i, w));

                    i += digit * w;
                    t = (k <= bias ? PunyParams.Tmin : (k >= bias + PunyParams.Tmax ? PunyParams.Tmax : k - bias));
                    if (digit < t) break;

                    if (w > Int32.MaxValue / (PunyParams.Base - t))
                        throw new PunycodeOverflowException("punycode_overflow");

                    w *= (PunyParams.Base - t);
                }

                bias = Adapt(i - oldi, outlen + 1, oldi == 0);

                /* i was supposed to wrap around from out+1 to 0,   */
                /* incrementing n each time, so we'll fix that now: */

                if (i / (outlen + 1) > Int32.MaxValue - n)
                    throw new PunycodeOverflowException("punycode_overflow");

                n += i / (outlen + 1);
                i %= (outlen + 1);

                /* Insert n at position i of the output: */
                if (outlen >= Int32.MaxValue)
                    throw new PunycodeBigOutputException("punycode_big_output");
                //if (case_flags) {
                //memmove(case_flags + i + 1, case_flags + i, out - i);

                /* Case of last character determines uppercase flag: */
                //case_flags[i] = flagged(input[inl - 1]);
                //}
                output.Insert(i, (char)n);
                i++;
            }
            return output.ToString();
        }
    }

    /// <summary/>
    public class PunycodeBigOutputException : ApplicationException
    {
        /// <summary/>
        public PunycodeBigOutputException(string s) : base(s) { }
    }

    /// <summary/>
    public class PunycodeBadInputException : ApplicationException
    {
        /// <summary/>
        public PunycodeBadInputException(string s) : base(s) { }
    }

    /// <summary/>
    public class PunycodeOverflowException : ApplicationException
    {
        /// <summary/>
        public PunycodeOverflowException(string s) : base(s) { }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}

