<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- probably ought to put these in a common include file -->

	<xsl:variable name="Unassigned_Category">No Category Specified</xsl:variable>
	<xsl:variable name="Missing_Category">No Category Found</xsl:variable>
	<xsl:output method="html" version="1.0" encoding="UTF-8" indent="yes"/>
	<!-- indent="yes": may not be safe with doc types with "mixed content"?-->
	<xsl:template match="/">
		<html>
			<head>
				<!-- This was used when the CSS sheet was separate:
				<link href="Grades.css" rel="stylesheet" type="text/css" /> -->

				<style type="text/css">
					<xsl:copy-of select="document('Grades.css.xml')/all/text()"/>
				</style>
			</head>
			<body>
				<div class="PageTitle">					Grade Results for 
					<xsl:value-of select="//StudentTrackerGradeDigest/@ExerciseName"/>
				</div>
				<div class="TimeStamp">					(This test was run at 
					<xsl:value-of select="//StudentTrackerGradeDigest/@TestDateTime"/>
				</div>
				<div class="Overall_Feedback">
					<xsl:copy-of select="//StudentTrackerGradeDigest/OverallFeedback"/>
				</div>
				<table class="ST_Table" width="100%" border="1">
					<xsl:for-each select="//Exercises/Exercise">
						<tr>
							<td colspan="2">
								<div class="ExerciseTitle">
									<xsl:value-of select="@name"/>
								</div>
							</td>
						</tr>
						<xsl:apply-templates select="FailedTests"/>
						<xsl:apply-templates select="PassedTests"/>
					</xsl:for-each>
				</table>
				<p>
					<font size="+2">
						<b>			  Raw Grade:
							<xsl:value-of select="//StudentTrackerGradeDigest/@RawGrade"/>
						</b>
					</font>
				</p>
				<font size="+2">+2 bonus (if you get 6 points or higher)</font>
				<div class="OverallGrade">                    Overall Grade:
					<xsl:value-of select="//StudentTrackerGradeDigest/@OverallGrade"/>
				</div>
				<div class="MaxGrade">                    (Possible grades range from 
					<xsl:value-of select="//StudentTrackerGradeDigest/@MinGrade"/>
					<xsl:value-of select="//StudentTrackerGradeDigest/@MaxGrade"/>
				</div>
				<div class="Disclaimer">
					<b>
						<u>Note:</u>
					</b>
					<xsl:value-of select="//StudentTrackerGradeDigest/Disclaimer"/>
				</div>
			</body>
		</html>
	</xsl:template>
	<!-- NOTE: The following element types are ignored:
        SetGrade (used by ConsoleRunner, but not this transform)
        -->
	<xsl:template match="FailedTests">
		<tr>
			<td>
				<div class="FailedTests">Failed Tests</div>
			</td>
			<td class="FT_PointPenalty">Point Penalty</td>
		</tr>
		<!-- if there are no failed tests -->
		<xsl:choose>
			<xsl:when test="node()">
				<xsl:apply-templates select="ModGrade"/>
			</xsl:when>
			<xsl:otherwise>
				<tr>
					<td colspan="2"> There are no failed tests</td>
				</tr>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="ModGrade">
		<!-- Yeah, this is a hack... -->

		<xsl:choose>
			<xsl:when test="@GradingCategory=$Unassigned_Category">
				<tr class="UnassignedCategory">
					<td class="FT_GradingCategory">                        Grading Category: 
						<i>
							<xsl:value-of select="@GradingCategory"/>
						</i>
					</td>
					<td class="FT_PointPenalty">
						<xsl:value-of select="@value"/>
					</td>
				</tr>
			</xsl:when>
			<xsl:when test="@GradingCategory=$Missing_Category">
				<tr class="MissingCategory">
					<td class="FT_GradingCategory">                        Grading Category: 
						<i>
							<xsl:value-of select="@GradingCategory"/>
						</i>
					</td>
					<td class="FT_PointPenalty">
						<xsl:value-of select="@value"/>
					</td>
				</tr>
			</xsl:when>
			<xsl:otherwise>
				<tr>
					<td class="FT_GradingCategory">                        Grading Category: 
						<i>
							<xsl:value-of select="@GradingCategory"/>
						</i>
					</td>
					<td class="FT_PointPenalty">
						<xsl:value-of select="@value"/>
					</td>
				</tr>
			</xsl:otherwise>
		</xsl:choose>
		<tr>
			<td class="FT_Explanation" colspan="2">
				<xsl:value-of select="@explanation"/>
			</td>
		</tr>
		<xsl:if test="Tests=node()">
			<!-- only include this if there are specific tests listed
                                        (otherwise, the explanation is assumed to list the failure) -->

			<tr>
				<td colspan="2" style="border-top-width:0px">                    This penalty was triggered by the failure of the following tests:
					<table class="FT_ListOfTests">
						<tr>
							<td/>
						</tr>
						<xsl:apply-templates select="Tests"/>
					</table>
				</td>
			</tr>
		</xsl:if>
	</xsl:template>
	<xsl:template match="Test">
		<tr>
			<td class="FT_TestName">
				<xsl:value-of select="@testname"/>
			</td>
		</tr>
		<tr>
			<td class="FT_TestExplanation">
				<xsl:value-of select="@explanation"/>
				<br/>
				<b>Error Message From The Test Itself:</b>
				<br/>
				<pre>
					<xsl:value-of select="Test_Messaage"/>
				</pre>
				<br/>
				<b>Stack Trace From The Test Itself:</b>
				<br/>
				<pre>
					<xsl:value-of select="Test_StackTrace"/>
				</pre>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="PassedTests">
		<tr>
			<td colspan="2">
				<div class="PassedTests">Passed Tests</div>
			</td>
		</tr>
		<xsl:choose>
			<xsl:when test="node()">
				<xsl:apply-templates select="Comment_PassedTest"/>
			</xsl:when>
			<xsl:otherwise>
				<tr>
					<td colspan="2"> There are no passed tests</td>
				</tr>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template match="Comment_PassedTest">
		<tr>
			<td colspan="2">				Name Of Passed Test: 
				<i>
					<xsl:value-of select="@testname"/>
				</i>
			</td>
		</tr>
	</xsl:template>
</xsl:stylesheet>
