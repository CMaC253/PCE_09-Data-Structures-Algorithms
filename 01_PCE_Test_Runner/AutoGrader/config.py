import re
import os.path

RUNNING_ON_WINDOWS = True
PREFIX = ""

def FixPath(thePath):
	if RUNNING_ON_WINDOWS:
		thePath = re.sub("/cygdrive/(.)/", r"\1:\\", thePath)
		return os.path.normpath(thePath)
	else:
		return thePath

# Please just pretend like this hack doesn't exist :)
def Windowsify(hack):
	# print "\n\nSTART: \n" + repr(hack) + "\n" + hack
	hack = re.sub("/cygdrive/(.)/", r"\1:\\", hack)
	# print "1\n" + repr(hack)
	hack = os.path.normpath(hack)
	# print "2\n" + repr(hack) + "\n" + hack
	# print "3\n" + hack
	hack = re.sub('\\\\target:', "/target:", hack)
	hack = re.sub('\\\/target:', "/target:", hack)
	hack = re.sub('\\\\out:', r"/out:", hack)
	hack = re.sub('\\\/out:', r"/out:", hack)
	hack = re.sub('\\\\r:', "/r:", hack)
	hack = re.sub('\\\/r:', "/r:", hack)
	# print "4\n" + repr(hack)
	return hack

# def Windowsify(hack):
	# # print "\n\n1\n" + hack
	# #hack = re.sub("/cygdrive/(.)/", r"\1:\\\\", hack)
	# hack = re.sub("/cygdrive/(.)/", r"\1:\\", hack)
	# hack = re.sub(":\\", r"\1:\\", hack)
	# # print "2\n" + hack
	# #hack = re.sub('/', "\\\\\\\\", hack)
	# hack = re.sub('/', "\\\\", hack)
	# # print "3\n" + hack
	# hack = re.sub('\\\\target:', "/target:", hack)
	# hack = re.sub('\\\/target:', "/target:", hack)
	# hack = re.sub('\\\\out:', r"/out:", hack)
	# hack = re.sub('\\\/out:', r"/out:", hack)
	# hack = re.sub('\\\\r:', "/r:", hack)
	# hack = re.sub('\\\/r:', "/r:", hack)
	# # print "4\n" + hack
	# return hack
