<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns="http://www.tei-c.org/ns/1.0"
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	exclude-result-prefixes="xs xd"
	version="2.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Sep 2, 2015</xd:p>
			<xd:p><xd:b>Author:</xd:b> lehecka</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:output method="xml" indent="yes" />
	<xsl:strip-space elements="*"/>
	
	<xsl:template match="/">
		<itj:editorial-comment xmlns="http://www.tei-c.org/ns/1.0"
			xmlns:itj="http://vokabular.ujc.cas.cz/ns/it-jakub/1.0">
			<div type="editorial" subtype="comment">
				<xsl:copy-of select="//tei:sourceDesc[@n='characteristic']" />
				<xsl:copy-of select="//tei:sourceDesc[@n='bibl-primary']" />
				<xsl:copy-of select="//tei:sourceDesc[@n='bibl-secondary']" />
				<xsl:copy-of select="//tei:sourceDesc[not(@n)]" />
			</div>
		</itj:editorial-comment>
	</xsl:template>
	
</xsl:stylesheet>