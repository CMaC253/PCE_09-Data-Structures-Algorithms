#! /usr/bin/env python
#
#
# Usage: <script> rootDir
#   rootDir is the 'base' directory, under which all the PCE subdirectories must be located
#
#     WARNING: THIS _WILL_ DELETE THE "OUTPUT" FOLDERS!!
#
# Note: Yes, the odd name is so that tab-completion will uniquely pick this out.
#
from config import *
import os.path
import re
import shutil
import subprocess
import sys

# All paths are relative:
# 0 = path to Autograder directory
# 1 = path to 'src' (dir containing student folders)
# 2 = path to put all the output
G_PCEs = [  
            ["PCE 03", os.path.normpath("PCE03/PCE_03_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader"), "PCE03", os.path.normpath("PCE03/OUTPUT")],
            ["PCE 04", os.path.normpath("PCE04/PCE_04_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader"), "PCE04", os.path.normpath("PCE04/OUTPUT")],
            ["PCE 05", os.path.normpath("PCE05/PCE_05_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader"), "PCE05", "PCE05/OUTPUT"],
            ["PCE 06", os.path.normpath("PCE06/PCE_06_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader"), "PCE06", "PCE06/OUTPUT"],
            ["PCE 07", os.path.normpath("PCE07/PCE_07_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader"), "PCE07", "PCE07/OUTPUT"],
            ["PCE 08", os.path.normpath("PCE08/PCE_08_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader"), "PCE08", "PCE08/OUTPUT"],
            # ["PCE 09", "PCE09/PCE_09_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader", "PCE09", "PCE09/OUTPUT"],
            # ["PCE 10", "PCE10/PCE_10_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader", "PCE10", "PCE10/OUTPUT"]
            # BIT 142 files:
            # ["PCE 08", "PCE08/PCE_08_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader", "PCE08", "PCE08/OUTPUT"],
            # ["PCE 09", "PCE09/PCE_09_VS_2010_NUnit_2_5/01_PCE_Test_Runner/AutoGrader", "PCE09", "PCE09/OUTPUT"],
            
         ]
G_Grade_PCE_Script = "gradeAllInPCE.py"
G_Graded_Feedback = """[BIT 143-ST] {name} graded, feedback attached
Attached are your grades for the pre-class exercises for {name}. 

If you have any questions, you must bring them to the instructor's attention no later than the end of the next class.  Feel free to do that by replying to this email.If you do not contact the instructor about this PCE by the stated deadline, then your grade will be your final grade.

Since the official deadline for the last possible time to hand in any PCEs was last Monday, neither this nor any other PCEs will be accepted after this point in time.

Thanks!
--Mike"""
G_Missing_Submission = "[BIT 143-ST] {name} graded; I DID NOT GET YOUR {name} !!!!\n\nThe instructor did not receive your {name}.  If you previously handed in something for this PCE then your current grade will remain unchanged (but please do double-check StudentTracker to make sure that the grade you think you have is actually the one that I have recorded for you); if you have never handed in anything for this PCE then you will be getting a 0 (zero) for {name}.\n\nIf you have any questions, you must bring them to the instructor's attention no later than the end of the next class.  Feel free to do that by replying to this email.  If you do not contact the instructor about this PCE by the stated deadline, then your grade will be your final grade.\n\nSince the official deadline for the last possible time to hand in any PCEs was last Monday, neither this nor any other PCEs will be accepted after this point in time.\n\nThanks!\n--Mike"

def main(rootDir, rgPCEs):
    global RUNNING_ON_WINDOWS

    # print "in main"
    if rootDir[ len(rootDir) - 1 ] != os.sep:
        rootDir += os.sep
    # print "Root dir: " + repr(rootDir)

    print "PCEs: "
    for PCE in rgPCEs:
        for index, Dir in enumerate(PCE): #each elt will be a dir - make sure it ends ok
            if index != 0 and Dir[ len(Dir) - 1 ] != os.sep:
                Dir += os.sep
                PCE[index] = Dir

        # PCEName = PCE[0]
        # AGDir = rootDir + PCE[1]
        # SrcDir =rootDir +  PCE[2]
        # OutputDir = rootDir + PCE[3]
        
    for PCE in rgPCEs:
        # unpack the variables:
        PCEName = PCE[0]
        AGDir = rootDir + PCE[1]
        SrcDir =rootDir +  PCE[2]
        OutputDir = rootDir + PCE[3]

        print "======================= " + PCEName + "\n\n\tAG dir:" + AGDir + "\n\tSrc dir:" + SrcDir  + "\n\tOutput dir:" + OutputDir + "\n"
        
        # print "rootDir: " + rootDir
        # print "AGDir (starting): " + AGDir
        
        # if RUNNING_ON_WINDOWS:
            # print "RUNNING ON WINDOWS"
            # AGDir = Windowsify(AGDir)
            # SrcDir = Windowsify(SrcDir)
            # OutputDir = Windowsify(OutputDir)

        AG = AGDir + G_Grade_PCE_Script

        # change to autograder directory
        #    ("gradeAllInPCE.py" expects to be run from here)
        os.chdir(AGDir)

        # get rid of any old results by removing & recreatingthe output directory
        shutil.rmtree( OutputDir, True ) #ignore errors
        try: 
            if not os.access(OutputDir, os.W_OK ):
                os.makedirs( OutputDir )
        except Exception, ex:
            print ex
            # something went wrong - ignore this otherwise

            # Call the script to grade all the students here:
        # print "===================================================\n\n\n\nAbout To Call:\n" + AG + "\nSrcDir:" + SrcDir + "\nOutputDir:" + OutputDir+"\n\n"
        if RUNNING_ON_WINDOWS:
            retVal = subprocess.call(["python.exe", AG, SrcDir, OutputDir])
        else:
            retVal = subprocess.call([AG, SrcDir, OutputDir])

        print "======================= " + PCEName


        # Generate the emails we'll send:
        fnFeedbackEmail = OutputDir + "GRADED.txt"
        f = open(fnFeedbackEmail,'w')
        f.write( G_Graded_Feedback.format(name=PCEName) )
        f.close()

        fnMissingEmail = OutputDir + "MISSING.txt"
        f = open(fnMissingEmail,'w')  
        f.write( G_Missing_Submission.format(name=PCEName) )
        f.close()

        # And we're done!


if __name__ == "__main__":
    if len(sys.argv) != 2:
        scriptName = sys.argv[0]
        scriptName = re.sub(".*/", "", scriptName)
        print "Usage: <" + os.path.basename(scriptName) + "> rootDir\n\trootDir: Where the PCE03, PCE04, etc, folders are"
        print "WARNING: THIS _WILL_ DELETE THE \"OUTPUT\" FOLDERS!!"
        sys.exit(1) # fail!

    rootDir = sys.argv[1]
  
    main(rootDir, G_PCEs)
