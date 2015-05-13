<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
  xmlns:tei="http://www.tei-c.org/ns/1.0"
  xmlns:nlp="http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0"
  xmlns:exist="http://exist.sourceforge.net/NS/exist"
  exclude-result-prefixes="xd tei nlp exist"
  version="1.0">

	<xsl:output method="text"/>
	
  <xsl:template match="tei:c[@type='space']" priority="5">
    <xsl:text> </xsl:text>
  </xsl:template>

	<xsl:template match="*">
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="tei:choice" />

	<xsl:template match="tei:note" />

	<xsl:template match="*[@rend='hidden']" priority="10" />



</xsl:stylesheet>