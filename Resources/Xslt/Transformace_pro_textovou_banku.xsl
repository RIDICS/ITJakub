<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Oct 22, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris Lehečka</xd:p>
			<xd:p>Transformace pro textovou banku. Slučuje jednotlivé kroky do jedné transformace.</xd:p>
		</xd:desc>
	</xd:doc>
	
	 <xsl:include href="Odstranit_tabulku_metadat.xsl"/>
	<xsl:include href="TB_Prejmenovat_odstavcove_prvky.xsl"/>
	
<!--	<xsl:template match="*">
		<xsl:apply-templates />
	</xsl:template>
	-->
</xsl:stylesheet>