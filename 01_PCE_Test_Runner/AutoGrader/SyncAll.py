#! /usr/bin/env python

# To Sync a bunch of files using Bash:
# find -L . -name RunTests\.cs -execdir cp /cygdrive/e/Work/Student_Work/BIT_143_New/PCE03/PCE_03_VS_2010_NUnit_2_5/01_PCE_Test_Runner/RunTests.cs '{}' \;

import os
import sys

destinations = [
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE03\\PCE_03_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT143\\Lessons\\Lesson_03\\PCEs\\PCE_03_VS_2010_NUnit_2_5',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE04\\PCE_04_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT143\\Lessons\\Lesson_04\\PCEs\\PCE_04_VS_2010_NUnit_2_5',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE05\\PCE_05_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT143\\Lessons\\Lesson_05\\PCEs\\PCE_05_VS_2010_NUnit_2_5',                
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE06\\PCE_06_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT143\\Lessons\\Lesson_06\\PCEs\\PCE_06_VS_2010_NUnit_2_5',                
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE07\\PCE_07_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT143\\Lessons\\Lesson_07\\PCEs\\PCE_07_VS_2010_NUnit_2_5',                
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE08\\PCE_08_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT143\\Lessons\\Lesson_08\\PCEs\\PCE_08_VS_2010_NUnit_2_5',                
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE09\\PCE_09_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT143\\Lessons\\Lesson_09\\PCEs\\PCE_09_VS_2010_NUnit_2_5',                
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE10\\PCE_10_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT143\\Lessons\\Lesson_10\\PCEs\\PCE_10_VS_2010_NUnit_2_5',                
                'E:\\Work\\Student_Work\\BIT_142_New\\PCE08\\PCE_08_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT142\\Lessons\\Lesson_08\\PCEs\\PCE_08_VS_2010_NUnit_2_5',
                'E:\\Work\\Student_Work\\BIT_142_New\\PCE09\\PCE_09_VS_2010_NUnit_2_5\\',
                'E:\\Work\\Website\\Courses\\BIT142\\Lessons\\Lesson_09\\PCEs\\PCE_09_VS_2010_NUnit_2_5',
                ]
#filesToCopy = [
#               'All_Grades.css',
#               'AutoGrader.xslt',
#               'config.py',
#               'Display.xslt',
#               'gradeAllInPCE.py',
#               'Grades.css.xml',
#               'Mono.py',
#               'PCEs_gradeAll.py',
#               'SyncAll.py',
#               'TextToReplace.xslt'
#               ]

filesToExclude = " ".join([ # files to NOT copy:
                    # 01_PCE_Test_Runner:
                   'additionalFiles.py',
                   'GradingInfo.xslt',
                   # 02_PCE_For_Tests:
                   'PCE_Tests_To_Run.cs',
                   'BinarySearchTree_Verifier.cs',
                   'LinkedList_Verifier.cs',
                   'BinarySearchTree_Verifier.cs',
                   'Recursion_Testing_Subclasses.cs',
                   'Recursion_Verifier.cs',
                   # 03_PCE_Student_Code:
                   'Student_Answers.cs'
                 ])
# ROBOCOPY = "robocopy %s %s /e /r:0 /v /xf " + filesToExclude # Verbose
ROBOCOPY = "robocopy %s %s /e /r:0 /xf " + filesToExclude# Concise

def abspathAll(listOfPaths):
    newListOfPaths = []
    for path in listOfPaths: # "slice copy" of listOfPaths
        newListOfPaths.append( os.path.abspath(path) )
    
    return newListOfPaths

destinations = abspathAll(destinations)
cwd = os.path.abspath(os.getcwd()).lower()

# Go through dest list & identify which destination is actually the current working dir
#    Use that dest as source
#    Remove that dest from list

srcDir = None
for dest in destinations:
    destLower = dest.lower()
    commonPre = os.path.commonprefix([ destLower, cwd] )
    if commonPre == destLower:
        srcDir = commonPre
        destinations.remove(dest)
        print 'Found src dir to be %s' % srcDir
        break

if srcDir == None:
    print "The current working dir is NOT a valid 'source' directory\n\t(it must be a subdir of one of the destination dirs)"
    sys.exit(0)

# Go through dest list & robocopy from source to dest
for dest in destinations:
    cmdLine = ROBOCOPY % (srcDir, dest)
    os.system(cmdLine)