using System;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Configuration;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Language related support class including langauge list and codes.
    /// </summary>
    /// <value> 
    /// The Arabic part is built upon "The Unicode Standard, Version 5.2" with plain, accented, and koranic chars.
    /// 
    /// For language codes see: ISO 639-2 Language Code List - Codes for the representation of names of languages (Library of Congress)
    /// See: http://www-01.sil.org/iso639-3/codes.asp?order=639_1&letter=%25
    /// See: http://en.wikipedia.org/wiki/List_of_ISO_639-2_codes
    /// </value>
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
    public class Language
    {
        private static XDocument xd;

        private const string latinPlainUpper = "\u0041-\u005a"; // ABCDEFGHIJKLMNOPQRSTUVWXYZ
        private const string latinPlainLower = "\u0061-\u007a"; // abcdefghijklmnopqrstuvwxyz
        private const string latinAccent = "\u00c0-\u00fc"; // ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûü

        // http://en.wikipedia.org/wiki/Cyrillic_script_in_Unicode
        private const string cyrillicPlain = "\u0400–\u04ff";
        private const string cyrillicSupplement = "\u0500–\u052f";
        private const string cyrillicExtendedA = "\u2de0–\u2dff";
        private const string cyrillicExtendedB = "\ua640–\ua69f";
        private const string choneticExtensions = "\u1d2b|\u1d78";

        private const string arabicPlain = "\u0621-\u063a|\u0641-\u064a"; // ءآأؤإئابةتثجحخدذرزسشصضطظعغـفقكلمنهوىي...
        private const string arabicAccent = "\u064b-\u0652"; //  ًٌٍَُِّْٕٖٜٓٔٗ٘ٙٚٛٝٞ
        private const string arabicDigit = "\u0660-\u0669";
        private const string arabicKoran = "\u0617-\u061a|\u06d6-\u06ed"; //  ۖۗۘۙۚۛۜ۝۞ۣ۟۠ۡۢۤۥۦۧۨ۩۪ۭ۫۬
        private const string arabicPoint = "\u0670";
        private const string arabicKoranExtended = "\u0671";
        private const string arabicExtended = "\u0671-\u06d3";
        //arabicJoined = "\ufe81-\ufefc";

        private const string hiragana = "\u3041-\u309f";
        private const string katakana = "\u30a0-\u30ff";
        private const string katakanaPhonecticExtensions = "\u31f0-\u31ff";
        private const string katakanaHalfwidth = "\uff65-\uff9f";

        private const string cjkUnifiedIdeographs = "\u4e00-\u9fbb";
        private const string cjkUnifiedIdeographsExtentionA = "\u3400-\u4dbf";
        private const string cjkUnifiedIdeographsExtentionB = "\u20000-\u200ff";
        private const string cjkCompatibilityIdeographs = "\f900-\uf9ff";
        private const string cjkCompatibilityIdeographsSupplement = "\u2f800-\u2f8bf";

        private const string hangulSyllables = "\uac00-\ud7af";
        private const string hangulJamo = "\u1100-\u11ff";
        private const string hangulCompatibilityJamo = "\u3130-\u318f";
        private const string hangulHalfwidth = "\uffa0-\uffdc";

        private const string latin = latinPlainLower + "|" + latinPlainUpper + "|" + latinAccent;

        private const string cyrillic = @"\w+"; //cyrillic_plain + "|" + cyrillicSupplement + "|" + cyrillicExtendedA + "|" + cyrillicExtendedB + "|" + choneticExtensions;

        private const string arabic = arabicPlain + "|" + arabicAccent + "|" + arabicDigit + "|" + arabicKoran + "|" + arabicPoint + "|" + arabicKoranExtended;
        private const string arabicNonWord = arabicAccent + "|" + arabicDigit + "|" + arabicKoran + "|" + arabicPoint + "|" + arabicKoranExtended;
        private const string kana = hiragana + "|" + katakana + "|" + katakanaPhonecticExtensions + "|" + katakanaHalfwidth;
        private const string hangul = hangulSyllables + "|" + hangulJamo + "|" + hangulCompatibilityJamo + "|" + hangulHalfwidth;

        //word = latin + "|" + arabic + "|" + kana + "|" + hangul;
        //ideograph = cjk_unified_ideographs + "|" + cjk_unified_ideographs_extention_a + "|" + cjk_unified_ideographs_extention_b + "|" + cjk_compatibility_ideographs + "|" + cjk_compatibility_ideographs_supplement;

        /// <summary/>
        public string Id { get; set; }

        /// <summary/>
        public string Symbol { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string EnglishName { get; set; }

        /// <summary/>
        public string ArabicName { get; set; }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public Language()
        {
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public Language(string iso_639_1)
        {
            Language language;

            language = LanguageByIso6391(iso_639_1);

            this.Id = language.Id;
            this.Name = language.Name;
            this.Symbol = language.Symbol;
            this.EnglishName = language.EnglishName;
            this.ArabicName = language.ArabicName;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static ArrayList ListOfAllArabicWords
        {
            get
            {
                string u;
                ArrayList wordArrayList;
                Assembly _assembly;
                //StreamReader streamReader;

                wordArrayList = null;
                _assembly = Assembly.GetExecutingAssembly();

                try
                {
                    using (var streamReader = new StreamReader(_assembly.GetManifestResourceStream("Ia.Cl.data.language.List of all Arabic words.txt")))
                    {
                        wordArrayList = new ArrayList(100000);

                        if (streamReader.Peek() != -1)
                        {
                            while (!streamReader.EndOfStream)
                            {
                                u = streamReader.ReadLine();
                                if (u.Length > 0) wordArrayList.Add(u.Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    wordArrayList = null;
                }
                finally
                {
                }

                return wordArrayList;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Language LanguageByIso6391(string iso_639_1)
        {
            Language language;

            language = (from q in XDocument.Elements("languageList").Elements("iso").Elements("language")
                        where q.Attribute("iso_639_1").Value == iso_639_1
                        select new Language
                        {
                            Id = q.Attribute("iso_639_1").Value,
                            Symbol = q.Attribute("iso_639_1").Value,
                            Name = q.Attribute("name").Value,
                            EnglishName = q.Attribute("englishName").Value,
                            ArabicName = q.Attribute("arabicName").Value
                        }
            ).First<Language>();

            return language;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Ideograph(string language)
        {
            string s;

            s = "";

            if (language == "en") s = "";
            else if (language == "es") s = "";
            else if (language == "fr") s = "";
            else if (language == "de") s = "";
            else if (language == "nl") s = "";
            else if (language == "ja") s = cjkUnifiedIdeographs + "|" + cjkUnifiedIdeographsExtentionA + "|" + cjkUnifiedIdeographsExtentionB + "|" + cjkCompatibilityIdeographs + "|" + cjkCompatibilityIdeographsSupplement;
            else if (language == "ko") s = cjkUnifiedIdeographs + "|" + cjkUnifiedIdeographsExtentionA + "|" + cjkUnifiedIdeographsExtentionB + "|" + cjkCompatibilityIdeographs + "|" + cjkCompatibilityIdeographsSupplement;
            else if (language == "zh") s = cjkUnifiedIdeographs + "|" + cjkUnifiedIdeographsExtentionA + "|" + cjkUnifiedIdeographsExtentionB + "|" + cjkCompatibilityIdeographs + "|" + cjkCompatibilityIdeographsSupplement;
            else if (language == "ar") s = "";

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string WordCharacters(string language)
        {
            string s;

            s = "";

            if (language == "en") s = latin;
            else if (language == "es") s = latin;
            else if (language == "fr") s = latin;
            else if (language == "de") s = latin;
            else if (language == "nl") s = latin;
            else if (language == "ru") s = cyrillic;
            else if (language == "ja") s = kana;
            else if (language == "ko") s = hangul;
            else if (language == "zh") s = null;
            else if (language == "ar") s = arabic;

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string WordsRegularExpression(string language)
        {
            string s;

            if (language == "ja") s = "[" + hiragana + "]+|[" + katakana + "]+|[" + katakanaPhonecticExtensions + "]+|[" + katakanaHalfwidth + "]+";
            else
            {
                s = "[" + WordCharacters(language) + "]+";
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string BasicWord(string language)
        {
            string s;

            s = "";

            if (language == "en") s = latinPlainLower;
            else if (language == "es") s = latinPlainLower;
            else if (language == "fr") s = latinPlainLower;
            else if (language == "de") s = latinPlainLower;
            else if (language == "nl") s = latinPlainLower;
            else if (language == "ru") s = cyrillic;
            else if (language == "ja") s = kana;
            else if (language == "ko") s = hangul;
            else if (language == "zh") s = null;
            else if (language == "ar") s = arabicPlain;

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string BasicWordsRegularExpression(string language)
        {
            string s;

            if (language == "ja") s = "[" + hiragana + "]+|[" + katakana + "]+|[" + katakanaPhonecticExtensions + "]+|[" + katakanaHalfwidth + "]+";
            else
            {
                s = "[" + BasicWord(language) + "]+";
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string NonWord(string language)
        {
            string s;

            s = "";

            if (language == "en") s = "";
            else if (language == "es") s = "";
            else if (language == "fr") s = "";
            else if (language == "de") s = "";
            else if (language == "nl") s = "";
            else if (language == "ja") s = "";
            else if (language == "ko") s = "";
            else if (language == "zh") s = "";
            else if (language == "ar") s = arabicNonWord;

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string BasicForm(string word)
        {
            // for Western languages, this function takes in a word and returns a copy of the word with all capital letters changed to small, and all
            // accent letters to standard ASCII ones. For Japanese and Korean, on the other hand, this function is not yet defined. It will just return the
            // same argument unchanged, for now.

            word = word.Replace("ß", "ss");
            word = word.ToLowerInvariant();

            word = word.Replace("ٱ", "ا");
            word = Regex.Replace(word, "[" + global::Ia.Cl.Model.Language.NonWord("ar") + "]", "");

            word = RemoveDiacritics(word);

            return word;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string RemoveDiacritics(string word)
        {
            // unaccenting, from "Sorting It All Out" http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx

            string stFormD = word.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);

                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(stFormD[ich]);
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generate an array of "similar" Arabic pronouciations of a word, like أحمد and احمد. 
        /// </summary>
        /// <param name="word">Word to find similars to it</param>
        /// <returns>ArrayList of words that will look similar to givin word</returns>
        public static ArrayList ProduceSimilarArabicWords(string word)
        {
            ArrayList al;
            Hashtable ht;

            ht = new Hashtable(20);
            al = new ArrayList(20);

            // add words to Hashtable:
            ht[word] = 1;
            ht[word.Replace("ـ", "")] = 1;

            ht[Regex.Replace(word, "\\bو", "و ")] = 1;
            ht[Regex.Replace(word, "\\bو\\s+", "و")] = 1;

            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "أ", "ا", ref ht);
            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "إ", "ا", ref ht);

            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "ى", "ي", ref ht);
            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "ة", "ه", ref ht);

            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "و", "ؤ", ref ht);
            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "عبد ", "عبد", ref ht);

            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "ابو ", "ابو", ref ht);
            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "داود ", "داوود", ref ht);

            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "آ", "ءا", ref ht);
            ProduceSimilarArabicWordsAddBothVariationsToArrayList(word, "آ", "ا", ref ht);

            foreach (string u in ht.Keys) if (u.Length > 0) al.Add(u);

            return al;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <param name="word"></param>
        /// <param name="variant1"></param>
        /// <param name="variant2"></param>
        /// <param name="ht"></param>
        /// <returns></returns>
        private static void ProduceSimilarArabicWordsAddBothVariationsToArrayList(string word, string variant1, string variant2, ref Hashtable ht)
        {
            ht[word.Replace(variant1, variant2)] = 1;
            ht[word.Replace(variant2, variant1)] = 1;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert Arabic numerals ١٢٣٤٥٦٧٨٩٠ to Latin 1234567890
        /// </summary>
        /// <param name="s">Arabic number in string format</param>
        /// <returns>Latin equivalent</returns>
        public static string ConvertArabicNumbersToLatin(string s)
        {
            s = s.Replace("١", "1");
            s = s.Replace("٢", "2");
            s = s.Replace("٣", "3");
            s = s.Replace("٤", "4");
            s = s.Replace("٥", "5");
            s = s.Replace("٦", "6");
            s = s.Replace("٧", "7");
            s = s.Replace("٨", "8");
            s = s.Replace("٩", "9");
            s = s.Replace("٠", "0");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert Latin numerals 1234567890 to Arabic ١٢٣٤٥٦٧٨٩٠
        /// </summary>
        /// <param name="s">Latin number in string format</param>
        /// <returns>Arabic equivalent</returns>
        public static string ConvertLatinNumbersToArabic(string s)
        {
            s = s.Replace("1", "١");
            s = s.Replace("2", "٢");
            s = s.Replace("3", "٣");
            s = s.Replace("4", "٤");
            s = s.Replace("5", "٥");
            s = s.Replace("6", "٦");
            s = s.Replace("7", "٧");
            s = s.Replace("8", "٨");
            s = s.Replace("9", "٩");
            s = s.Replace("0", "٠");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Correct an Arabic string to the proper format of Arabic
        /// </summary>
        /// <param name="name">Name to be examined</param>
        /// <returns>String of correct format</returns>
        public static string CorrectArabicNameNounStringFormat(string name)
        {
            name = Regex.Replace(name, @"\s+", @" ");
            name = name.Trim();

            // remove all 'ـ' chars
            name = name.Replace("ـ", "");

            // last 'ه' to 'ة' (on word border)
            // exceptions: 'الله' ...etc.
            if (!Regex.IsMatch(name, "\\bشاه\\b")) name = Regex.Replace(name, "ه\\b", "ة");
            name = name.Replace("اللة", "الله");

            // remove first 'دكتور' 'د' 'الدكتور'
            name = Regex.Replace(name, "\\bدكتور\\b", "");
            name = Regex.Replace(name, "\\bالدكتور\\b", "");
            name = Regex.Replace(name, "\\bدكتورة\\b", "");
            name = Regex.Replace(name, "\\bالدكتورة\\b", "");
            name = Regex.Replace(name, "\\bد\\b", "");

            name = Regex.Replace(name, @"\bعبد\s+", "عبد");
            name = Regex.Replace(name, @"\s+و\s+", " و");

            // first ''last 'ى' to 'ي' (on word border)
            if (!name.Contains("يسرى")
                && !name.Contains("يحيى")
                && !name.Contains("هدى")
                && !name.Contains("سلمى")
                && !name.Contains("منى")
                && !name.Contains("منتهى")
                && !name.Contains("ليلى")
                && !name.Contains("عيسى")
                && !name.Contains("موسى")
                && !name.Contains("سلوى")
                && !name.Contains("بشرى")
                && !name.Contains("صغرى")
                && !name.Contains("صدى")
                && !name.Contains("كبرى")
                && !name.Contains("مصطفى")
                && !name.Contains("ندى")
                && !name.Contains("يسرى")
                && !name.Contains("يمنى")
                && !name.Contains("مستشفى")
                && !name.Contains("تقوى")
                && !name.Contains("ذكرى")
                && !name.Contains("بشرى")
                && !name.Contains("موسيقى")
                && !name.Contains("ذكرى")
                && !name.Contains("ضحى")
                && !name.Contains("لبنى")
                && !name.Contains("ذكرى")
                && !name.Contains("مقتدى")
                && !name.Contains("مقهى")
                && !name.Contains("ملهى")
                && !name.Contains("منتدى")
                && !name.Contains("منتهى")
                && !name.Contains("يمنى")
                && !name.Contains("مرتضى")
                ) name = Regex.Replace(name, "ى\\b", "ي");

            // 
            name = Regex.Replace(name, "\\bاحمد", "أحمد");
            name = Regex.Replace(name, "\\bازياء", "أزياء");
            name = Regex.Replace(name, "\\bاوكسجين", "أوكسجين");
            name = Regex.Replace(name, "\\bاقبال", "إقبال");
            name = Regex.Replace(name, "\\bابيار", "أبيار");
            name = Regex.Replace(name, "اسنان", "أسنان");
            name = Regex.Replace(name, "[أ|ا]براهيم", "إبراهيم");
            name = Regex.Replace(name, "[أ|ا]سماعيل", "إسماعيل");
            name = Regex.Replace(name, "اجياد", "أجياد");
            name = Regex.Replace(name, "\\bامل\\b", "أمل");
            name = Regex.Replace(name, "\\bايوب\\b", "أيوب");
            name = Regex.Replace(name, "\\bايهاب\\b", "إيهاب");
            name = Regex.Replace(name, "\\bايمن\\b", "أيمن");
            name = Regex.Replace(name, "\\bايمان\\b", "إيمان");
            name = Regex.Replace(name, "\\bاياد\\b", "أياد");
            name = Regex.Replace(name, "\\bانيسة\\b", "أنيسة");
            name = Regex.Replace(name, "\\bانيس\\b", "أنيس");
            name = Regex.Replace(name, "\\bانور\\b", "أنور");
            name = Regex.Replace(name, "\\bانوار\\b", "أنوار");
            name = Regex.Replace(name, "\\bامينة\\b", "أمينة");
            name = Regex.Replace(name, "\\bامين\\b", "أمين");
            name = Regex.Replace(name, "\\bاميمة\\b", "أميمة");
            name = Regex.Replace(name, "\\bامير\\b", "أمير");
            name = Regex.Replace(name, "\\bاميرة\\b", "أميرة");
            name = Regex.Replace(name, "\\bامنة\\b", "آمنة");
            name = Regex.Replace(name, "\\bامثال\\b", "أمثال");
            name = Regex.Replace(name, "\\bاماني\\b", "أماني");
            name = Regex.Replace(name, "\\bامان\\b", "أمان");
            name = Regex.Replace(name, "\\bامال\\b", "آمال");
            name = Regex.Replace(name, "\\bام\\b", "أم");
            name = Regex.Replace(name, "\\bالهام\\b", "إلهام");

            // 'أل to 'ال'
            name = Regex.Replace(name, "\\bأل", "ال");

            // 'اا' at begining to 'ا'
            name = name.Replace("\\bاا", "ا");

            // reduce any 3 concecutive similar arabic letters to only 2
            name = name.Replace(@"(\d)\1\1", @"\1\1");

            name = Regex.Replace(name, @"\s+", @" ");
            name = name.Trim();

            return name;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Remove Non-Latin Characters
        /// </summary>
        /// <param name="name">Name to be clean</param>
        /// <returns>String of name cleaned</returns>
        public static string RemoveNonLatinCharacters(string name)
        {
            // remove all non Latin characters

            string s;

            s = "[^ " + WordCharacters("en") + "]";
            name = Regex.Replace(name, s, "");

            return name;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Remove non Arabic and non Arabic-Extended letters and digits
        /// </summary>
        /// <param name="name">Line to filter</param>
        /// <returns>Filtered line</returns>
        public static string RemoveNonArabicAndNonArabicExtendedLettersAndDigits(string name)
        {
            // 

            string s;

            s = "[^ " + arabicPlain + "|" + arabicDigit + "|" + arabicExtended + "]";
            name = Regex.Replace(name, s, "");

            return name;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert single digits to equivalent word digits
        /// </summary>
        /// <param name="s">String to process</param>
        /// <returns>Filtered string</returns>
        public static string ConvertSingleLatinDigitsToArabicWordEquivalents(string s)
        {
            s = ConvertArabicNumbersToLatin(s);

            s = s.Replace("1", "واحد");
            s = s.Replace("2", "إثنين");
            s = s.Replace("3", "ثلاثة");
            s = s.Replace("4", "أربعة");
            s = s.Replace("5", "خمسة");
            s = s.Replace("6", "ستة");
            s = s.Replace("7", "سبعة");
            s = s.Replace("8", "ثمانية");
            s = s.Replace("9", "تسعة");
            s = s.Replace("0", "صفر");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Remove Latin Transliterations of Subject References Of Arabic Words
        /// </summary>
        /// <param name="iso2"></param>
        /// <param name="name">Name to be clean</param>
        /// <returns>String of name cleaned</returns>
        public static string RemoveLatinTransliterationsOfSubjectReferencesOfArabicWords(string iso2, string name)
        {
            // 

            name = Regex.Replace(name, "\\bas\\b", "", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bal\\b", "", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bash\\b", "", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bat\\b", "", RegexOptions.IgnoreCase);

            name = Regex.Replace(name, "\\bad\\b", "", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bar\\b", "", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\ban\\b", "", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bath\\b", "", RegexOptions.IgnoreCase);

            name = Regex.Replace(name, "\\baz\\b", "", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\baz̧( |\\b)", "", RegexOptions.IgnoreCase); // different than above

            return name;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Produce Arabic Text of Latin Transliterations of Arabic Word Definit Article
        /// </summary>
        /// <param name="name">Latin Transliteration of Arabic word</param>
        /// <returns>Arabic text</returns>
        public static string ProduceArabicTextOfLatinTransliterationsOfArabicWordDefinitArticle(string name)
        {
            name = Regex.Replace(name, "\\bas\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bal\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bash\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bat\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bad\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bar\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\ban\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\bath\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\baz\\s*\\b", "ال", RegexOptions.IgnoreCase);

            name = Regex.Replace(name, "\\baş\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\baţ\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\baḑ\\s*\\b", "ال", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\badh\\s*\\b", "ال", RegexOptions.IgnoreCase);

            return name;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Remove space after the latin transliteration of Arabic word's definit article
        /// </summary>
        /// <param name="name">Latin Transliteration of Arabic word with article space</param>
        /// <returns>Latin transliteration without article space</returns>
        public static string RemoveSpaceAfterLatinTransliterationsOfArabicWordsDefinitArticle(string name)
        {
            name = Regex.Replace(name, "\\b(as|al|ash|at|ad|ar|an|ath|az|aş|aţ|aḑ|adh|az̧)\\s*\\b", "$1", RegexOptions.IgnoreCase);
            // note two different z and z̧

            return name;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Arabic text</param>
        /// <returns>Arabic text</returns>
        public static string RemoveWrongSpaceBetweenArabicDefinitArticleAndItsWord(string name)
        {
            name = Regex.Replace(name, "\\b[أ|ا]ت ت", "الت", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ث ث", "الث", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]د د", "الد", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ذ ذ", "الذ", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ر ر", "الر", RegexOptions.IgnoreCase);

            name = Regex.Replace(name, "\\b[أ|ا]ز ز", "الز", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]س س", "الس", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ش ش", "الش", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ص ص", "الص", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ض ض", "الض", RegexOptions.IgnoreCase);

            name = Regex.Replace(name, "\\b[أ|ا]ط ط", "الط", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ظ ظ", "الظ", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ل ل", "الل", RegexOptions.IgnoreCase);
            name = Regex.Replace(name, "\\b[أ|ا]ن ن", "الن", RegexOptions.IgnoreCase);

            name = Regex.Replace(name, "\\b[أ|ا]ل ", "ال", RegexOptions.IgnoreCase);

            return name;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Produce Approximate Arabic Text of Latin Transliterations of Arabic Words
        /// </summary>
        /// <param name="name">Latin Transliteration of Arabic word</param>
        /// <returns>Approximate Arabic text</returns>
        public static string ProduceApproximateArabicTextOfLatinTransliterationsOfArabicWords(string name)
        {
            // 

            /*
             * To match:
             * '’' use '\u2019'
             * '‘' use '\u2018'
             */

            name = name.ToLower();

            // for some reason I can not match "'\b"
            // does not work: name = Regex.Replace(name, "i\u2018\\b", "ع");
            // works(?): name = Regex.Replace(name, "i\u2018(\\b|$)", "ع");

            name = Regex.Replace(name, "ayyā", "يا");
            name = Regex.Replace(name, "iyah\\b", "ية");
            name = name.Replace("dhdh", "ذ");
            name = name.Replace("thth", "ث");
            name = name.Replace("shsh", "ش");
            name = Regex.Replace(name, "deid", "ديد");

            name = Regex.Replace(name, "lay", "لي");
            name = Regex.Replace(name, "way", "وي");
            name = Regex.Replace(name, "ain", "ين");
            name = Regex.Replace(name, "llá\\b", "لا");
            name = Regex.Replace(name, "iyā", "يا");
            name = name.Replace("yya", "ي");
            name = Regex.Replace(name, "\\bAya", "أيا");

            name = name.Replace("mm", "م");
            name = name.Replace("bb", "ب");
            name = name.Replace("dd", "د");
            name = name.Replace("ff", "ف");
            name = name.Replace("ss", "س");
            name = name.Replace("ll", "ل");
            name = name.Replace("rr", "ر");
            name = name.Replace("zz", "ز");
            name = name.Replace("nn", "ن");
            name = name.Replace("jj", "ج");
            name = name.Replace("ww", "و");
            name = name.Replace("qq", "ق");
            name = name.Replace("tt", "ت");
            name = name.Replace("ḩḩ", "ح");
            name = name.Replace("kk", "ك");
            name = name.Replace("ţţ", "ط");
            name = name.Replace("şş", "ص");
            name = name.Replace("ḑḑ", "ض"); // not same
            name = name.Replace("ḍḍ", "ض");
            name = name.Replace("ay", "ي");
            name = name.Replace("au", "و");
            name = name.Replace("āy", "اي");
            name = name.Replace("kh", "خ");
            name = name.Replace("sh", "ش");
            name = name.Replace("th", "ث");
            name = name.Replace("dh", "ض");
            name = name.Replace("gh", "غ");

            name = Regex.Replace(name, "ah\\b", "ة");
            name = Regex.Replace(name, "āt\\b", "ات");
            name = Regex.Replace(name, "at\\b", "ات");
            name = Regex.Replace(name, "ā\u2019i", "ائ");
            name = Regex.Replace(name, "ā\u2019(\\b|$)", "اء");
            name = Regex.Replace(name, "\u2018ā(\\b|$)", "عا");
            name = Regex.Replace(name, "\u2018a", "ع");
            name = Regex.Replace(name, "\u2018u", "ع");
            name = Regex.Replace(name, "\u2018ū", "عو");
            name = Regex.Replace(name, "\u2018ī", "عي");
            name = Regex.Replace(name, "i\u2018(\\b|$)", "ع");
            name = Regex.Replace(name, "i\u2018", "ئ");
            name = Regex.Replace(name, "i\u2019", "ئ");
            name = Regex.Replace(name, "ay\u2018\\b", "يع");
            name = Regex.Replace(name, "ay\\b", "ي");
            name = Regex.Replace(name, "ei", "ي");
            name = Regex.Replace(name, "yā", "يا");
            name = Regex.Replace(name, "ya", "ي");
            name = Regex.Replace(name, "īt\\b", "يت");
            name = Regex.Replace(name, "ḩá\\b", "حا");

            name = Regex.Replace(name, "t\\b", "ة");
            name = name.Replace("ş", "ص");
            name = name.Replace("ḑ", "ض"); // not same
            name = name.Replace("ḍ", "ض");
            name = name.Replace("ţ", "ط");
            name = name.Replace("ā", "ا");
            name = name.Replace("a’", "ا");
            name = name.Replace("ī", "ي");
            name = name.Replace("ū", "و");
            name = name.Replace("ḩ", "ح");

            name = Regex.Replace(name, "\\ba", "أ");
            name = Regex.Replace(name, "\\bu", "أ");
            name = Regex.Replace(name, "\\bi", "أ");
            name = Regex.Replace(name, "i\\b", "ي");
            name = Regex.Replace(name, "y\\b", "ي");

            name = name.Replace("a", "");
            name = name.Replace("b", "ب");
            name = name.Replace("t", "ت");
            //name = name.Replace("c", "");
            name = name.Replace("j", "ج");
            name = name.Replace("g", "ج");
            name = name.Replace("d", "د");
            name = name.Replace("r", "ر");
            name = name.Replace("r", "ر");
            name = name.Replace("s", "س");
            name = name.Replace("t", "ت");
            name = name.Replace("d", "د");
            name = name.Replace("e", "");
            name = name.Replace("h", "ه");
            name = name.Replace("i", "");
            name = name.Replace("j", "ج");
            name = name.Replace("p", "ب");
            name = name.Replace("f", "ف");
            name = name.Replace("q", "ق");
            name = name.Replace("k", "ك");
            name = name.Replace("l", "ل");
            name = name.Replace("m", "م");
            name = name.Replace("n", "ن");
            name = name.Replace("h", "ه");
            name = name.Replace("u", "");
            name = name.Replace("v", "ف");
            name = name.Replace("o", "");
            name = name.Replace("w", "و");
            name = name.Replace("y", "");
            name = name.Replace("w", "و");
            name = name.Replace("z̧", "ظ"); // note this is not 'z'. KEEP THIS ORDER.
            name = name.Replace("z", "ز");
            name = name.Replace("á", "اء");

            name = Regex.Replace(name, "\\b\u2018", "ع");

            //name = name.Replace("7", "ح");
            //name = name.Replace("3", "ع");
            //name = name.Replace("6", "ط");

            // name = name.Replace("", "");
            // name = Regex.Replace(name, "\\b\\b", "");

            return name;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// From a country code this will return the main language used in this country.
        /// </summary>
        /// <param name="countryCode">Country code (iso2)</param>
        /// <returns>Language Code (ISO 639-2)</returns>
        public static string ReturnLanguageCodeUsedInCountryFromCountyCode(string countryCode)
        {
            // Incomplete

            string languageCode;

            countryCode = countryCode.ToLower();

            switch (countryCode)
            {
                // Korea
                case "kr":
                case "kp": languageCode = "ko"; break;

                // Japan
                case "jp": languageCode = "ja"; break;

                // Arab countries
                case "dz":
                case "bh":
                case "eg":
                case "iq":
                case "jo":
                case "kw":
                case "lb":
                case "ly":
                case "mr":
                case "ma":
                case "om":
                case "ps":
                case "qa":
                case "sa":
                case "so":
                case "sd":
                case "sy":
                case "tn":
                case "ae":
                case "ye": languageCode = "ar"; break;

                // Iran, Afghanistan, Tajikistan
                case "ir":
                case "af":
                case "tj": languageCode = "fa"; break; // Persian

                // English
                default: languageCode = "en"; break; // English
            }

            return languageCode;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static string ReturnUserLanguage(HttpRequest userLanguages)
        {
            string language;
            string[] languages = HttpContext.Current.Request.UserLanguages;

            if (languages == null || languages.Length == 0)
            {
                language = null;
            }
            else
            {
                try
                {
                    language = languages[0].ToLowerInvariant().Trim();
                    language = language.Substring(0, 2);
                }
                catch (ArgumentException)
                {
                    language = null;
                }
            }

            return language;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Remove the small Koranic jawaz and sili characters because they cause the displayed text to render unstable
        /// </summary>
        /// <param name="line"></param>
        /// <returns>cleaned line</returns>
        public static string RemoveSmallKoranicJawazAndSiliCharacters(string line)
        {
            // 
            // remove ۚ and ۖ

            // remove the small character as it makes text unstable
            line = line.Replace(" ۚ", ""); // small ج
            line = line.Replace(" ۖ", ""); // small  صلى

            return line;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <param name="line"></param>
        /// <returns>cleaned line</returns>
        public static string SlightlyChangeSomeSentencesToPreventSystemFromGeneratingSingleCharacters(string line)
        {
            // slightly change sentences to prevent system single characters
            //s = s.Replace("الله", "اللـه");
            //s = s.Replace("ريال", "ريـال");
            //s = s.Replace("محمد", "مـحمد");
            //s = s.Replace("جل جلاله", "جل جلالـه");
            line = line.Replace("صلى الله عليه وسلم", "صـلى الله عليه وسلم");

            return line;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Pluralize(string word)
        {
            string s;

            s = PluralizationService.CreateService(CultureInfo.CurrentCulture).Pluralize(word);

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks if a word contains a German diacritic letter.
        /// <see cref="http://en.wikipedia.org/wiki/German_alphabet"/>
        /// </summary>
        /// <remarks> 
        /// German uses letter-diacritic combinations (Ä/ä, Ö/ö, Ü/ü) using the umlaut and one ligature (ß (called eszett (sz) or scharfes S, sharp s)), but they do not constitute distinct letters in the alphabet.
        /// </remarks>
        /// </summary>
        public static bool ContainsGermanDiacriticLetter(string line)
        {
            bool lineContainsGermanDiacriticLetter;

            if (line != null)
            {
                lineContainsGermanDiacriticLetter = Regex.IsMatch(line, "[ÄäÖöÜüß]");
            }
            else lineContainsGermanDiacriticLetter = false;

            return lineContainsGermanDiacriticLetter;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// How to embed and access resources by using Visual C# http://support.microsoft.com/kb/319292/en-us
        /// 
        /// 1. Change the "Build Action" property of your XML file from "Content" to "Embedded Resource".
        /// 2. Add "using System.Reflection".
        /// 3. See sample below.
        /// 
        /// </summary>

        public static XDocument XDocument
        {
            get
            {
                Assembly _assembly;
                StreamReader streamReader;

                xd = null;
                _assembly = Assembly.GetExecutingAssembly();
                streamReader = new StreamReader(_assembly.GetManifestResourceStream("Ia.Cl.model.language.xml"));

                try
                {
                    if (streamReader.Peek() != -1)
                    {
                        xd = System.Xml.Linq.XDocument.Load(streamReader);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }

                return xd;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}
