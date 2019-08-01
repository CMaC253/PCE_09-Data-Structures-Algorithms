<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<!-- The exercise name to put at the top of the document -->
	<xsl:template name="LessonNumber">PCE 09</xsl:template>


	<!-- This is for Categories that have a name, but the name doesn't match anything.
			This should never happen 'in production', and will be flagged as an error 
			during the output phase -->
	<xsl:template match="Category[@name!='']" priority="-10">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="$Missing_Category"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">
				Unable to find a grading category for <xsl:value-of select="@name"/>
			</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST Find And Remove Next Smallest: Normal Case']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-2" />
			<xsl:with-param name="Explanation">There is a problem finding and removing the next smaller value, given a pretty normal value to find and remove</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST Find And Remove Next Smallest: One Step Left']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Given a target value, remove the node immediately to the left</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST Find And Remove Next Smallest: One Step Left With Subtree']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Given a target value, remove the node immediately to the left (more values to the left of that node)</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST Find And Remove Next Smallest: One Step Right']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Given a target value, remove the node immediately to the right</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST Find And Remove Next Smallest: One Step Right With Subtree']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Given a target value, remove the node immediately to the right (more values to the right of that node)</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST.Remove from Empty Tree']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Failed when asked to remove from an empty tree</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST.Remove from tree with single node']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Failed when asked to remove from a tree containing a single node</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST.Remove from tree with two nodes']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Failed when asked to remove from a tree containing two nodes</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='BST.Remove from three element tree (case 2)']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Case 2 on a 3-node tree</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="Category[@name='BST.Remove (general)']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-2" />
			<xsl:with-param name="Explanation">Failed to properly partition one (or more) arrays</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='QS_Partition']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Failed to properly partition one (or more) arrays</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

	<xsl:template match="Category[@name='QS_QuickSort']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-2" />
			<xsl:with-param name="Explanation">Failed to properly QuickSort one (or more) arrays</xsl:with-param>
		</xsl:call-template>
	</xsl:template>
	
	<xsl:template match="Category[@name='QS_QuickSort_BiggerArrays']">
		<xsl:call-template name="GenerateFailedTest">
			<xsl:with-param name="CategoryName">
				<xsl:value-of select="@name"/>
			</xsl:with-param>
			<xsl:with-param name="NodeList" select="." />
			<xsl:with-param name="PointPenalty" select="-1" />
			<xsl:with-param name="Explanation">Failed to properly QuickSort one (or more) arrays. These arrays are slightly larger, having at least 7 elements (each)</xsl:with-param>
		</xsl:call-template>
	</xsl:template>

</xsl:stylesheet>

