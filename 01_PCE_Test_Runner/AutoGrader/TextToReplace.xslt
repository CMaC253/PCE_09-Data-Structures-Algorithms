<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <!-- This is for boilerplate 'disclaimers' and such -->
	
  <xsl:template name="GenerateDisclaimer">
	  <Disclaimer>
		  The instructor reserves the right to go through your
		  program and verify the correctness of any of your work.
		  This includes (but is not limited to) both checking for 
		  attempts to 'crack the tests', as well as re-examining
		  code from failed tests.  
		  Therefore, the grade you see listed here may (or may not)
		  be your final grade, after you hand it in.
	  </Disclaimer>	  
  </xsl:template>

	<xsl:template name="GenerateOverallFeedback">
		<OverallFeedback>
			<p>Thank you for handing in these PCEs!</p>
<!--			<p>
				<u>Overall</u>
				<ul>
					<li>
						Please hand in ONLY the Student_Answers.cs file, and your feedback file (if something else is required, an exercise will specifically call that file out).
						Do NOT include the project for your Pre-Class Exercises anymore.  Thanks!
					</li>
				</ul>
			</p>
			<p>
				<u>PCE Feedback</u>
				<ul>
					<li>This looks good - thanks!</li>
					<li>
						This appears to be missing. There should be a file in .DOC/.DOCX/.PDF format in the zip that you handed in, that contains feedback for these pre-class exercises (Send this to me &amp; you&#39;ll get the points back,
						but make sure to hand this in the first time, in
						the future!) &#160; &#160; &#160; &#160; &#160; &#160; &#160; &#160;  <span style="color:#ff0000">(-2)</span>
					</li>
				</ul>
			</p>
-->		</OverallFeedback>
	</xsl:template>

</xsl:stylesheet>