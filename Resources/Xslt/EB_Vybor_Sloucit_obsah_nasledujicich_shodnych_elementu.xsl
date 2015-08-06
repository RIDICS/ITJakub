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
	<xsl:template match="div[@subtype='annotation']">
		<xsl:variable name="name" select="local-name()"/>
		<xsl:copy>
		<!-- Is this the first element in a sequence? -->
		<xsl:if test="local-name(preceding-sibling::*[position()=1]) != $name">
			
				<xsl:apply-templates />
				
				<!-- Match the next sibling if it has the same name -->
				<xsl:apply-templates select="following-sibling::*[1][local-name()=$name]" mode="next"/>
			
		</xsl:if>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="div[@subtype='annotation'][name() = name(preceding-sibling::*[1])]"  mode="next">
		
			<xsl:apply-templates />
		
	</xsl:template>
	
	<!-- Recursive template used to match the next sibling if it has the same name -->
  <!--   //*[name() = name((following-sibling::*)[1])] -->
  <!-- Normalni/child::* -->
  <xsl:template match="//*[name() = name(following-sibling::*[1])]" mode="next">
<!--	<xsl:template match="div[@subtype='annotation']/child::*" mode="next">-->
		<xsl:variable name="name" select="local-name()"/>
  	<p>
		<xsl:apply-templates />
  	</p>
		<xsl:apply-templates select="following-sibling::*[1][local-name()=$name]" mode="next"/>
	</xsl:template>
	
	
	<!--  
	<xsl:template match="*">
		<xsl:variable name="name" select="local-name()"/>
		
		<xsl:if test="name() != name(preceding-sibling::*[1])">
			<xsl:copy>
				<xsl:copy-of select="@*"/>
				<xsl:value-of select="."/>
				<xsl:if test="name() = name(following-sibling::*[1])">
					<xsl:apply-templates select="following-sibling::*[1]" mode="merge"/>
				</xsl:if>
			</xsl:copy>
		</xsl:if>
	</xsl:template>
	-->

<!-- 
	<xsl:template match="*" mode="merge"> -->
		<!-- don't copy element, only it's contents. -->
<!--		<xsl:apply-templates/>
	</xsl:template>
	-->

</xsl:stylesheet>
