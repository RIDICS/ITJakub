<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd" version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 1, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris Lehečka</xd:p>
			<xd:p>Sloučí obsah dvou po sobě jdoucích elementů, které mají shodné jméno</xd:p>
		</xd:desc>
	</xd:doc>
  <xsl:output omit-xml-declaration="no" indent="yes"/>
  <xsl:strip-space elements="*"/>
	<xsl:include href="Kopirovani_prvku.xsl"/>
	
	<xsl:template match="/">
		<xsl:comment> Sloucit_obsah_nasledujicich_shodnych_elementu </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>

	
	<!-- Match children of the block element -->
	<!-- Dodělat shodu tak, aby se slučovaly všechny podřízené elementy -->
  <!-- Normalni/child::* -->
  <!--<xsl:template match="//*[name() = name(following-sibling::*[1])]">-->
  <xsl:template match="Normalni/child::* | Nadpis/child::*">
		<xsl:variable name="name" select="local-name()"/>
		
		<!-- Is this the first element in a sequence? -->
		<xsl:if test="local-name(preceding-sibling::*[position()=1]) != $name">
			<xsl:copy>
				<xsl:apply-templates select="@*" />
				<xsl:apply-templates />
				
				<!-- Match the next sibling if it has the same name -->
				<xsl:apply-templates select="following-sibling::*[1][local-name()=$name]" mode="next"/>
			</xsl:copy>
		</xsl:if>
	</xsl:template>
	
	<!-- Recursive template used to match the next sibling if it has the same name -->
  <!--   //*[name() = name((following-sibling::*)[1])] -->
  <!-- Normalni/child::* -->
  <!--<xsl:template match="//*[name() = name(following-sibling::*[1])]" mode="next">-->
	<xsl:template match="Normalni/child::* | Nadpis/child::*" mode="next">
		<xsl:variable name="name" select="local-name()"/>
		<xsl:apply-templates />
		<xsl:apply-templates select="following-sibling::*[1][local-name()=$name]" mode="next"/>
	</xsl:template>
	
</xsl:stylesheet>
