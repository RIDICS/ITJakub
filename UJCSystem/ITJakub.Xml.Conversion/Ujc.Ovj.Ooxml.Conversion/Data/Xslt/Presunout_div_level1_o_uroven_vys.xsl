<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet exclude-result-prefixes="xs xd" version="1.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 15, 2014</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xsl:output indent="yes" />
	
	<xsl:strip-space elements="*" />
	
	<xsl:include href="Kopirovani_prvku.xsl" />
	<xsl:template match="/">
		<xsl:comment> Presunout_div_level1_o_uroven_vys </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="body">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:apply-templates />
		</xsl:copy>
	</xsl:template>
	
	
	<xsl:template match="*[div[@type='level1']]">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:apply-templates select="*[not(@type='level1')]" />
		</xsl:copy>
		<xsl:apply-templates select="div[@type='level1']" mode="move" />
	</xsl:template>
	
	<xsl:template match="div[@type='level1']" mode="move">
		<div>
			<xsl:apply-templates />
		</div>
	</xsl:template>
	
</xsl:stylesheet>