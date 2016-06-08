<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei" version="2.0">
	
	<xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>

    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
	<xsl:preserve-space elements="text"/>
	<xsl:variable name="vychozi-jazyk" select="'cs'"/>

	<xsl:template match="/">
		<xsl:text xml:space="preserve">
		</xsl:text>
    <xsl:comment> DDBW_Compute_entryFreeId </xsl:comment>
		<xsl:text xml:space="preserve">
		</xsl:text>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="tei:entryFree">
    <xsl:copy>
      <xsl:copy-of select="@*"/>
      
      <xsl:attribute name="xml:id">
        <xsl:value-of select="concat('en', substring(string(1000001 + count(preceding-sibling::tei:entryFree)), 2))"/>
      </xsl:attribute>
      
      <xsl:apply-templates />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="*">
    <xsl:copy>
      <xsl:copy-of select="@*" />
      
      <xsl:apply-templates />
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
