<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
  
  <xsl:strip-space elements="*"/>
  
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 1, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p>Přesune element foliace před značku odstavce, nadpisu atp., pokud jde o první značku</xd:p>
		</xd:desc>
	</xd:doc>
  
	<xsl:include href="Kopirovani_prvku.xsl"/>
  
  <xd:doc>
    <xd:desc>
      <xd:p>Platí pro všechny elementy, jejichž prvním podřízeným elementem je <xd:b>foliace</xd:b>.</xd:p>
    </xd:desc>
  </xd:doc>
	
<!--	<xsl:template match="body" priority="2">
		<xsl:element name="body">
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>-->
  
	<xsl:template match="/doc//*[name(child::*[position() = 1]) = 'foliace']">
    <xsl:copy-of select="child::*[1]"/>
    <xsl:element name="{local-name()}">
      <xsl:apply-templates select="child::*[position() &gt; 1]"></xsl:apply-templates>
    </xsl:element>
  </xsl:template>
	
	<xsl:template match="/doc//*[name(child::*[position() = 1]) = 'pb']">
		<xsl:copy-of select="child::*[1]"/>
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*" />
			<xsl:apply-templates select="child::*[position() &gt; 1]"></xsl:apply-templates>
		</xsl:element>
	</xsl:template>
	
</xsl:stylesheet>
