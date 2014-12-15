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
			<xd:p>Přesune element kniha (členění bible) před značku folia, pokud jsou bezprostředně za sebou</xd:p>
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
  
	<xsl:template match="/doc//folio[name(following-sibling::*[position() = 1]) = 'kniha' or name(following-sibling::*[position() = 1]) = 'strana' or name(following-sibling::*[position() = 1]) = 'strana_edice']">
		<xsl:copy-of select="following-sibling::*[1]"/>
    <xsl:element name="{local-name()}">
    	<xsl:apply-templates select="@*" />
    </xsl:element>
  </xsl:template>

	<xsl:template match="/doc//kniha[name(preceding-sibling::*[position() = 1]) = 'folio']" />
	<xsl:template match="/doc//kniha[name(preceding-sibling::*[position() = 1]) = 'strana']" />
	<xsl:template match="/doc//kniha[name(preceding-sibling::*[position() = 1]) = 'strana_edice']" />
	
</xsl:stylesheet>
