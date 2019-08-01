using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using System.Xml; // to compensate for XLST not having variables that, y'know, vary
using System.Xml.Xsl;

/*
 * This file contains code that runs the tests.
 * 
 * Other than the first couple of lines in main (next to the comment with the word
 * 'STUDENTS' in it), you shouldn't need to understand anything in this file.
 * 
 * 
 * 
 * This program will launch the unit tests, in one of three ways:
 * 1) in the GUI
 * 2) under the debugger
 * 3) using the console-runner + XSLT to produce a gradesheet for this project
 * 
 * In order to enable this project:
 * A) right-click on the ConsoleRunner project, and select 'Set As Startup Project'
 * B) in Main, below, assign the appropriate constant to the doThis variable
 * C) compile + run!
 * 
 * Details for the GUI:
 * None.
 *      (You'll need to select which test you want to run, the click 'Run', 
 *      but there sholdn't be anything else you need to do
 * 
 * Details for the debugger:
 *      You can set breakpoints your source code, back in the PCE_StudentCode project
 *      for your code, or in the PCE_Tests project if you want to stop in a particular test
 *      Once you've got them set, Press F5 (Start (with debugging))
 *      will NOT see the GUI
 *      You WILL be able to use the debugger on your code, if you choose
 * 
 * Details for the Gradesheet-Producing Mode:
 * None.
 *      You'll see console output scroll by, but everything should work fine on it's own.
 *      When the gradesheet is ready, Visual Studio will launch IE (or FF, etc) so you
 *      can view the file.
 * 
 * TODO: Fixup the double->int thing 
 *      (return value of ProduceGrades is used as int value for program)
 * 
 */

namespace PCE_ForTests_Express
{
    class RunTests
    {
        private const int RUN_TEST_IN_GUI = 1;
        private const int RUN_TESTS_UNDER_DEBUGGER = 2;
        private const int PRODUCE_GRADESHEET = 3;

        // Return value: The operating system will be given the final grade
        // for the student (gradesheet mode), Int32.MinValue otherwise
        static int Main(string[] args)
        {

            int doThis;
            // STUDENTS: UNCOMENT ONE OF THESE LINES TO SELECT HOW TO RUN THE PROJECT
             doThis = RUN_TEST_IN_GUI;
            // doThis = RUN_TESTS_UNDER_DEBUGGER;
             doThis = PRODUCE_GRADESHEET;

             // change doThis, if the command-line parameters say so
            overrideMode(ref doThis, args); 

            string[] NUnitArgs = { "PCE_Test.dll" };
            int mainReturnValue = Int32.MinValue;

            switch (doThis)
            {
                case RUN_TEST_IN_GUI:
                    System.Diagnostics.Process.Start(@"..\..\..\net-2.0\NUnit.exe",
                                                    "PCE_Test.dll");
                    break;

                case RUN_TESTS_UNDER_DEBUGGER:
                    // Use the 'Debug->Start Debugging' menu option to debug your test(s)
                    int result = NUnit.ConsoleRunner.Runner.Main(NUnitArgs);
                    break;

                case PRODUCE_GRADESHEET:
                    bool displayInBrowser = true;
                    foreach (string arg in args)
                    {
                        if (arg == "NO_BROWSER")
                        {
                            displayInBrowser = false;
                            break;
                        }
                    }

                    mainReturnValue = (int)Produce_Gradesheet.ProduceGrades(
                        NUnitArgs, displayInBrowser);
                    break;
            }

            Console.WriteLine("TEST RUNNER: Final Grade: {0}", mainReturnValue);
            return mainReturnValue;
        }

        public static void overrideMode(ref int doThis, string[] args)
        {
            foreach (string s in args)
            {
                if (String.Compare(s, "MODE_GRADESHEET", true) == 0)// ignore case
                {
                    doThis = RunTests.PRODUCE_GRADESHEET;
                    return;
                }
                if (String.Compare(s, "MODE_GUI", true) == 0)// ignore case
                {
                    doThis = RunTests.RUN_TEST_IN_GUI;
                    return;
                }
                // if none match, the doThis retains it's original value.
            }
        }
    }

    class Produce_Gradesheet
    {
        // Where are all the Auto-grader related files?  
        // Note: this needs to end in the directory separator
        private const string AG_DIR_PREFIX = @"..\..\AutoGrader\";
        private const string DISPLAY_DIR_PREFIX = @"..\..\..\";

        // fn = "FileName"
        private const string FN_NUNIT_OUTPUT = "TestResult.xml";
        private const string FN_XFORM_NUNIT_TO_ST= "AutoGrader.xslt";
        private const string FN_ST_INTERMEDIATE_FILE = "Grades.xml";

        private const string FN_XFORM_DISPLAY = "Display.xslt";
        private const string FN_DISPLAY = "Grades.html";
        private const string FN_DISPLAY_CSS = "Grades.css";

        private const string ELT_RAW_GRADE = "RawGrade"; 
        private const string ELT_OVERALL_GRADE = "OverallGrade";
        private const string ELT_TEST_DATETIME = "TestDateTime";

        private const string TEST_SUITE= "test-suite";
        private const string TEST_SUITE_DESC = "Exercise";
        private const string TEST_SUITE_IGNORE_DESC = "Ignore These Exercises";
        private const string sEltName = "test-case";
        private const string defaultCategory = "Default";
        private const string missingCategory = "No Category Found";


        // returns the final grade for the student
        public static double ProduceGrades(string[] args, bool displayInBrowser)
        {
            int result = NUnit.ConsoleRunner.Runner.Main( args );
            // Console.WriteLine(result);

            FixNUnitOutput(FN_NUNIT_OUTPUT);

            XslCompiledTransform xslt = new XslCompiledTransform(true);

            // Step 1: Transform NUnit output into easier to deal with ST-AG format
            xslt.Load(AG_DIR_PREFIX + FN_XFORM_NUNIT_TO_ST); // Load AutoGrader.xslt
            xslt.Transform(FN_NUNIT_OUTPUT, FN_ST_INTERMEDIATE_FILE); // Tranform: NUnitOutput.xml -> Grades.xml

            double grade = collectAndAppendGrade(FN_ST_INTERMEDIATE_FILE); // fixup Grades.xml 
                                                                  //    (add timestamp, grade total)
            
            Console.WriteLine("Your final grade is: {0}", grade);

            // Step 2: Transform ST-AG raw format into html for display
            string fnDisplay = DISPLAY_DIR_PREFIX + FN_DISPLAY;
            XsltSettings settings = new XsltSettings(true, true);
            
            xslt.Load(AG_DIR_PREFIX + FN_XFORM_DISPLAY, XsltSettings.TrustedXslt, new XmlUrlResolver() ); // Load Display.xslt
            xslt.Transform(FN_ST_INTERMEDIATE_FILE, fnDisplay);   // Transform: Grades.xml -> Grades.html

            // Now that .CSS is included in the .HTML file, it's no longer
            // necessary to copy .CSS to the output directory...
            // Copy .CSS into target directory, if it's not there already...
            // if( ! System.IO.File.Exists(DISPLAY_DIR_PREFIX + FN_DISPLAY_CSS) )
            //    System.IO.File.Copy(AG_DIR_PREFIX + FN_DISPLAY_CSS, DISPLAY_DIR_PREFIX + FN_DISPLAY_CSS);

            // Step 3: Display the page (relies on OS to find a browser..
            if( displayInBrowser )
            System.Diagnostics.Process.Start(fnDisplay);

            return grade;
        }

        static void FixNUnitOutput(string fnNUnitOutput)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(fnNUnitOutput);

            Console.WriteLine("\n\n\n\n\n\n");

            FixNUnitOutput(doc.DocumentElement, null);

            Console.WriteLine("\n\n\n\n\n\n");

            doc.Save(fnNUnitOutput);
        }

        static void FixNUnitOutput(XmlNode cur, Dictionary<string, List<XmlNode>> Categories)
        {
            bool atNodeToOutputCategories;

            // If we're at a 'test-suite' node, and either there is no description attribute,
            // or there is but the description isn't 'exercise', then skip it (and it's children)
            if (String.Compare(cur.Name, TEST_SUITE, true) == 0 &&
                cur.Attributes["description"] != null &&
                String.Compare(cur.Attributes["description"].Value, TEST_SUITE_IGNORE_DESC, true) == 0)
            {
                return;
            }

            // If we're at a results node, which is the child of a test-suite node that's
            // been tagged with the description "Exercise", then we'll want to put the
            // Categories/Category nodes here, so initialize the collection

            atNodeToOutputCategories = String.Compare( cur.Name, "results", true) == 0 && 
                cur.ParentNode != null &&
                String.Compare(cur.ParentNode.Name, TEST_SUITE, true) == 0 &&
                cur.ParentNode.Attributes["description"] != null &&
                String.Compare(cur.ParentNode.Attributes["description"].Value, TEST_SUITE_DESC, true) == 0;

            if (atNodeToOutputCategories)
            {
                // This had better be null at this point!
                Categories = new Dictionary<string, List<XmlNode>>();
            }

            // Clone the list of XML child nodes
            // so that modifications to the real node will NOT
            // be reflected here...
            List<XmlNode> kids = new List<XmlNode>() ;
            foreach (XmlNode child in cur.ChildNodes)
            {
                kids.Add(child);
            }

            foreach( XmlNode child in kids )
            {

                if( string.Compare( child.Name, sEltName, true) == 0 )
                {
                    if (null == Categories)
                    {
                        // Another option is to return, maybe?
                        throw new ApplicationException("Malformed input document - my assumptions about finding a test-suite w/ the description=\"Exercise\" before finding a test-case didn't hold up!");
                    }

                    // This line was used for when the categories were placed into the description
                    // string catName = child.Attributes[sAttrDesc] != null ? child.Attributes[sAttrDesc].Value : missingCategory;

                    string catName = DetectFirstCategory(child);
                    if (catName == null)
                        // try again, looking for the category of a test-suite (such as when generated for TestCase)
                        catName = DetectFirstCategoryOfTestSuiteParent(child);
                    if (catName == null)
                        catName = missingCategory;

                    // Remove this child node from the current node AFTER we've (possibly) looked in the parent for the category
                    cur.RemoveChild(child);

                    Console.WriteLine("Found a test case node: name={0} category={1}", child.Attributes["name"].Value, catName);

                    List<XmlNode> existingCat;

                    // If the category has not yet been seen for this node, then create it
                    if (!Categories.TryGetValue(catName, out existingCat))
                    {
                        existingCat = new List<XmlNode>();
                        Categories.Add(catName, existingCat);
                    }

                    existingCat.Add(child);
                }
                if(child.NodeType == XmlNodeType.Element &&  child.ChildNodes.Count > 0)
                    FixNUnitOutput(child, Categories);
            }

            // only add in the new categories if there is at least one....
            if (atNodeToOutputCategories && 
                Categories != null && 
                Categories.Count > 0)
            {
                XmlNode cats = cur.OwnerDocument.CreateElement( "Categories" );
                cur.AppendChild(cats);
                foreach (KeyValuePair<string, List<XmlNode>> category in Categories)
                {
                    // Add new category to overall list:
                    XmlNode thisCat = cats.OwnerDocument.CreateElement("Category");
                    cats.AppendChild(thisCat);

                    // Add name to this particular category:
                    XmlAttribute catName = thisCat.OwnerDocument.CreateAttribute("name");
                    catName.Value = category.Key;
                    thisCat.Attributes.Append(catName);


                    foreach (XmlNode node in category.Value)
                    {
                        thisCat.AppendChild(node); 
                    }
                }
                Categories.Clear();
            }
        }

        private static string DetectFirstCategory(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element &&
                    child.Name == "categories")
                {
                    foreach (XmlNode subchild in child.ChildNodes)
                    {
                        if (subchild.NodeType == XmlNodeType.Element &&
                            subchild.Name == "category")
                        {
                            return subchild.Attributes["name"].Value;
                        }
                    }
                }
            }
            return null;
        }
        // A test-case within 
        private static string DetectFirstCategoryOfTestSuiteParent(XmlNode node)
        {
            if (node.ParentNode == null) // .ParentNode is the results node of the test-suite
                goto DidntFindCategory;
            if( node.ParentNode.ParentNode == null ||
                node.ParentNode.ParentNode.Name != "test-suite")
                goto DidntFindCategory;
            XmlNode testSuite = node.ParentNode.ParentNode;

            // FirstChild can't be null - if nothing else it'll be this test-case
            if( testSuite.FirstChild.Name != "categories")
                goto DidntFindCategory;
            if( testSuite.FirstChild.FirstChild == null ||
                testSuite.FirstChild.FirstChild.Name != "category")
                goto DidntFindCategory;
            XmlNode cat = testSuite.FirstChild.FirstChild;
            return cat.Attributes[0].Value;

        DidntFindCategory:
            return null;
        }
        // This entire function is silly. The problem is that a "variable" in XSLT can't change
        // it's value (i.e., the XSLT variables can't vary), therefore we need to accumulate
        // the overall grade for the thingee here. 
        //
        // After accumulating the overall grade, that value will then be appended to the document
        // in a "OverallGrade" node, with the attribute value="x" indicating that the final grade is x
        //
        // Update: XSLT is also silly because it doesn't appear to have a way to get the current date/time
        //      so while we're accumulating the grade, we'll also add this timestamp in.
        //
        static double collectAndAppendGrade(string fnIntermediate)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.IgnoreWhitespace = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            XmlReader reader = XmlReader.Create(fnIntermediate, settings);

            // get StartingGrade="100" MinGrade="0" MaxGrade="100"
            // from StudentTrackerGradeDigest

            double grade = 0.0;
            double minGrade = Double.MinValue;
            double maxGrade = Double.MaxValue;

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    if (reader.Name == "StudentTrackerGradeDigest")
                    {
                        string szVal = reader.GetAttribute("MinGrade");
                        double temp;
                        if (Double.TryParse(szVal, out temp))
                            minGrade = temp;
                        else
                            Console.WriteLine("ERROR IN GRADESHEET: {0} is not a valid (numeric) minimum grade!\n\tUsing {1} instead", szVal, minGrade);
                        szVal = reader.GetAttribute("MaxGrade");
                        if (Double.TryParse(szVal, out temp))
                            maxGrade = temp;
                        else
                            Console.WriteLine("ERROR IN GRADESHEET: {0} is not a valid (numeric) maximum grade!\n\tUsing {1} instead", szVal, maxGrade);
                    }
                    if (reader.Name == "SetGrade")
                    {
                        string szVal = reader.GetAttribute("value");
                        double temp;
                        if (Double.TryParse(szVal, out temp))
                            grade = temp;
                        else
                            Console.WriteLine("ERROR IN GRADESHEET: {0} is not a valid (numeric) grade!", szVal);
                    }

                    if (reader.Name == "ModGrade")
                    {
                        string szVal = reader.GetAttribute("value");
                        double temp;
                        if (Double.TryParse(szVal, out temp))
                            grade += temp; // ModGrade's value may very well be negative
                        else
                            Console.WriteLine("ERROR IN GRADESHEET: {0} is not a valid (numeric) grade!", szVal);
                    }

                    if( grade > maxGrade)
                        grade = maxGrade;
                    if( grade < minGrade)
                        grade = minGrade;
                }
            }

            reader.Close();

            XmlDocument doc = new XmlDocument();
            doc.Load(fnIntermediate);
            doc.DocumentElement.SetAttribute(ELT_RAW_GRADE, grade.ToString());
            grade = RawGradeToOverallGrade(grade, maxGrade);
            doc.DocumentElement.SetAttribute(ELT_OVERALL_GRADE, grade.ToString());
            doc.DocumentElement.SetAttribute(ELT_TEST_DATETIME, DateTime.Now.ToString("yyyy/MM/dd @ HH:mm:ss"));

            doc.Save(fnIntermediate);
            
            return grade;
        }

        // If student gets 6 points or higher then they get a 2 point bump, up to the maximum
        private static double RawGradeToOverallGrade(double raw, double max)
        {
            if (raw >= 6)
                return Math.Min(raw + 2, max);
            else
                return raw;
        }
    }
}
