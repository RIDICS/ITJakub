<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	exclude-result-prefixes="xd" version="1.0">
	<xsl:template match="*" priority="-20">
		<xsl:message terminate="no"> Neznámý element: <xsl:value-of select="name()"/> </xsl:message>
		<error type="element" reason="missing template" name="{name()}" /> 
		<xsl:element name="span">
			<xsl:attribute name="class" ><xsl:value-of select="name()"/></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
</xsl:stylesheet>