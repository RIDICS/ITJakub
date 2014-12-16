<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
    xmlns:t="http://www.tei-c.org/ns/1.0">
    
    
	<xsl:include href="Kopirovani_prvku.xsl"/>
    
    <xsl:template match="/">
        <xsl:apply-templates />
    </xsl:template>
    
    <xsl:template match="t:body" >
    	<xsl:element name="body" namespace="http://www.tei-c.org/ns/1.0">
    		<xsl:element name="div" namespace="http://www.tei-c.org/ns/1.0" />
    	</xsl:element>
    </xsl:template>
    <xsl:template match="t:back" />

</xsl:stylesheet>