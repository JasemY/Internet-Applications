//--------------------- Copyright Block ----------------------
/* 

PrayTime.cs: Prayer Times Calculator (ver 1.2)
Copyright (C) 2007-2010 PrayTimes.org

C# Code By: Jandost Khoso
Original JS Code By: Hamid Zarrabi-Zadeh

License: GNU General Public License, ver 3

TERMS OF USE:
	Permission is granted to use this code, with or 
	without modification, in any website or application 
	provided that credit is given to the original work 
	with a link back to PrayTimes.org.

This program is distributed in the hope that it will 
be useful, but WITHOUT ANY WARRANTY. 

PLEASE DO NOT REMOVE THIS COPYRIGHT BLOCK.

*/

using System;

namespace Ia.Cl.Model
{
    /// <summary publish="true">
    /// Prayer times support class.
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
    public class PrayerTime
    {
        //------------------------ Constants --------------------------

        // Calculation Methods
        /// <summary/>
        public static int Jafari = 0;    // Ithna Ashari

        /// <summary/>
        public static int Karachi = 1;    // University of Islamic Sciences, Karachi

        /// <summary/>
        public static int ISNA = 2;    // Islamic Society of North America (ISNA)

        /// <summary/>
        public static int MWL = 3;    // Muslim World League (MWL)

        /// <summary/>
        public static int Makkah = 4;    // Umm al-Qura, Makkah

        /// <summary/>
        public static int Egypt = 5;    // Egyptian General Authority of Survey

        /// <summary/>
        public static int Custom = 6;    // Custom Setting

        /// <summary/>
        public static int Tehran = 7;    // Institute of Geophysics, University of Tehran

        // Juristic Methods
        /// <summary/>
        public static int Shafii = 0;    // Shafii (standard)

        /// <summary/>
        public static int Hanafi = 1;    // Hanafi

        // Adjusting Methods for Higher Latitudes
        /// <summary/>
        public static int None = 0;    // No adjustment

        /// <summary/>
        public static int MidNight = 1;    // middle of night
        /// <summary/>

        public static int OneSeventh = 2;    // 1/7th of night
        /// <summary/>
        public static int AngleBased = 3;    // angle/60th of night


        // Time Formats
        /// <summary/>
        public static int Time24 = 0;    // 24-hour format

        /// <summary/>
        public static int Time12 = 1;    // 12-hour format

        /// <summary/>
        public static int Time12NS = 2;    // 12-hour format with no suffix

        /// <summary/>
        public static int Floating = 3;    // floating point number

        // Time Names
        /// <summary/>
        public static string[] timeNames = { "Fajr", "Sunrise", "Dhuhr", "Asr", "Sunset", "Maghrib", "Isha" };
        static string InvalidTime = "----";	 // The string used for inv




        //---------------------- Global Variables --------------------


        private int calcMethod = 3;		// caculation method
        private int asrJuristic;		// Juristic method for Asr
        private int dhuhrMinutes = 0;		// minutes after mid-day for Dhuhr
        private int adjustHighLats = 1;	// adjusting method for higher latitudes

        private int timeFormat = 0;		// time format

        private double lat;        // latitude
        private double lng;        // longitude
        private int timeZone;   // time-zone
        private double JDate;      // Julian date

        private int[] times;


        //--------------------- Technical Settings --------------------


        private int numIterations = 1;		// number of iterations needed to compute times



        //------------------- Calc Method Parameters --------------------

        private double[][] methodParams;

        /// <summary/>
        public PrayerTime()
        {
            times = new int[7];
            methodParams = new double[8][];
            this.methodParams[Jafari] = new double[] { 16, 0, 4, 0, 14 };
            this.methodParams[Karachi] = new double[] { 18, 1, 0, 0, 18 };
            this.methodParams[ISNA] = new double[] { 15, 1, 0, 0, 15 };
            this.methodParams[MWL] = new double[] { 18, 1, 0, 0, 17 };
            this.methodParams[Makkah] = new double[] { 18.5, 1, 0, 1, 90 };
            this.methodParams[Egypt] = new double[] { 19.5, 1, 0, 0, 17.5 };
            this.methodParams[Tehran] = new double[] { 17.7, 0, 4.5, 0, 14 };
            this.methodParams[Custom] = new double[] { 18, 1, 0, 0, 17 };
        }

        // return prayer times for a given date
        /// <summary/>
        public string[] GetPrayerTimes(int year, int month, int day, double latitude, double longitude, int timeZone)
        {
            return this.GetDatePrayerTimes(year, month + 1, day, latitude, longitude, timeZone);
        }

        // set the calculation method
        /// <summary/>
        public void SetCalcMethod(int methodID)
        {
            this.calcMethod = methodID;
        }

        // set the juristic method for Asr
        /// <summary/>
        public void SetAsrMethod(int methodID)
        {
            if (methodID < 0 || methodID > 1) return;
            this.asrJuristic = methodID;
        }

        // set the angle for calculating Fajr
        /// <summary/>
        public void SetFajrAngle(double angle)
        {
            this.SetCustomParams(new int[] { (int)angle, -1, -1, -1, -1 });
        }

        // set the angle for calculating Maghrib
        /// <summary/>
        public void SetMaghribAngle(double angle)
        {
            this.SetCustomParams(new int[] { -1, 0, (int)angle, -1, -1 });
        }

        // set the angle for calculating Isha
        /// <summary/>
        public void SetIshaAngle(double angle)
        {
            this.SetCustomParams(new int[] { -1, -1, -1, 0, (int)angle });
        }

        // set the minutes after mid-day for calculating Dhuhr
        /// <summary/>
        public void SetDhuhrMinutes(int minutes)
        {
            this.dhuhrMinutes = minutes;
        }

        // set the minutes after Sunset for calculating Maghrib
        /// <summary/>
        public void SetMaghribMinutes(int minutes)
        {
            this.SetCustomParams(new int[] { -1, 1, minutes, -1, -1 });
        }

        // set the minutes after Maghrib for calculating Isha
        /// <summary/>
        public void SetIshaMinutes(int minutes)
        {
            this.SetCustomParams(new int[] { -1, -1, -1, 1, minutes });
        }

        // set custom values for calculation parameters
        /// <summary/>
        public void SetCustomParams(int[] param)
        {
            for (int i = 0; i < 5; i++)
            {
                if (param[i] == -1) this.methodParams[Custom][i] = this.methodParams[this.calcMethod][i];
                else this.methodParams[Custom][i] = param[i];
            }

            this.calcMethod = Custom;
        }

        // set adjusting method for higher latitudes
        /// <summary/>
        public void SetHighLatsMethod(int methodID)
        {
            this.adjustHighLats = methodID;
        }

        // set the time format
        /// <summary/>
        public void SetTimeFormat(int timeFormat)
        {
            this.timeFormat = timeFormat;
        }

        // convert float hours to 24h format
        /// <summary/>
        public string FloatToTime24(double time)
        {
            if (time < 0) return InvalidTime;
            time = this.FixHour(time + 0.5 / 60);  // add 0.5 minutes to round
            double hours = Math.Floor(time);
            double minutes = Math.Floor((time - hours) * 60);
            return this.TwoDigitsFormat((int)hours) + ":" + this.TwoDigitsFormat((int)minutes);
        }

        // convert float hours to 12h format
        /// <summary/>
        public string FloatToTime12(double time, bool noSuffix)
        {
            if (time < 0) return InvalidTime;
            time = this.FixHour(time + 0.5 / 60);  // add 0.5 minutes to round
            double hours = Math.Floor(time);
            double minutes = Math.Floor((time - hours) * 60);
            string suffix = hours >= 12 ? " pm" : " am";
            hours = (hours + 12 - 1) % 12 + 1;
            return ((int)hours) + ":" + this.TwoDigitsFormat((int)minutes) + (noSuffix ? "" : suffix);
        }

        // convert float hours to 12h format with no suffix
        /// <summary/>
        public string FloatToTime12NS(double time)
        {
            return this.FloatToTime12(time, true);
        }

        //---------------------- Compute Prayer Times -----------------------


        // return prayer times for a given date
        /// <summary/>
        public string[] GetDatePrayerTimes(int year, int month, int day, double latitude, double longitude, int timeZone)
        {
            this.lat = latitude;
            this.lng = longitude;
            this.timeZone = timeZone;
            this.JDate = this.JulianDate(year, month, day) - longitude / (15 * 24);

            return this.ComputeDayTimes();
        }

        // compute declination angle of sun and equation of time
        /// <summary/>
        public double[] SunPosition(double jd)
        {
            double D = jd - 2451545.0;
            double g = this.FixAngle(357.529 + 0.98560028 * D);
            double q = this.FixAngle(280.459 + 0.98564736 * D);
            double L = this.FixAngle(q + 1.915 * this.Dsin(g) + 0.020 * this.Dsin(2 * g));

            double R = 1.00014 - 0.01671 * this.Dcos(g) - 0.00014 * this.Dcos(2 * g);
            double e = 23.439 - 0.00000036 * D;

            double d = this.Darcsin(this.Dsin(e) * this.Dsin(L));
            double RA = this.Darctan2(this.Dcos(e) * this.Dsin(L), this.Dcos(L)) / 15;
            RA = this.FixHour(RA);
            double EqT = q / 15 - RA;

            return new double[] { d, EqT };
        }

        // compute equation of time
        /// <summary/>
        public double EquationOfTime(double jd)
        {
            return this.SunPosition(jd)[1];
        }

        // compute declination angle of sun
        /// <summary/>
        public double SunDeclination(double jd)
        {
            return this.SunPosition(jd)[0];
        }

        // compute mid-day (Dhuhr, Zawal) time
        /// <summary/>
        public double ComputeMidDay(double t)
        {
            double T = this.EquationOfTime(this.JDate + t);
            double Z = this.FixHour(12 - T);
            return Z;
        }

        // compute time for a given angle G
        /// <summary/>
        public double ComputeTime(double G, double t)
        {
            //System.out.println("G: "+G);

            double D = this.SunDeclination(this.JDate + t);
            double Z = this.ComputeMidDay(t);
            double V = ((double)1 / 15) * this.Darccos((-this.Dsin(G) - this.Dsin(D) * this.Dsin(this.lat)) /
                    (this.Dcos(D) * this.Dcos(this.lat)));
            return Z + (G > 90 ? -V : V);
        }

        // compute the time of Asr
        /// <summary/>
        public double ComputeAsr(int step, double t)  // Shafii: step=1, Hanafi: step=2
        {
            double D = this.SunDeclination(this.JDate + t);
            double G = -this.Darccot(step + this.Dtan(Math.Abs(this.lat - D)));
            return this.ComputeTime(G, t);
        }

        //---------------------- Compute Prayer Times -----------------------

        // compute prayer times at given julian date
        /// <summary/>
        public double[] ComputeTimes(double[] times)
        {
            double[] t = this.DayPortion(times);


            double Fajr = this.ComputeTime(180 - this.methodParams[this.calcMethod][0], t[0]);
            double Sunrise = this.ComputeTime(180 - 0.833, t[1]);
            double Dhuhr = this.ComputeMidDay(t[2]);
            double Asr = this.ComputeAsr(1 + this.asrJuristic, t[3]);
            double Sunset = this.ComputeTime(0.833, t[4]); ;
            double Maghrib = this.ComputeTime(this.methodParams[this.calcMethod][2], t[5]);
            double Isha = this.ComputeTime(this.methodParams[this.calcMethod][4], t[6]);

            return new double[] { Fajr, Sunrise, Dhuhr, Asr, Sunset, Maghrib, Isha };
        }

        // adjust Fajr, Isha and Maghrib for locations in higher latitudes
        /// <summary/>
        public double[] AdjustHighLatTimes(double[] times)
        {
            double nightTime = this.GetTimeDifference(times[4], times[1]); // sunset to sunrise

            // Adjust Fajr
            double FajrDiff = this.NightPortion(this.methodParams[this.calcMethod][0]) * nightTime;
            if (this.GetTimeDifference(times[0], times[1]) > FajrDiff)
                times[0] = times[1] - FajrDiff;

            // Adjust Isha
            double IshaAngle = (this.methodParams[this.calcMethod][3] == 0) ? this.methodParams

        [this.calcMethod][4] : 18;
            double IshaDiff = this.NightPortion(IshaAngle) * nightTime;
            if (this.GetTimeDifference(times[4], times[6]) > IshaDiff)
                times[6] = times[4] + IshaDiff;

            // Adjust Maghrib
            double MaghribAngle = (methodParams[this.calcMethod][1] == 0) ? this.methodParams

        [this.calcMethod][2] : 4;
            double MaghribDiff = this.NightPortion(MaghribAngle) * nightTime;
            if (this.GetTimeDifference(times[4], times[5]) > MaghribDiff)
                times[5] = times[4] + MaghribDiff;

            return times;
        }

        // the night portion used for adjusting times in higher latitudes
        /// <summary/>
        public double NightPortion(double angle)
        {
            double val = 0;
            if (this.adjustHighLats == AngleBased)
                val = 1.0 / 60.0 * angle;
            if (this.adjustHighLats == MidNight)
                val = 1.0 / 2.0;
            if (this.adjustHighLats == OneSeventh)
                val = 1.0 / 7.0;

            return val;
        }

        /// <summary/>
        public double[] DayPortion(double[] times)
        {
            for (int i = 0; i < times.Length; i++)
            {
                times[i] /= 24;
            }
            return times;
        }

        // compute prayer times at given julian date
        /// <summary/>
        public string[] ComputeDayTimes()
        {
            double[] times = { 5, 6, 12, 13, 18, 18, 18 }; //default times

            for (int i = 0; i < this.numIterations; i++)
            {
                times = this.ComputeTimes(times);
            }

            times = this.AdjustTimes(times);
            return this.AdjustTimesFormat(times);
        }


        // adjust times in a prayer time array
        /// <summary/>
        public double[] AdjustTimes(double[] times)
        {
            for (int i = 0; i < 7; i++)
            {
                times[i] += this.timeZone - this.lng / 15;
            }
            times[2] += this.dhuhrMinutes / 60; //Dhuhr
            if (this.methodParams[this.calcMethod][1] == 1) // Maghrib
                times[5] = times[4] + this.methodParams[this.calcMethod][2] / 60.0;
            if (this.methodParams[this.calcMethod][3] == 1) // Isha
                times[6] = times[5] + this.methodParams[this.calcMethod][4] / 60.0;

            if (this.adjustHighLats != None)
            {
                times = this.AdjustHighLatTimes(times);
            }

            return times;
        }

        /// <summary/>
        public string[] AdjustTimesFormat(double[] times)
        {
            string[] formatted = new string[times.Length];

            if (this.timeFormat == Floating)
            {
                for (int i = 0; i < times.Length; ++i)
                {
                    formatted[i] = times[i] + "";
                }
                return formatted;
            }

            for (int i = 0; i < 7; i++)
            {
                if (this.timeFormat == Time12)
                    formatted[i] = this.FloatToTime12(times[i], true);
                else if (this.timeFormat == Time12NS)
                    formatted[i] = this.FloatToTime12NS(times[i]);
                else
                    formatted[i] = this.FloatToTime24(times[i]);
            }
            return formatted;
        }

        //---------------------- Misc Functions -----------------------

        // compute the difference between two times
        /// <summary/>
        public double GetTimeDifference(double c1, double c2)
        {
            double diff = this.FixHour(c2 - c1); ;
            return diff;
        }

        // add a leading 0 if necessary
        /// <summary/>
        public string TwoDigitsFormat(int num)
        {

            return (num < 10) ? "0" + num : num + "";
        }

        //---------------------- Julian Date Functions -----------------------

        // calculate julian date from a calendar date
        /// <summary/>
        public double JulianDate(int year, int month, int day)
        {
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }

            double A = (double)Math.Floor(year / 100.0);
            double B = 2 - A + Math.Floor(A / 4);

            double JD = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + B - 1524.5;
            return JD;
        }


        //---------------------- Time-Zone Functions -----------------------


        // detect daylight saving in a given date
        /// <summary/>
        public bool UseDayLightaving(int year, int month, int day)
        {
            return System.TimeZoneInfo.Local.IsDaylightSavingTime(new DateTime(year, month, day));
            // TimeZone . CurrentTimeZone . IsDaylightSavingTime ( new DateTime ( year , month , day ) ); 
        }

        // ---------------------- Trigonometric Functions -----------------------

        // degree sin
        /// <summary/>
        public double Dsin(double d)
        {
            return Math.Sin(this.DegreeToRadian(d));
        }

        // degree cos
        /// <summary/>
        public double Dcos(double d)
        {
            return Math.Cos(this.DegreeToRadian(d));
        }

        // degree tan
        /// <summary/>
        public double Dtan(double d)
        {
            return Math.Tan(this.DegreeToRadian(d));
        }

        // degree arcsin
        /// <summary/>
        public double Darcsin(double x)
        {
            return this.RadianToDegree(Math.Asin(x));
        }

        // degree arccos
        /// <summary/>
        public double Darccos(double x)
        {
            return this.RadianToDegree(Math.Acos(x));
        }

        // degree arctan
        /// <summary/>
        public double Darctan(double x)
        {
            return this.RadianToDegree(Math.Atan(x));
        }

        // degree arctan2
        /// <summary/>
        public double Darctan2(double y, double x)
        {
            return this.RadianToDegree(Math.Atan2(y, x));
        }

        // degree arccot
        /// <summary/>
        public double Darccot(double x)
        {
            return this.RadianToDegree(Math.Atan(1 / x));
        }


        // Radian to Degree
        /// <summary/>
        public double RadianToDegree(double radian)
        {
            return (radian * 180.0) / Math.PI;
        }

        // degree to radian
        /// <summary/>
        public double DegreeToRadian(double degree)
        {
            return (degree * Math.PI) / 180.0;
        }

        /// <summary/>
        public double FixAngle(double angel)
        {
            angel = angel - 360.0 * (Math.Floor(angel / 360.0));
            angel = angel < 0 ? angel + 360.0 : angel;
            return angel;
        }

        // range reduce hours to 0..23
        /// <summary/>
        public double FixHour(double hour)
        {
            hour = hour - 24.0 * (Math.Floor(hour / 24.0));
            hour = hour < 0 ? hour + 24.0 : hour;
            return hour;
        }
    }
}