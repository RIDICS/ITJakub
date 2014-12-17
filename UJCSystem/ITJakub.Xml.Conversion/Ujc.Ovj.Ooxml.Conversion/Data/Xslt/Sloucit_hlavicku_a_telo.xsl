<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd" version="1.0">
	<xsl:output indent="yes"/>
	<xsl:strip-space elements="*"/>
  <xd:doc scope="stylesheet">
    <xd:desc>
      <xd:p><xd:b>Created on:</xd:b> Dec 2, 2010</xd:p>
      <xd:p><xd:b>Author:</xd:b> boris</xd:p>
      <xd:p>Sloučí hlavičku a tělo dokumentu</xd:p>
    </xd:desc>
  </xd:doc>
  <xsl:param name="hlavicka" />
		<xsl:param name="front" />
	<xsl:param name="body" />
	

  <xsl:template match="/">
    <TEI>
    	<xsl:copy-of select="document($hlavicka)/teiHeader"/>
    	<text xml:lang="cs">
    		<xsl:copy-of select="document($front)/front"/>
    		<xsl:copy-of select="document($body)/body"/>
    	</text>
    </TEI>
  </xsl:template>

</xsl:stylesheet>
