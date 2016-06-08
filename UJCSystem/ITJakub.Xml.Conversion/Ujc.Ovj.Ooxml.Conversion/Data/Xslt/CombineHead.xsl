<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xml="http://www.w3.org/XML/1998/namespace"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    xmlns="http://www.tei-c.org/ns/1.0"
    exclude-result-prefixes="tei"
    version="1.0">

  <xsl:import href="Kopirovani_prvku.xsl"/>

  <xsl:param name="fascimileDoc" />

  <xsl:template match="/">
    <xsl:comment> CombineHead </xsl:comment>
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="tei:TEI">
    <xsl:copy>
      <xsl:copy-of select="@*" />
      <xsl:apply-templates select="*" />
      <xsl:apply-templates select="document($fascimileDoc)"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="tei:TEI/tei:text" />
</xsl:stylesheet>