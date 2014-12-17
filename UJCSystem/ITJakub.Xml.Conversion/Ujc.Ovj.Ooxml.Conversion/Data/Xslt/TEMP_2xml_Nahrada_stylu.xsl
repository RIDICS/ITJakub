<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
  xmlns:x="http://schema.brus.cz/2010/WDoc2Xml.xsd"
  exclude-result-prefixes="xd x"
  version="1.0">
  <xd:doc scope="stylesheet">
    <xd:desc>
      <xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
      <xd:p><xd:b>Author:</xd:b> boris</xd:p>
      <xd:p></xd:p>
    </xd:desc>
  </xd:doc>
  
  
  <xsl:template match="/">
    <xsl:element name="xsl:stylesheet">
      <!--<xsl:attribute name="xmlns:xd"><xsl:text>http://www.oxygenxml.com/ns/doc/xsl</xsl:text></xsl:attribute>-->
      <!--<xsl:attribute name="exclude-result-prefixes"><xsl:text>xd</xsl:text></xsl:attribute>-->
      <xsl:attribute name="version"><xsl:text>1.0</xsl:text></xsl:attribute>
      <xsl:apply-templates select="//x:tag" />
    </xsl:element>
  </xsl:template>
  
  <xsl:template match="x:tag[@ignorovat='false']">
    
    <xsl:element name="xsl:template">
      <xsl:attribute name="match"><xsl:value-of select="@styl"/></xsl:attribute>
      <xsl:element name="xsl:element">
        <xsl:attribute name="name"><xsl:value-of select="@nazev"/></xsl:attribute>
      </xsl:element>
      <xsl:apply-templates select="x:atribut" />
      <xsl:element name="xsl:apply-templates" />
        
      
    </xsl:element>
    
  </xsl:template>

  <xsl:template match="x:tag[@ignorovat='true']">
    <xsl:element name="xsl:template">
      <xsl:attribute name="match"><xsl:value-of select="@styl"/></xsl:attribute>
    </xsl:element>
  </xsl:template>
  
  <!-- <atribut nazev="n" hodnota="^\o"/> -->
  <xsl:template match="x:atribut">
    <xsl:element name="xsl:attribute">
      <xsl:attribute name="name"><xsl:value-of select="@nazev"/></xsl:attribute>
      <xsl:choose>
        <xsl:when test="@hodnota = '^\o'">
          <xsl:element name="xsl:value-of">
            <xsl:attribute name="select"><xsl:text>.</xsl:text></xsl:attribute>
          </xsl:element>
        </xsl:when>
        <xsl:otherwise>
          <xsl:element name="xsl:text"><xsl:value-of select="@hodnota"/></xsl:element>
        </xsl:otherwise>
      </xsl:choose>
      
    </xsl:element>
  </xsl:template>

</xsl:stylesheet>