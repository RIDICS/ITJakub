<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd" version="1.0"
	>
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 3, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p/>
		</xd:desc>
	</xd:doc>

	<xsl:include href="Prevod_stylu_na_TEI.xsl"/>
	<xsl:include href="EM+EB_Prevod_stylu_na_TEI_spolecne.xsl"/>
	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>

	<xsl:template match="/">
		<xsl:comment> EM_Prevod_stylu_na_TEI </xsl:comment>
		<xsl:apply-templates/>
	</xsl:template>
	
	<!-- Ignrtované styly -->
	
	<!-- Anotace je určena jenom pro e-shop Academie -->
	<xsl:template match="Anotace" />
	
	<xsl:template match="Komercni_titul" />

</xsl:stylesheet>
