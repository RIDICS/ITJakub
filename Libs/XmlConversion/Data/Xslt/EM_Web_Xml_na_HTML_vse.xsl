<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xsl:output method="html" indent="yes" encoding="UTF-8" />
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 22, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>

	<xsl:include href="EM_Web_Xml_na_HTML_fulltextcontent.xsl"/>
	<xsl:include href="EM_Web_Xml_na_HTML_notesTool.xsl"/>
	
	<xsl:template match="/">
			<xsl:apply-templates select="xmls" />
	</xsl:template>
	

</xsl:stylesheet>