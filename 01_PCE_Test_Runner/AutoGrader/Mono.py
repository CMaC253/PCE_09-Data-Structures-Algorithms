#! /usr/bin/env python
#
# This script should be be run from the {root}/01_PCE_Test_Runner/Autograder directory
#
#
#
#
#
# NOTE NOTE:
# This file will build your code, the test code, and the test runner.
# It assumes that your PCE_Test_Runner/RunTests.cs is set up to do whatever you want,
#  so if you want to run this in the GUI, you'll need to adjust RunTests.cs FIRST

# WARNING:
# This has been tested under CygWin (1.5.x), and *should* work.  No guarantees, though -
# talk to the instructor if you're interested in using this, particularly if you run into problems

# EXTRA WARNING:
# This hasn't actually been tested in a while (April 14, 2009).  User beware :)

# To use this on CygWin, you may need to run the dos2unix command to convert the end-of-lines
# to unix line endings.  If, when you run the script, you get errors about \r, try dos2unix...

import sys
import os.path
import re
import shutil
import subprocess
from config import *

# modularize an extra files:
from additionalFiles import AddFiles_ToTestDLL

AdditionalFiles_Test = list()
AddFiles_ToTestDLL(AdditionalFiles_Test)


# If you want to use Mono, change out the below line...
COMPILER = "/cygdrive/c/Windows/Microsoft.NET/Framework/v3.5/csc.exe"

#COMPILER = r"C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe"
#  Mono: you'll want to turn this off
USING_CSC = True
# Also - Mono has command line switches prefaced by -, MS's CSC prefaces them with /
#	(You'll need to fix this yourself, below)


STUDENT_OUTPUT = "03_PCE_StudentCode.exe"
TEST_OUTPUT = "PCE_Test.dll"
RUNNER_OUTPUT = "PCE_Test_Runner.exe"


COMPILER_FAIL = 0
COMPILER_OK = 1
def RunCompiler(COMPILER_ARGS, whatIsThis, COMPILER_ERROR_FILE):
	global COMPILER
# *Sigh*  I'm running this under cygwin, 
# but need to mash the paths to make this work with the csc
# command-line parameters (/out looks like a dir on *nix, 
# and /cygwin/e/etc looks like a param to CSC.

	if USING_CSC:
		COMPILER_ARGS = Windowsify(COMPILER_ARGS)

	# print "COMPILER: " + COMPILER

	if RUNNING_ON_WINDOWS:
		COMPILER = Windowsify(COMPILER)

	# print "COMPILER: " + COMPILER
	
	CMD_LINE = COMPILER + " " + COMPILER_ARGS

	# print "Command to compile "+whatIsThis+"\n" + CMD_LINE
	ExitCode = os.system(CMD_LINE)
	if(ExitCode != 0):
		errMsg = "ERROR: Can't compile " + whatIsThis + " code"
		print errMsg
		print "Compiler output:\n\n"
		os.system("cat " + COMPILER_ERROR_FILE);
		print "\n\n"
		raise Exception(errMsg)
		
	print PREFIX + whatIsThis + " code compiled successfully"
	return COMPILER_OK

def main_loader(dirSrc, no_browser, mode_gradesheet, prefix):
	global PREFIX
	PREFIX = prefix
	return main(dirSrc, no_browser, mode_gradesheet)


def main(dirSrc, no_browser, mode_gradesheet):
	# print "Building from %s\n\tThis is assumed to be the \"Autograder\" directory in the starter project\n" % (dirSrc)

	if not os.path.exists(dirSrc):
		print "The source directory " + sys.argv[1] + "(" + dirSrc + ") does not exist!"
		sys.exit(3)

	SOLUTION_PREFIX = os.path.normpath(dirSrc + "../../../") + "/"
	
	COMPILER_ERROR_FILE = SOLUTION_PREFIX + "Err.txt"

	OUTPUT_DIR = SOLUTION_PREFIX + "01_PCE_Test_Runner/bin/Debug/"

	STUDENT_PREFIX = SOLUTION_PREFIX + "03_PCE_StudentCode/"
	CS_FILES_STUDENT = STUDENT_PREFIX + "Student_Answers.cs " + STUDENT_PREFIX + "Properties/AssemblyInfo.cs"


	TEST_PREFIX = SOLUTION_PREFIX + "02_PCE_ForTests/"
	CS_FILES_TEST = TEST_PREFIX + "PCE_Tests_To_Run.cs " + TEST_PREFIX + "TestHelpers.cs " + TEST_PREFIX + "Properties/AssemblyInfo.cs"

	# add in any additional files:
	for test_file_cs in AdditionalFiles_Test:
		CS_FILES_TEST = CS_FILES_TEST + " " + TEST_PREFIX + test_file_cs

#	print "CS_FILES_TEST: " + CS_FILES_TEST
#	sys.exit(0)

	RUNNER_PREFIX = SOLUTION_PREFIX + "01_PCE_Test_Runner/"
	CS_FILES_RUNNER = RUNNER_PREFIX + "RunTests.cs " + RUNNER_PREFIX + "Properties/AssemblyInfo.cs" 

	REF_PREFIX = SOLUTION_PREFIX + "net-2.0/"
	REFERENCES = [REF_PREFIX + "lib/nunit.core.dll", REF_PREFIX + "framework/nunit.framework.dll", REF_PREFIX + "lib/nunit-console-runner.dll", REF_PREFIX + "lib/nunit.util.dll", REF_PREFIX + "lib/nunit.core.interfaces.dll"]

## The PCE_Test.dll needs to know about (have a ref to) student classes, but nothing else
## does, so we'll use this only for building the test .DLL
	TEST_REFERENCES = REFERENCES
	TEST_REFERENCES.append(OUTPUT_DIR + STUDENT_OUTPUT)

#	REQUIRED_LIBS = REF_PREFIX + "nunit.core.dll," + REF_PREFIX + "nunit.core.interfaces.dll," + REF_PREFIX + "nunit.framework.dll" + REF_PREFIX + "nunit.util.dll" + REF_PREFIX + "nunit-console-runner.dll"


	## Don't clutter up the main directory - use a subdir, instead
	if not os.path.exists(OUTPUT_DIR):
		os.makedirs(OUTPUT_DIR)

	startingDir = os.getcwd() #pushd would be great...
	os.chdir(OUTPUT_DIR)

	try:
	## Compile student code
	## Student code should not have any NUnit tests, so we don't need the NUnit references...
		COMPILER_ARGS = " /target:library /out:" + STUDENT_OUTPUT + " " + CS_FILES_STUDENT + ">" + COMPILER_ERROR_FILE
		RunCompiler(COMPILER_ARGS, "Student", COMPILER_ERROR_FILE);
	
	## Compile test code
		TR_TEMP = ""
		for ref in TEST_REFERENCES:
			TR_TEMP = TR_TEMP + "/r:" + ref + " "
		TEST_REFERENCES = TR_TEMP
	
		COMPILER_ARGS = " /target:library /out:" + TEST_OUTPUT + " " + TEST_REFERENCES + " " + CS_FILES_TEST + ">" + COMPILER_ERROR_FILE
		RunCompiler(COMPILER_ARGS, "Test DLL", COMPILER_ERROR_FILE);
	
	## Mostly copy-and-pasted from above
	## Compile the command-line runner / XML massager
		TR_TEMP = ""
		for ref in REFERENCES:
			TR_TEMP = TR_TEMP + "/r:" + ref + " "
		RUNNER_REFERENCES = TR_TEMP
	
		COMPILER_ARGS = " /target:exe /out:" + RUNNER_OUTPUT + " " + RUNNER_REFERENCES + " " + CS_FILES_RUNNER + ">" + COMPILER_ERROR_FILE
		RunCompiler(COMPILER_ARGS, "Test RUNNER", COMPILER_ERROR_FILE);
	
	
	## Make sure that all required DLLs are present:
		for dll in REFERENCES:
			dst = OUTPUT_DIR + os.path.basename(dll)
			if not os.path.exists(dst):
				shutil.copyfile(dll, dst)
	
	## Finally, run the tests:
		CMD_LINE = OUTPUT_DIR + RUNNER_OUTPUT;
		if no_browser:
		    CMD_LINE = CMD_LINE + " NO_BROWSER "
		if mode_gradesheet:
		    CMD_LINE = CMD_LINE + " MODE_GRADESHEET "
		CMD_LINE = CMD_LINE + "  " + OUTPUT_DIR + TEST_OUTPUT + " > " + COMPILER_ERROR_FILE 
		# print CMD_LINE
	
	#one last hack before we're done: make sure we can access everything, even under Vista
		
		os.system("chmod 0777 *");
	#	StudentGrade = os.system( CMD_LINE )
	
		try:
		    StudentGrade = subprocess.call(CMD_LINE, shell=True)
		    #	print "Return value: " + str(StudentGrade)
		except OSError, e:
		    print "Execution of actual test failed:", e
	
	finally:
		os.chdir(startingDir) #pushd (other half)
	
	return StudentGrade
## end of main

if __name__ == "__main__":
	no_browser = False;
	mode_gradesheet = False
	
	if len(sys.argv) > 3:
		print "This script is meant to be run from your \"Autograder\" directory"
		print "\t Optional Parameters:"
		print "\tNO_BROWSER\n\t\tPass NO_BROWSER argument to test runner"
		print "\t\tThis will stop the runner from displaying the autograded results in a browser"
		print "\tMODE_GRADESHEET\n\t\tPass MODE_GRADESHEET argument to test runner"
		print "\t\tThis will force the autograder to produce a gradesheet (and NOT a GUI)"
		sys.exit(200) # fail!
		
	for arg in sys.argv:
		if arg == "NO_BROWSER":
			print "NO_BROWSER: We will NOT display output in web browser at finish"
			no_browser = True
    	if arg == "MODE_GRADESHEET":
    		print "MODE_GRADESHEET: We will force the test to run in 'gradesheet' mode"
    		mode_gradesheet = True
    
	main(os.getcwd(), no_browser, mode_gradesheet)
