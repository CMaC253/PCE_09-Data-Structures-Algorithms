<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

    <!-- This file is responsible for taking the output of the NUnit console runner, and transforming it
            into an .XML file that the Display.xslt file will then use to render output .
         Essentially, this file serves as an adapter, so that the Display logic doesn't have to
            worry about the details of NUnit output -->


    <!-- This will pull in the grading rules that are specific to the PCEs-->
    <xsl:import href="GradingInfo.xslt"/>
	<xsl:import href="TextToReplace.xslt"/>

    <!--===================  VARIABLE DECLARATIONS =========================-->

    <!-- If nothing else matches, a test that fails will be put into it's own category & 
        given this point penalty: -->
    <xsl:variable name="Default_Test_Failure_Point_Penalty">-0.5</xsl:variable>

    <!-- Category name for when a test's failure gets the default test failure point penalty -->
    <xsl:variable name="Default_Category">Default</xsl:variable>

    <!-- Category name for tests that should NOT be graded (everyting else will be put in
            a bucket & listed -->
    <xsl:variable name="Ungraded_Category">NOT GRADED</xsl:variable>

    <!-- Category name for when there is no specific grading category -->
    <xsl:variable name="Unassigned_Category">No Category Specified</xsl:variable>

    <!-- Category name for when there is no specific grading category -->
    <xsl:variable name="Missing_Category">No Category Found</xsl:variable>

    <xsl:output method='xml'
                version="1.0"
                encoding="UTF-8"
                indent="yes"
                omit-xml-declaration="no"/>
    <!-- indent="yes": may not be safe with doc types with "mixed content"?-->

    <!--========================================================================-->

    <xsl:template match="/">
        <StudentTrackerGradeDigest MinGrade="0" MaxGrade="10">
			<xsl:attribute name="ExerciseName"><xsl:call-template name="LessonNumber"/></xsl:attribute>
            <xsl:text>
		</xsl:text>
            <SetGrade value="10" />
			<xsl:call-template name="GenerateOverallFeedback"/>

            <Exercises>
                <!-- foreach test-suite that is a class (and therefore tests 1 exercise -->
                <xsl:for-each select="//test-suite[@description='Exercise']">
                    <!-- apply the templates to those (failed & passed) test cases -->
                    <!-- each point modification is indicated by a category, so that we can ding people once
                per type of mistake, instead of once per failed test -->
                    <Exercise>
                        <xsl:attribute name="name">
                            <xsl:value-of select='@name'/>
                        </xsl:attribute>

                        <FailedTests>
                            <!-- if there's at least one failure... -->
                            <xsl:for-each select="results/Categories/Category">
                                <xsl:sort select="@name"/>
                                <xsl:if test="test-case[@success='False']">
                                    <xsl:choose>
                                        <xsl:when test="@name=$Default_Category">
                                            <xsl:for-each select="test-case[@success='False']">
                                                <xsl:call-template name="GenerateSingleFailedTest">
                                                    <xsl:with-param name="CategoryName">
                                                        <xsl:value-of select="$Default_Category"/>
                                                    </xsl:with-param>
                                                    <xsl:with-param name="Node" select="current()" />
                                                    <xsl:with-param name="Testname">
                                                        <xsl:value-of select="@name"/>
                                                    </xsl:with-param>
                                                </xsl:call-template>
                                            </xsl:for-each>
                                        </xsl:when>
                                        <xsl:when test="@name=$Ungraded_Category">
                                            <!-- Anything marked as 'NOT GRADED' will be listed, 
                                                    but not penalized-->
                                            <xsl:call-template name="GenerateFailedTest">
                                                <xsl:with-param name="CategoryName">
                                                    <xsl:value-of select="@name"/>
                                                </xsl:with-param>
                                                <xsl:with-param name="NodeList" select="." />
                                                <xsl:with-param name="PointPenalty" select="0" />
                                                <xsl:with-param name="Explanation">The following tests failed, but since they're not being graded, their failure does not cause a point penalty</xsl:with-param>
                                            </xsl:call-template>
                                        </xsl:when>
                                        <xsl:when test="@name='' or @name=$Missing_Category">
                                            <!-- Anything without a category will also be listed,
                                                    but not penalized-->
                                            <xsl:call-template name="GenerateFailedTest">
                                                <xsl:with-param name="CategoryName">
                                                    <xsl:value-of select="$Unassigned_Category"/>
                                                </xsl:with-param>
                                                <xsl:with-param name="NodeList" select="." />
                                                <xsl:with-param name="PointPenalty" select="0" />
                                                <xsl:with-param name="Explanation">The following tests failed, but since they were not assigned to any category, their failure does not cause a point penalty</xsl:with-param>
                                            </xsl:call-template>
                                        </xsl:when>
                                        <xsl:otherwise>
                                            <xsl:apply-templates select="." />
                                        </xsl:otherwise>
                                    </xsl:choose>
                                </xsl:if>
                            </xsl:for-each>
                        </FailedTests>

                        <PassedTests>
                            <xsl:apply-templates select="results//test-case[@success='True']" />
                        </PassedTests>
                    </Exercise>
                </xsl:for-each>
            </Exercises>
			<xsl:call-template name="GenerateDisclaimer"/>
        </StudentTrackerGradeDigest>
    </xsl:template>

    <xsl:template name="GenerateSingleFailedTest">
        <xsl:param name="CategoryName" select="DEFAULT_NAME"/>
        <xsl:param name="Node" select="ERROR"/>
        <xsl:param name="Testname" />
        <xsl:param name="PointPenalty" select="$Default_Test_Failure_Point_Penalty" />

        <xsl:element name="ModGrade">
            <xsl:attribute name="value">
                <xsl:value-of select="$PointPenalty"/>
            </xsl:attribute>
            <xsl:attribute name="GradingCategory">
                <xsl:value-of select="$CategoryName"/>
            </xsl:attribute>
            <xsl:attribute name="explanation">
                The test named <xsl:value-of select="$Testname"/> failed
            </xsl:attribute>
            <Tests>
                <xsl:element name="Test">
                    <xsl:attribute name="testname">
                        <xsl:value-of select="$Testname"/>
                    </xsl:attribute>
                    <xsl:attribute name="explanation">There is no test-specific message, for this test</xsl:attribute>
                    <xsl:element name="Test_Messaage">
                        <xsl:value-of select="$Node/failure/message"/>
                    </xsl:element>
                    <xsl:element name="Test_StackTrace">
                        <xsl:value-of select="$Node/failure/stack-trace"/>
                    </xsl:element>
                </xsl:element>
            </Tests>
        </xsl:element>
    </xsl:template>

    <xsl:template name="GenerateFailedTest">
        <xsl:param name="CategoryName" select="DEFAULT_NAME"/>
        <xsl:param name="PointPenalty" select="-1"/>
        <xsl:param name="NodeList" select="ERROR"/>
        <xsl:param name="Explanation">EXPLANATION GOES HERE</xsl:param>

        <xsl:element name="ModGrade">
            <xsl:attribute name="value">
                <xsl:value-of select="$PointPenalty"/>
            </xsl:attribute>
            <xsl:attribute name="GradingCategory">
                <xsl:value-of select="$CategoryName"/>
            </xsl:attribute>
            <xsl:attribute name="explanation">
                <xsl:value-of select="$Explanation"/>
            </xsl:attribute>
            <Tests>
                <xsl:for-each select="$NodeList/test-case[@success='False']">
                    <xsl:element name="Test">
                        <xsl:attribute name="testname">
                            <xsl:value-of select="@name"/>
                        </xsl:attribute>
                        <xsl:attribute name="explanation"></xsl:attribute>
                        <xsl:element name="Test_Messaage">
                            <xsl:value-of select="failure/message"/>
                        </xsl:element>
                        <xsl:element name="Test_StackTrace">
                            <xsl:value-of select="failure/stack-trace"/>
                        </xsl:element>
                    </xsl:element>
                </xsl:for-each>
            </Tests>
        </xsl:element>
    </xsl:template>


    <!-- This is the template for tests that didn't match anything more specific,
            in case there's a test case that doesn't match anything - ->

    <xsl:template match="//test-case[@success='False']" priority="-20">
        <!- - This template runs with lower priority than the default templates, and ensures
                that if we don't have a match, we'll still get a message.  Also, if we do
                have a match, we will NOT use this rule  - ->

        <xsl:call-template name="GenerateSingleFailedTest">
            <xsl:with-param name="CategoryName">
                <xsl:value-of select="$Unassigned_Category"/>
            </xsl:with-param>
            <xsl:with-param name="Node" select="current()" />
            <xsl:with-param name="Testname">
                <xsl:value-of select="@name"/>
            </xsl:with-param>
            <xsl:with-param name="PointPenalty"> 0 </xsl:with-param>
        </xsl:call-template>
    </xsl:template>
    -->

    <xsl:template match="//test-case[@success='True']" priority="-20">
        <xsl:element name="Comment_PassedTest">
            <xsl:attribute name="testname">
                <xsl:value-of select="@name"/>
            </xsl:attribute>
        </xsl:element>
    </xsl:template>

</xsl:stylesheet>