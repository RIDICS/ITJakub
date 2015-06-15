<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet exclude-result-prefixes="xs xd" version="2.0" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Jan 8, 2012</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris</xd:p>
			<xd:p />
		</xd:desc>
	</xd:doc>

	<xsl:output indent="yes" />

	<xsl:strip-space elements="*" />

	<xsl:include href="Kopirovani_prvku.xsl" />

	<xd:doc>
		<xd:desc>
			<xd:p>Pokud tělo obsahuje alespoň jeden element <xd:b>head</xd:b> nebo <xd:b>head1</xd:b> (začínající na "head"), vytvoří se pro takto ohraničené oblasti element <xd:b>div</xd:b>.</xd:p>
			<xd:p>Bylo by vhodné elementy, které předcházejí, rovněž seskupit do <xd:b>div</xd:b>, pokud tyto elementy netvoří <xd:b>div</xd:b> nebo <xd:b>pb</xd:b>.</xd:p>
			<xd:p>Pokud tělo žádný element <xd:b>head</xd:b> nebo <xd:b>head1</xd:b> neobsahuje, elementy se zkopírují. (Nebo by se měla vytvořit alespon jedna úroveň <xd:b>div</xd:b>?)</xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:template match="/">
		<xsl:comment> Seskupit_prvky_div1_do_div_2.0 </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="body">
		<xsl:copy>
			<xsl:copy-of select="@*" />
			<xsl:choose>
				<xsl:when test="div1">
					<xsl:for-each-group select="*" group-adjacent="if (self::div1) then 0 else position()">
						<xsl:apply-templates select="." />
					</xsl:for-each-group>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="div1">
		<div>
			<xsl:apply-templates select="current-group()" mode="each-div1"/>
		</div>
	</xsl:template>

	<xsl:template match="div1" mode="each-div1">
		<div>
				<xsl:apply-templates select="child::*" />
		</div>
	</xsl:template>
	

	<xsl:template match="*" mode="group">
		<xsl:copy-of select="current-group()" />
	</xsl:template>

	<xsl:template match="div[@type = 'incipit' or @type='explicit' or @type='editorial']">
		<xsl:copy-of select="." />
	</xsl:template>

</xsl:stylesheet>
