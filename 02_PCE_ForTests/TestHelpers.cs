using System;
using System.IO; // For the Console.Out stuff
using System.Text; // For StringBuilder

using NUnit.Framework;

/*
 * This file contains helper classes for the tests.  It does NOT contain any tests itself.
 * 
 * These helper routines are put here, in a separate file, so that it's easy to
 * copy-and-paste this single file between all the different starter projects that get
 * handed out, and yet still have a single, coherent copy of the code.
 * (Yeah, there's probably a better way to do this, but I wanted to keep things simple
 * for y'all :)  )
 */

namespace PCE_StarterProject
{
    public class TestHelpers
    {
        /// <summary>
        /// Use this Description to tag a TestFixture as containing at least one test we that want to grade
        /// </summary>
        public const string TEST_SUITE_DESC = "Exercise";
        /// <summary>
        /// Use this Description to tag an TestFixture as containing NO tests that we want to grade
        /// </summary>
        public const string TEST_SUITE_IGNORE_DESC = "Ignore These Exercises";

        // These should be the same for all PCEs:
        /// <summary>
        /// Use this Category (NOT Description) to indicate that this single test yields the default grade point penalty if it fails
        /// </summary>
        public const string CAT_DEFAULT = "Default";
        /// <summary>
        /// Use this Category (NOT Description) to indicate that this single test should NOT be graded.
        /// </summary>
        public const string CAT_DO_NOT_GRADE = "NOT GRADED";

        // Utility methods:
        public static bool ArraysTheSame(int[] a1, int[] a2)
        {
            if (a1.Length != a2.Length)
                return false;
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    Console.WriteLine("At slot " + i + ", found " + a1[i] + " vs. " + a2[i]);
                    return false;
                }
            }
            return true;
        }

        // Returns true if the first, contigiuous non-zero run of numbers in the
        // subset array is present anywhere in the src array.  src can contain
        // any number of zeros before and/or after subset
        public static bool ArraysTheSameIgnoreZeros(int[] src, int[] subset)
        {
            // Find first non-zero value in src:
            int iSrc = 0;
            for (; iSrc < src.Length && src[iSrc] == 0; iSrc++)
                ;

            // Find first non-zero value in subset:
            int iSubset = 0;
            for (; iSubset < subset.Length && subset[iSubset] == 0; iSubset++)
                ;

            // Are there any differences in the run of numbers?
            // NOTE: This will actually compare any trailing zeros, too
            for (; iSrc < src.Length &&
                   iSubset < subset.Length; iSrc++, iSubset++)
            {
                if (src[iSrc] != subset[iSubset])
                    return false;
            }

            // If the both arrays' remainders only contain zeros, then they're the same
            for (; iSrc < src.Length; iSrc++)
                if( src[iSrc] != 0)
                    return false;
            for (; iSubset < subset.Length; iSubset++)
                if (subset[iSubset] != 0)
                    return false;

            // If we're here, then each array had:
            //      an (optional) run of zeros, 
            //      an identical middle run of numbers
            //      an (optional) trailing run of zeros
            return true;
        }

        // Essentially returns true if a == b, in a 'fuzzy' sort of way
        // Actually returns true if there exists a number Num, such that Num is
        //      within a +/- tolerance, and Num == b
        public static bool EqualsFuzzy(int a, int b, int tolerance)
        {
            return a + tolerance >= b && a - tolerance <= b;
        }
        //  I'm avoiding generics/templates so that BIT 142 students can
        // make sense of this :)
        public static bool EqualsFuzzy(double a, double b, double tolerance)
        {
            return a + tolerance >= b && a - tolerance <= b;
        }

        // This is a convenience function - it does the fuzzy string compare, and
        // also prints out messages to the user, so that it's clear what should have
        // been generated (and what was actually generated)
        public static void DoStringComparison(string sCorrect, string sActual)
        {
            String msgToUser = "Expected the output\n" + sCorrect + "\nActually got:\n" +
                sActual + "END OF YOUR OUTPUT\n(END OF YOUR OUTPUT was added so it's clear what your output ends"; 

            Console.WriteLine(); // visual spacing between this & previous output
            Console.WriteLine(msgToUser);

            bool theSame = TestHelpers.EqualsFuzzyString(sCorrect, sActual);
            Assert.That(theSame == true, msgToUser);
        }
        /// <summary>
        /// Checks to see if two strings are the same, excepting minor differences
        /// 
        /// This method:
        /// * Gets rid of leading/trailing whitespace ( using Trim() )
        /// * Does the comparison in a case-insensitive manner
        /// </summary>
        /// <param name="aOriginal"></param>
        /// <param name="bOriginal"></param>
        /// <returns></returns>
        public static bool EqualsFuzzyString(string aOriginal, string bOriginal)
        {
            // chop any whitespace at the start/end
            string a = aOriginal.Trim();
            string b = bOriginal.Trim();

            // convert internal whitespace to 'cannonical' form - 
            // multiple blank spaces (space, tab) are converted to a 
            // single blank space
            a = CannonicalWhitespace(a);
            b = CannonicalWhitespace(b);

//            Console.WriteLine("Comparing:\n{0}\nto:\n{1}\n<<<", a, b);

            // Compare in a case-insensitive manner
            return String.Compare(a, b, true) == 0;
        }
        enum TypeOfChars
        {
            Transcribe, // anything we want to just copy over
            Whitespace,
            Newline
        }
        public static string CannonicalWhitespace(string src)
        {
            StringBuilder sbOut = new StringBuilder(src.Length);

            // Compress multiple blank spaces down to just one
            // Compress multiple newlines down to just one
            TypeOfChars currentRun = TypeOfChars.Transcribe;

            StringBuilder sbCurrentLine = new StringBuilder();
            for (int i = 0; i < src.Length; i++)
            {
                char c = src[i];

                if (Char.IsWhiteSpace(c))
                {
                    // Cannonical-ize newlines
                    // CRLFs -> default
                    if (is_CRLF_at(src, i) || is_CR_or_LF_at(src, i))
                    {
                        if (is_CRLF_at(src, i))
                            i++; // move past the LF ('\n')

                        // Remove multiple, blank lines & replace with a single line:
                        if (currentRun == TypeOfChars.Newline)
                            continue;
                        sbCurrentLine.AppendLine();
                        AddLineToEnd(sbOut, sbCurrentLine.ToString());
                        sbCurrentLine.Remove(0, sbCurrentLine.Length);

                        currentRun = TypeOfChars.Newline;
                        continue;
                    }

                    // if we've already begun the run of whitespace,
                    // then don't put more than 1 blank space in the output
                    if (currentRun == TypeOfChars.Whitespace)
                        continue;
                    else
                    {
                        // replace all spaces in this run with a single,
                        // 'cannonical' blank space (whitespace)
                        currentRun = TypeOfChars.Whitespace;
                        sbCurrentLine.Append( " " );
                    }
                }
                else
                {
                    // If we're here, it's because the next character is NOT a newline
                    currentRun = TypeOfChars.Transcribe;
                    sbCurrentLine.Append( c ); // add to end of curent line
                }
            }

            // Dump whatever's left into the string...
            AddLineToEnd(sbOut, sbCurrentLine.ToString());

            return sbOut.ToString();
        }
        // makes sure that each line has been trimmed, etc
        private static void AddLineToEnd(StringBuilder sbOut, String sLine)
        {
            sLine = sLine.Trim();
            sbOut.Append(sLine);
        }

        #region CannonicalWhitespace, original version
        //public static string CannonicalWhitespace(string src)
        //{
        //    StringBuilder sbOut = new StringBuilder(src.Length);

        //    // Compress multiple blank spaces down to just one
        //    // Compress multiple newlines down to just one
        //    TypeOfChars currentRun = TypeOfChars.Transcribe;

        //    for(int i = 0; i < src.Length; i++)
        //    {
        //        char c = src[i];

        //        if (Char.IsWhiteSpace(c))
        //        {
        //            // Cannonical-ize newlines
        //            // CRLFs -> default
        //            if (is_CRLF_at(src, i) || is_CR_or_LF_at(src, i))
        //            {
        //                if( is_CRLF_at(src,i) )
        //                    i++; // move past the LF ('\n')

        //                if (currentRun == TypeOfChars.Newline)
        //                    continue;

        //                sbOut.AppendLine();

        //                currentRun = TypeOfChars.Newline;
        //                continue;
        //            }

        //            // if we've already begun the run of whitespace,
        //            // then don'objectToTest put more than 1 blank space in the output
        //            if (currentRun == TypeOfChars.Whitespace)
        //                continue;
        //            else
        //            {
        //                // replace all spaces in this run with a single,
        //                // 'cannonical' blank space (whitespace)
        //                currentRun = TypeOfChars.Whitespace;
        //                sbOut.Append(" ");
        //            }
        //        }
        //        else
        //        {
        //            // If we're here, it's because the next character is NOT a newline
        //            currentRun = TypeOfChars.Transcribe;
        //            sbOut.Append(c);
        //        }
        //    }
        //    return sbOut.ToString();
        //}
        #endregion

        // // Test Cases:
        // string s = CannonicalWhitespace("Got     it"); -> "Got it"
        // "1\r\n5\r\n7\r\n20\r\n22\r\n25\r\n30" -> "1\n5\n..."
        // "1\r\n\r\n5\r\n\r\n7\r\n20\r\n22\r\n25\r\n30" -> "1\n5\n..."
        // "1\n\n5\r\n\r\n7\n20\n22\n25\n30" -> "1\n5\n..."
        // "1\n\r\n5\r\n\n7\n20\n22\n25\n30" -> "1\n5\n..."
        // "\r\n \t \r\n" -> "\n \n"

        // Retuns true if there's a CR-LF sequence at index i
        // if i is the last index in the string, handles that, too
        private static bool is_CRLF_at(string s, int i)
        {
            return i < s.Length &&
                   s[i] == '\r' &&
                   i + 1 < s.Length &&
                   s[i + 1] == '\n';
        }
        // returns true if index i is within the string, and
        // that character is either \r OR \n
        private static bool is_CR_or_LF_at(string s, int i)
        {
            return i < s.Length &&
                   (s[i] == '\r' ||
                   s[i] == '\n');
        }

        /// <summary>
        /// Given an array of integers that contains, say {1, 2, 10},
        /// return a string of the form
        /// [1, 2, 10]
        /// </summary>
        /// <param name="ar"></param>
        /// <returns></returns>
        public static String Array_ToString(int[] ar)
        {
            if( ar.Length == 0)
                return "[EMPTY ARRAY]";

            string s = "[" + ar[0];
            for (int i = 1; i < ar.Length; i++)
                s += "," + ar[i];
            s += "]";

            return s;
        }


        public static String PrintArrayToString(int[] nums)
        {
            return PrintArrayToString(nums, Order.Forwards);
        }
        public enum Order
        {
            Forwards,
            Backwards
        }
        /// <summary>
        /// Given an array of integers that contains, say {1, 2, 10},
        /// return a string of the form
        /// 1
        /// 2
        /// 10
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static String PrintArrayToString(int[] nums, Order or)
        {
            StringWriter sw = new StringWriter();

            if (or == Order.Forwards)
            {
                for (int i = 0; i < nums.Length; i++)
                    sw.WriteLine(nums[i]);
            }
            else // or == Order.Backwards
            {
                for (int i = nums.Length-1; i >= 0; i--)
                    sw.WriteLine(nums[i]);
            }
            return sw.ToString();
        }
        /// <summary>
        /// Given an array of integers that contains, say {1, 2, 10},
        /// PRINT the contents in the format
        /// 1
        /// 2
        /// 10
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static void PrintArray(int[] ar)
        {
            for (int i = 0; i < ar.Length; i++)
                Console.WriteLine(ar[i]);
        }

        public static void PrintArray(int[] ar, int from, int toIncluded)
        {
            for (int i = from; i <= toIncluded; i++)
                Console.WriteLine(ar[i]);
        }

        ///////////////////////////////////////////////////////////////////////
        //     Output capturing routines                                     //
        ///////////////////////////////////////////////////////////////////////
        private TextWriter originalOut = null;
        private TeeWriter capturedOut = null;

        private TextReader originalIn = null;
        private StringReader newIn = null;


        // If you know what you want to use for a sequence of console inputs, you
        // can set that up here.  
        // Unfortunately, you have to set everything up ahead of time.  You can't
        // set up some input, then grab something's output, then decide what the next
        // input should be...
        public void SetConsoleInput(string sInput)
        {
            sInput += "\n";
            // Console.WriteLine("Asked to set as console input:\n{0}\n<<<<", sInput);

            originalIn = Console.In;

            // clear out anything previously used...
            if (newIn != null)
                newIn.Dispose();

            newIn = new StringReader(sInput);
            Console.SetIn( newIn);
        }

        /// <summary>
        /// This method will set up everything so that Console.out output
        /// is captured to a string
        /// </summary>
        public void StartOutputCapturing()
        {
            originalOut = Console.Out;
            capturedOut = new TeeWriter(new StringWriter(), originalOut);
            Console.SetOut(capturedOut);
        }

        /// <summary>
        /// Stops the capture of output, restores the normal Console.Out stream, and
        /// returns a string 
        /// </summary>
        /// <returns>A string, containing all the output since capturing started</returns>
        public string StopOutputCapturing()
        {
            string sOut = "<NO OUTPUT CAPTURING IN PROGRESS>";

            if (capturedOut != null)
            {
                sOut = capturedOut.ToString();
                Console.SetOut(originalOut);
                capturedOut = null;
            }
            return sOut;
        }
    }

    /// <summary>
    /// Inspired by Unix's tee command, this will take two 
    /// TextWriters, wrap them, and write anything it's given to both.
    /// 
    /// The intended usage is to 'duplicate' output, so that a test's output
    /// will show up on the console AND be consumed by the NUnit test runner.
    /// 
    /// In order to implement the base class ok, the 'real' writer (assumed to be the
    /// temporary string writer) has all the methods directed towards it,
    /// while the 'Extra' writer only has output-related commands given to it.
    /// </summary>
    public class TeeWriter : System.IO.TextWriter
    {
        TextWriter writerReal;
        TextWriter writerExtra;
        public TeeWriter(TextWriter real, TextWriter extraForOutput)
        {
            writerReal = real;
            writerExtra= extraForOutput;
        }

        public override void Close()
        {
            writerReal.Close();
        }

        public override bool Equals(Object obj)
        {
            return writerReal.Equals(obj) && writerExtra.Equals(obj);
        }
        public override int GetHashCode()
        {
            return writerReal.GetHashCode();
        }

        public override void Flush()
        {
            writerReal.Flush();
            writerExtra.Flush();
        }

        public override String ToString()
        {
            return writerReal.ToString();
        }

        #region TextWriter.Write

        public override void Write(Boolean b)
        {
            writerReal.Write(b);
            writerExtra.Write(b);
        }
        public override void Write(Char x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(Char[] x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(Decimal x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(Double x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(Int32 x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(Int64 x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(Object x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(Single x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(String x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(UInt32 x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(UInt64 x)
        {
            writerReal.Write(x);
            writerExtra.Write(x);
        }
        public override void Write(String x, Object x2)
        {
            writerReal.Write(x, x2);
            writerExtra.Write(x, x2);
        }
        public override void Write(String x, Object[] x2)
        {
            writerReal.Write(x, x2);
            writerExtra.Write(x, x2);
        }
        public override void Write(Char[] x, Int32 x2, Int32 x3)
        {
            writerReal.Write(x, x2, x3);
            writerExtra.Write(x, x2, x3);
        }
        public override void Write(String x, Object x2, Object x3)
        {
            writerReal.Write(x, x2, x3);
            writerExtra.Write(x, x2, x3);
        }
        public override void Write(String x, Object x2, Object x3, Object x4)
        {
            writerReal.Write(x, x2, x3, x4);
            writerExtra.Write(x, x2, x3, x4);
        }
        #endregion
        #region TextWriter.WriteLine
        public override void WriteLine()
        {
            writerReal.WriteLine();
            writerExtra.WriteLine();
        }
        public override void WriteLine(Boolean x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(Char x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(Char[] x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(Decimal x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(Double x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(Int32 x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(Int64 x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(Object x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(Single x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(String x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(UInt32 x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(UInt64 x)
        {
            writerReal.WriteLine(x);
            writerExtra.WriteLine(x);
        }
        public override void WriteLine(String x, Object x2)
        {
            writerReal.WriteLine(x, x2);
            writerExtra.WriteLine(x, x2);
        }
        public override void WriteLine(String x, Object[] x2)
        {
            writerReal.WriteLine(x, x2);
            writerExtra.WriteLine(x, x2);
        }
        public override void WriteLine(Char[] x, Int32 x2, Int32 x3)
        {
            writerReal.WriteLine(x, x2, x3);
            writerExtra.WriteLine(x, x2, x3);
        }
        public override void WriteLine(String x, Object x2, Object x3)
        {
            writerReal.WriteLine(x, x2, x3);
            writerExtra.WriteLine(x, x2, x3);
        }
        public override void WriteLine(String x, Object x2, Object x3, Object x4)
        {
            writerReal.WriteLine(x, x2, x3, x4);
            writerExtra.WriteLine(x, x2, x3, x4);
        }
        #endregion

        public override System.Text.Encoding Encoding
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
	}
}
