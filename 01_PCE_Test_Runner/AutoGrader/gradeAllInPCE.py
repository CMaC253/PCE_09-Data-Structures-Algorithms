#! /usr/bin/env python
#
# Usage: <script> src dst
#   src may be ., or any directory that ends with a /
#   dst must end with a /
#
#    Assumption: the script is running from an 'Autograder' directory with an
#                NUnit based autograder directory.  I.e., in 
#                .../PCE_xx_NUnit/01_PCE_Test_Runner/Autograder/ <RUN HERE>
#
#    src is assumed to contain:
#        1) a PCE_XX_NUnit directory, which contains the pristine/new tests
#        2) all the students' work, as immediate subdirectories 
#
#    dst is where all the gradesheets, plus the meta-results are put
#
import sys
import re
import shutil
import os.path
import fileinput
import zipfile
import string

G_PathForSourceCode = os.path.abspath( "../../03_PCE_StudentCode/Student_Answers.cs" ); # relative to cwd
G_PathToGrade=os.path.abspath( "../../Grades.html" )
G_MonoScript = "Mono"
G_Student_Code_Pattern="Student_Answers.cs"

Excluded_Dir_List = [ "PCE_.." ]
PATTERN_Student_Name_From_Dir = "(?P<studentName>.*?,.*?),.*"; 

DIRECTORY_LIMIT = 45

class Handler_Base:
    matched = False # has this handler matched a rule yet?
    processed = False # so we can use the same handler object for multiple patterns

    def Reset(self):
        self.matched = False 
        self.processed = False

    def Handle_Match(self, dirDest, studentName, fname, data, prefix):
        pass
    
    def PostStudent(self, dirDest, studentName, errMsgPrefix):
        pass
    
    def PostStudentHelper(self, fileName, fileContents, errorMsg ):
        if self.matched == False and self.processed == False:
            self.processed = True
            print errorMsg
            fileName =  self.UniqueFileName(fileName)
                #exception on fail
            self.WriteFile(fileName, fileContents)

    # to be called after all students have been processed
    def PostProcess(self, dirDest):
        pass

    #append _1, _2, etc, onto the filename
    def UniqueFileName(self, fileNameOrig):
        ctr = 0
        fileExtList = os.path.splitext(fileNameOrig)
        filename = fileNameOrig 
        while os.path.exists(filename):
            ctr = ctr + 1
            filename = fileExtList[0] + "_" + str(ctr) + fileExtList[1]
            if ctr > 10:
                raise Exception( "Couldn't create nextFile " + filename + " because this script can only unique-ify a nextFile name 10 times" )
        return filename
    
    # convenience function to write out a nextFile
    def WriteFile(self, filename, data):
        fout = open(filename, "wb")
        fout.write(data)
        fout.close()
# Handler_Base

# This will take a given .ZIP nextFile, walk through it, 
# and recursively ask the Handler_Manager to optionally process each nextFile
# in the .ZIP

## Unfinished; I was hoping to allow the ZIP_Handler to recursively process
## nested .ZIP files by passing in a ZipExtFile (obtained from zipefile.open)
## as the name, but Python doesn't support the 'string or nextFile-like object' thing
## until Python 2.7 (which isn't supported under CygWin)
#class ZIP_Handler (Handler_Base):
#    handlerManager = None
#    def __init__(self, hM):
#        self.handlerManager = hM
#
#
#    def Handle_Match(self, dirDest, studentName, fname, data, prefix):
#        print "%sZIP_HANDLER!============== %s" % (prefix, fname)
#    
#        # if not os.path.isfile(fname) or not fname.endswith(".zip"):
#            # print "%s isn't a nextFile (or doesn't end in .ZIP)" % fname
#            # return
#
#        #if not zipfile.is_zipfile(fname):
#        #    print "Has %s ending, but is not a PKZIP nextFile: %s" % (os.path.splitext(fname)[1], fname)
#        #    return
#
#        try:
#            # open the zipped nextFile
#            print "%sopen zipfile" % (prefix)
#            zfile = zipfile.ZipFile(fname, "r")
#            
#            # get each archived nextFile and process the decompressed data
#            print "%sAbout to iterate through zipfile" % (prefix)
#            for info in zfile.infolist():
#                zippedfilename = info.filename
#                
#                matchingHandlers = self.handlerManager.fileMatches( zippedfilename )
#                
#                if len(matchingHandlers) > 0:
#                    # decompress each nextFile's data
#                    print "%sabout to read %s" % (prefix, zippedfilename)
#                    data = zfile.read(zippedfilename)
#                    
#                    # print "%sabout to open %s" % (prefix, zippedfilename)
#                    fileLikeObject = zfile.open(zippedfilename)
#                    
#                    # print "%sopened nextFile, about to ask is_zipfile" % (prefix)
#                    #cygwin 1.7 only supports Python 2.6.5
#                    # is_zipfile only works on nextFile-like objects in version 2.7+ :(
#                    # if zipfile.is_zipfile( fileLikeObject ) :
#                    if fileLikeObject.endswith(".zip"):
#                        zippedFile = fileLikeObject
#                        print "found a nested zipfile - recursively processing"
#                        # continue
#                    else:
#                        zippedFile = zippedfilename
#                        print prefix + "it was NOT a nested zipfile, so we'll extract & process it directly"
#                    
#                    self.handlerManager.checkForMatches(matchingHandlers, dirDest, studentName, zippedFile, data, prefix )
#        except zipfile.BadZipfile:
#            print "BAD ZIP FILE: the zip nextFile for %s (named %s) is bad" % (studentName, fname)
#        except zipfile.LargeZipFile:
#            print "TOO-LARGE ZIP FILE: the zip nextFile for %s (named %s) is too large to be opened" % (studentName, fname)
#        except:
#            print "Exception was caught, trying to open (or process) zip nextFile of %s (nextFile name: %s)" % (studentName, fname)
## ZIP_Handler    

 # ZIP_Handler BACKUP
class ZIP_Handler (Handler_Base):
     handlerManager = None
     def __init__(self, hM):
         self.handlerManager = hM


     def Handle_Match(self, dirDest, studentName, fname, data, prefix):
     #    print "Found a matching nextFile, named %s" % fname
     #    print "\tdest: %s\n\tstudent: %s" % (dirDest, studentName)
     #    print "found .doc/docx/pdf nextFile: %s:" % fname
                     # trim off anything except the filename
 #        fname = re.sub(".*/", "", fname)
    
         print "%sZIP_HANDLER!============== %s" % (prefix, fname)
    
         # if not os.path.isfile(fname) or not fname.endswith(".zip"):
             # print "%s isn't a nextFile (or doesn't end in .ZIP)" % fname
             # return

         # if not zipfile.is_zipfile(fname):
             # print "Has .zip ending, but is not a PKZIP nextFile: %s" % fname
             # return

         try:
             # open the zipped nextFile
             # print "%sopen zipfile" % (prefix)
             zfile = zipfile.ZipFile(fname, "r")
            
             # print "Opened .ZIP nextFile"
            
             # get each archived nextFile and process the decompressed data
             # print "%sAbout to iterate through zipfile" % (prefix)
             for info in zfile.infolist():
                 zippedfilename = info.filename
                
                 matchingHandlers = self.handlerManager.fileMatches( zippedfilename )
                
                 if len(matchingHandlers) > 0:
                     # decompress each nextFile's data
                     # print "%sabout to read %s" % (prefix, zippedfilename)
                     data = zfile.read(zippedfilename)
                    
                     # print "%sabout to open %s" % (prefix, zippedfilename)
                     # fileLikeObject = zfile.open(zippedfilename)
                    
                     # print "%sopened nextFile, about to ask is_zipfile" % (prefix)
                     # if zipfile.is_zipfile( fileLikeObject ) : #cygwin 1.7 only supports Python 2.6.5
                     # is_zipfile only works on nextFile-like objects in version 2.7+ :(
                     if re.search(".zip", zippedfilename):
                         # zippedFile = fileLikeObject
                         print "<<< ERROR found a nested zipfile - this is not supported (until CygWin supports Python 2.7) >>>"
                         continue
                     else:
                         zippedFile = zippedfilename
                         print prefix + "it was NOT a nested zipfile, so we'll extract & process it directly"
                    
                     self.handlerManager.checkForMatches(matchingHandlers, dirDest, studentName, zippedFile, data, prefix)
         except zipfile.BadZipfile:
             print "BAD ZIP FILE: the zip nextFile for %s (named %s) is bad" % (studentName, fname)
         except zipfile.LargeZipFile:
             print "TOO-LARGE ZIP FILE: the zip nextFile for %s (named %s) is too large to be opened" % (studentName, fname)
         except:
             print "Exception was caught, trying to open (or process) zip nextFile of %s (nextFile name: %s)" % (studentName, fname)
 # ZIP_Handler BACKUP

# This just grabs and doc/docx/pdf type file, and 
# saves it to the output directory

class Feedback_Handler (Handler_Base):
    def Handle_Match(self, dirDest, studentName, fname, data, prefix):
    #    print "Found a matching nextFile, named %s" % fname
    #    print "\tdest: %s\n\tstudent: %s" % (dirDest, studentName)
    #    print "found .doc/docx/pdf nextFile: %s:" % fname
    
        # trim off anything except the filename
        fname = re.sub(".*/", "", fname)
    
        # save the decompressed data to a new nextFile
        fileName = dirDest + studentName + "_" + fname
        fileName = self.UniqueFileName(fileName) #exception on fail

        self.WriteFile(fileName, data)
        print "%sFEEDBACK_HANDLER!============== %s (creating %s)" % (prefix, fname, fileName)
        # print prefix + "New nextFile created --> %s" % fileName

    def PostStudent(self, dirDest, studentName, errMsgPrefix):
        fileName = dirDest + studentName + " FEEDBACK Missing.txt"
        fileContents = "No feedback for " + studentName
        errMsg = errMsgPrefix + "Did NOT find feedback for " + studentName
        
        self.PostStudentHelper(fileName, fileContents, errMsg)
#Feedback_Handler    

# This grabs the Student_Answers.cs nextFile, and tests it,
# saving the results to the output directory
class StudentCodeToTest_Handler (Handler_Base):
    targetPath = ""
    accumulatedGrades = list()
    
    def __init__(self, path):
        self.targetPath = path
        
    def Handle_Match(self, dirDest, studentName, fname, data, prefix):
        global PREFIX
    
        print "%sSTUDENTCODETOTEST_HANDLER!============== %s" % (prefix, fname)
        # print prefix + "Found a matching CODE nextFile, named %s" % fname
        # print prefix + "\tdest: %s" % dirDest
        # print prefix + "\tstudent: %s" % studentName
    
        #overwrite the target nextFile:
        # print "target: " + self.targetPath
        self.WriteFile(self.targetPath, data)

        # call Mono.py to generate results

        Builder = __import__(G_MonoScript)
        #NOP 
        try:
#            print "about to call mono.py"
            studentGrade = Builder.main_loader( os.getcwd(), True, True, prefix + '\t') #no browser pop-up
#            print "finished call to mono.py"
        except Exception, e:
            print "Problem with code from %s  %s" % (studentName, e)
            raise #re-throw
        
        # get .html file, move to output dir (with renaming)    
        finalGrades = self.UniqueFileName( os.path.abspath(dirDest + studentName + ", Grades.html") )
        print "\n\t" + studentName +" got " + str(studentGrade)
        
        shutil.move(G_PathToGrade, finalGrades )

        self.accumulatedGrades.append( (studentName, studentGrade)  )

    def PostStudent(self, dirDest, studentName, errMsgPrefix):
        if self.matched == False and self.processed == False:
            self.accumulatedGrades.append( (studentName, "NO WORKING SOURCE CODE FOUND")  )
            
            fileName = dirDest + studentName + " SOURCE CODE Missing.txt"
            fileContents = "No source code for " + studentName
            errMsg = errMsgPrefix + "Did NOT find working source code for " + studentName
            
            # PostStudentHelper already NOPs when matched == False && processed == False...
            self.PostStudentHelper(fileName, fileContents, errMsg)

    def PostProcess(self, dirDest):
        # HTML doc with everyones' grades
        tabulatedResults = "<html><head>\n<title>Results</title>\n"
        
        tabulatedResults += "<style>\n"
        for line in fileinput.input( os.path.abspath("All_Grades.css") ):
            tabulatedResults += line + "\n"
        tabulatedResults += "</style>\n"
        
        tabulatedResults += "</head><body>\n" 
        tabulatedResults += "<table width=\"100%\" class=\"Results_Table\">\n"
        tabulatedResults += "<tr class=\"Header_Row\"><td width=\"20%\" class=\"Grades_Col\">Name</td><td>Grade</td></tr>\n"
        
        i = 0
        for student in self.accumulatedGrades:
            name = student[0]
            nameLinked = "<a href=\""+ student[0]+", Grades.html\">" + student[0] + "</a>";
            grade = str(student[1])
            print "%30s  got  %s" % ( name, str(grade) )

            if i % 2 == 0:
                tabulatedResults += "<tr class=\"Even_Row\"><td>"+nameLinked+"</td><td>"+grade+"</td></tr>\n"
            else:
                tabulatedResults += "<tr class=\"Odd_Row\"><td>"+nameLinked+"</td><td>"+grade+"</td></tr>\n"
            i = i + 1
            
        tabulatedResults += "</table>\n"
        tabulatedResults += "</body></html>"
        self.WriteFile( os.path.abspath(dirDest + "ALL_GRADES.html"), tabulatedResults )
            

class Handler_Manager:
    def __init__(self):
        fbH = Feedback_Handler()
        scH = StudentCodeToTest_Handler(G_PathForSourceCode)
        zH = ZIP_Handler( self )
        # print scH.targetPath
        self.FT_Handlers = [
                            (".doc$", fbH),
                            (".docx$", fbH),
                            (".pdf$", fbH),
                            (G_Student_Code_Pattern, scH),
                            (".zip", zH)
                            ]
    def Reset(self):
        for pair in self.FT_Handlers:
            pair[1].Reset()
    
    def fileMatches(self, fname):
        # print "fileMatches: starting comparison for %s" % fname
        matches = list()
        for patHandler in self.FT_Handlers:
            if re.search(patHandler[0], fname):
                # print "fileMatches: matched %s to nextFile %s" % (patHandler[0], fname)
                matches.append(patHandler)
        
        return matches

    def checkForMatches(self, matchingHandlers, dirDest, studentName, fname, data, prefix):
        # assumption is that matchingHandlers = fileMatches(self, fname)
        
        for patHandler in matchingHandlers:
            try: 
                patHandler[1].Handle_Match( dirDest, studentName, fname, data, prefix + "\t")
                patHandler[1].matched = True # note: NOT set on exception
                        # in Handle_Match
                        
            except Exception, ex:
                print ex
                # something went wrong - ignore this otherwise

        # for patHandler in self.FT_Handlers:
            # if re.search(patHandler[0], fname):
                # try: 
                    # patHandler[1].Handle_Match( dirDest, studentName, fname, data, "\t")
                    # patHandler[1].matched = True # note: NOT set on exception
                            # # in Handle_Match
                # except Exception, ex:
                    # print ex
                    # # something went wrong - ignore this otherwise
                    
    def DoPostStudent(self, dirDest, studentName):
        for handler in self.FT_Handlers:
            handler[1].PostStudent(dirDest, studentName, "\t")

    def DoPostProcess(self, dirDest):
        for handler in self.FT_Handlers:
            handler[1].PostProcess(dirDest)

def isUnsupportedArchive( archiveName):
    return archiveName.endswith('rar') or archiveName.endswith('7z')

def main(dirSrc, dirDest):
    # print "Looking for feedback files in %s\noutputting to %s\n" % (dirSrc, dirDest)

    if not os.path.exists(dirSrc):
        print "The source directory " + sys.argv[1] + "(" + dirSrc + ") does not exist!"
        sys.exit(3)

    if not os.path.isdir(dirDest):
        os.makedirs(dirDest) #throws error on fail

    FTHandlers = Handler_Manager()

    i = 0
    for dirName in os.listdir(dirSrc):
        # print '=== NEXT DIR: ' + dirName
        if isUnsupportedArchive( dirName):
            print '<<< Found an unsupported archive (%s) >>>' % dirName
            continue
        
        i = i + 1
        if i > DIRECTORY_LIMIT:
            print "Directory limit (%s) reached, terminating..." % DIRECTORY_LIMIT
            break

        continueOuterLoop = True;
        for excludedDir in Excluded_Dir_List:
#            print "next excl: %s" % excludedDir
            if re.search(excludedDir, dirName):
                print "<<< Excluding directory %s, because it matched %s >>>" % (dirName, excludedDir)
                continueOuterLoop = False
                break

        if continueOuterLoop == False: #Python-standard (?) hack?
            continue

        studentName = re.sub(PATTERN_Student_Name_From_Dir, "\g<studentName>", dirName);
        dirTarget= os.path.join( dirSrc, dirName)

        FTHandlers.Reset() #reset all handlers for this student
        
        for dirpath, dirnames, filenames in os.walk(dirTarget):
            print "Walking through %s" % dirpath

            for fileName in filenames:

                fileFP = os.path.join(dirpath, fileName)
                if fileFP.endswith(".rar") or fileFP.endswith(".7z"):
                    print "<<< Error: Found .rar/.7z nextFile: %s >>>" % fileFP
                    continue

                # Try to handle the raw nextFile directly
                matchingHandlers = FTHandlers.fileMatches( fileFP )
    
                if len(matchingHandlers) > 0:
                    # decompress each nextFile's data
                    nextFile = open(fileFP)
                    data = nextFile.read() # read the entire nextFile
    
                    FTHandlers.checkForMatches(matchingHandlers, dirDest, studentName, fileFP, data, "")
                    
        FTHandlers.DoPostStudent(dirDest, studentName)
        print '-' * 80
    
    print "=== Finished processing all students"
    FTHandlers.DoPostProcess(dirDest)
        

if __name__ == "__main__":
    if len(sys.argv) != 3:
        scriptName = sys.argv[0]
        scriptName = re.sub(".*/", "", scriptName)
        print "Usage: " + scriptName + " SRC DST"
        print "\tAssumption: the script is running from an 'Autograder' directory with an"
        print "\tNUnit based autograder directory.  I.e., in"
        print "\t.../PCE_xx_NUnit/01_PCE_Test_Runner/Autograder/ <RUN HERE>"
        sys.exit(1) # fail!

    dirSrc = os.path.abspath(sys.argv[1]) + os.sep
    dirDest = os.path.abspath(sys.argv[2]) + os.sep
    
    # Don't process the output directory, even if it's in the same place as 
    # all the PCEs
    outputDir = os.path.basename( dirDest[0:len(dirDest)-1] )
    Excluded_Dir_List.append(  outputDir  )
    
    print "Excluded directories: %s" % Excluded_Dir_List
    
    main(dirSrc, dirDest)
