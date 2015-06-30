<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xs xd"
	version="2.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jun 19, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> lehecka</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xsl:variable name="rozmer">A4</xsl:variable>
	
	<xd:doc type="string" class="layout"> Paper height </xd:doc>
	<xsl:param name="pageHeight">297mm</xsl:param>
	
	<xd:doc type="string" class="layout"> Paper width </xd:doc>
	<xsl:param name="pageWidth">211mm</xsl:param>
	
	<xd:doc type="string" class="layout"> Margin at bottom of text area </xd:doc>
	<xsl:param name="pageMarginBottom">50pt</xsl:param>
	
	<xd:doc type="string" class="layout"> Left margin </xd:doc>
	<xsl:param name="pageMarginLeft">80pt</xsl:param>
	
	
	<xd:doc type="string" class="layout"> Right margin </xd:doc>
	<xsl:param name="pageMarginRight">80pt</xsl:param>
	
	<xd:doc type="string" class="layout"> Margin at top of text area </xd:doc>
	<xsl:param name="pageMarginTop">50pt</xsl:param>
	
	<xd:doc type="string" class="layout"> Margin at bottom of text body </xd:doc>
	<xsl:param name="bodyMarginBottom">24pt</xsl:param>
	
	<xd:doc type="string" class="layout"> Margin at top of text body </xd:doc>
	<xsl:param name="bodyMarginTop">24pt</xsl:param>
	
	<xd:doc type="string" class="layout"> Region after. The region after extent is the height of the area where footers are printed. </xd:doc>
	<xsl:param name="regionAfterExtent">14pt</xsl:param>
	
	<xd:doc type="string" class="layout"> Region before. The region before extent is the height of the area where headers are printed. </xd:doc>
	<xsl:param name="regionBeforeExtent">14pt</xsl:param>
	
	<xsl:param name="venovaniSize">
		<xsl:value-of select="concat( round($bodyMaster * 1.25), 'pt')"/>
	</xsl:param>
</xsl:stylesheet>