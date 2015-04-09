<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" xmlns:t="http://www.tei-c.org/ns/1.0" exclude-result-prefixes="xd t" version="1.0">



	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 22, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Přesune ediční komentáře z těla do hlavičky dokumentu.</xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:output method="xml" indent="yes" />
	<!--<xsl:strip-space elements="*"/>-->

	<xsl:include href="Kopirovani_prvku.xsl" />

	<xsl:template match="/">
		<xsl:comment> EM_Presunout_edicni_komentar </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="t:front">
		<xsl:copy>
			<xsl:apply-templates />
			<xsl:if test="parent::*/t:body//t:div[@type='editorial' and @subtype='comment']">
				<xsl:element name="div" namespace="http://www.tei-c.org/ns/1.0">
					<xsl:attribute name="type"><xsl:text>editorial</xsl:text></xsl:attribute>
					<xsl:attribute name="subtype"><xsl:text>comment</xsl:text></xsl:attribute>
					<xsl:apply-templates select="parent::*/t:body//t:div[@type='editorial' and @subtype='comment']" mode="copy" />
				</xsl:element>
			</xsl:if>
			<xsl:if test="parent::*/t:body//t:div[@type='editorial' and @subtype='grant']">
				<xsl:element name="div" namespace="http://www.tei-c.org/ns/1.0">
					<xsl:attribute name="type"><xsl:text>editorial</xsl:text></xsl:attribute>
					<xsl:attribute name="subtype"><xsl:text>grant</xsl:text></xsl:attribute>
					<xsl:apply-templates select="parent::*/t:body//t:div[@type='editorial' and @subtype='grant']" mode="copy" />
				</xsl:element>
			</xsl:if>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="t:body//t:div[@type='editorial' and @subtype='comment']" />
	<xsl:template match="t:body//t:div[@type='editorial' and @subtype='comment']" mode="copy">
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="t:body//t:div[@type='editorial' and @subtype='grant']" mode="copy">
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="t:body//t:div[@type='editorial' and @subtype='annotation']" />

</xsl:stylesheet>
