#! /usr/bin/env python

# To Sync a bunch of files using Bash:
# find -L . -name RunTests\.cs -execdir cp /cygdrive/e/Work/Student_Work/BIT_143_New/PCE03/PCE_03_VS_2010_NUnit_2_5/01_PCE_Test_Runner/RunTests.cs '{}' \;

import os
import shutil
import sys

destinations = [
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE03\\PCE_03_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE04\\PCE_04_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE05\\PCE_05_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE06\\PCE_06_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE07\\PCE_07_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE08\\PCE_08_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE09\\PCE_09_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_143_New\\PCE10\\PCE_10_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_142_New\\PCE08\\PCE_08_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                'E:\\Work\\Student_Work\\BIT_142_New\\PCE09\\PCE_09_VS_2010_NUnit_2_5\\01_PCE_Test_Runner\\AutoGrader\\',
                ]
filesToCopy = [
               'All_Grades.css',
               'AutoGrader.xslt',
               'config.py',
               'Display.xslt',
               'gradeAllInPCE.py',
               'Grades.css.xml',
               'Mono.py',
               'PCEs_gradeAll.py',
               'SyncAll.py',
               'TextToReplace.xslt'
               ]

# filesToExclude = [ # files to NOT copy:
# 'additionalFiles.py',
# 'GradingInfo.xslt',
# ]

def abspathAll(listOfPaths):
    newListOfPaths = []
    for path in listOfPaths: # "slice copy" of listOfPaths
        newListOfPaths.append( os.path.abspath(path) )
    
    return newListOfPaths

destinations = abspathAll(destinations)
pathfilesToCopy = abspathAll(filesToCopy)

cwd = os.path.abspath(os.getcwd()).lower()

for dest in destinations:
    dest = dest.lower()

    if cwd == dest:
        print 'X   %s\n\t(NOT copying to the current directory)\n' % dest
        continue
        
    print '==> %s' % (dest)
    szOut = "\t"
    for nextFile in pathfilesToCopy:
        szOut += filesToCopy[pathfilesToCopy.index(nextFile)] + " "
        if len(szOut) > 70:
            print szOut
            szOut = "\t"
        shutil.copy2(nextFile, dest)
    print szOut