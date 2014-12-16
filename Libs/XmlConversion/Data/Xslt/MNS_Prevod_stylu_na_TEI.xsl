<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
  exclude-result-prefixes="xd"
  version="1.0">
  <xd:doc scope="stylesheet">
    <xd:desc>
      <xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
      <xd:p><xd:b>Author:</xd:b> boris</xd:p>
      <xd:p></xd:p>
    </xd:desc>
  </xd:doc>
  
  <xsl:include href="Prevod_stylu_na_TEI.xsl"/>
  <xsl:output omit-xml-declaration="no" indent="yes"/>
  <xsl:strip-space elements="*"/>
  
  <xsl:template match="/">
    <xsl:apply-templates />
  </xsl:template>
  
  <xsl:template match="body">
    <body>
      <xsl:apply-templates />
    </body>
  </xsl:template>
  
  <xsl:template match="bible_zkratka_knihy | bible_cislo_kapitoly | bible_cislo_verse">
    <xsl:element name="supplied">
      <xsl:attribute name="n"><xsl:value-of select="."/></xsl:attribute>
      <xsl:attribute name="reason"><xsl:text>bible</xsl:text></xsl:attribute>
      <!--<xsl:apply-templates />-->
    </xsl:element>
  </xsl:template>


<xd:doc>
	<xd:desc>
		<xd:p>Položka rejstříku pro MNS se převede na element <xd:b>label</xd:b>, ož však nodpovídá významu tohoto prvku podle TEI.</xd:p>
		<xd:p>Vhodnější způsob zachycení je pomocí prvku <xd:b>item</xd:b>v rámci seznamu <xd:b>list @type="index"</xd:b>.</xd:p>
	</xd:desc>
</xd:doc>
	
	<xsl:template match="Polozka_rejstriku">
			<label>
				<xsl:apply-templates />
			</label>
	</xsl:template>
	
<xsl:template match="transliterace" />
<xsl:template match="Edicni_komentar" />
	<xsl:template match="transliterace_rozepsani_zkratky" />
	
  
<!--  <xsl:template match="*">
    <xsl:apply-templates  />
  </xsl:template>
-->  
</xsl:stylesheet>