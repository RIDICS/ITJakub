<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" 
	xmlns:tei="http://www.tei-c.org/ns/1.0"
	xmlns="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd tei" version="2.0">
	<xsl:import href="COMMON_Unknown_element.xsl" />
	<xsl:import href="Pagina.xsl"/>
	
	<xd:doc scope="stylesheet">
        <xd:desc>
            <xd:p><xd:b>Created on:</xd:b> Feb 5, 2013</xd:p>
            <xd:p><xd:b>Author:</xd:b> Boris</xd:p>
            <xd:p/>
        </xd:desc>
    </xd:doc>

    <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
    <xsl:strip-space elements="*"/>
	<xsl:preserve-space elements="text"/>
	<xsl:variable name="vychozi-jazyk" select="'cs'"/>

	<xsl:template match="body">
		<div xml:id="body.div-1" xmlns="http://www.tei-c.org/ns/1.0">
			<xsl:apply-templates />
		</div>
	</xsl:template>

	<xsl:template match="/">
		<xsl:text xml:space="preserve">
		</xsl:text>
        <xsl:comment> DDBW_Rename </xsl:comment>
		<xsl:text xml:space="preserve">
		</xsl:text>
        <body>
            <xsl:apply-templates/>
        </body>
    </xsl:template>

	<xsl:template match="Predmluva">
		<p>
			<xsl:apply-templates/>
		</p>
	</xsl:template>
	
	<xsl:template match="Predmluva/Nemcina">
		<foreign xml:lang="de-x-transcr">
			<xsl:apply-templates/>
		</foreign>
	</xsl:template>
	
	<xsl:template match="Litera">
		<xsl:element name="head2">
			<xsl:if test="node()[self::Nemcina]">
				<xsl:attribute name="xml:lang">
					<xsl:text>de-x-translit</xsl:text>
				</xsl:attribute>
			</xsl:if>
			
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Litera/Nemcina">
		<xsl:apply-templates/>
	</xsl:template>
	
	<xsl:template match="Heslovy_Odstavec">
		<xsl:element name="entryFree">
			<xsl:attribute name="xml:id">
				<xsl:value-of select="concat('en', substring(string(1000001 + count(preceding-sibling::Heslovy_Odstavec)), 2))"/>
			</xsl:attribute>
			<xsl:for-each-group select="node()" group-ending-with="Cislo_Vyznamu">
				<xsl:if test="position() = 1">
					<xsl:apply-templates select="current-group()[self::Stranka]"/>
					
					<sense>
						<xsl:apply-templates select="current-group()[not (self::Cislo_Vyznamu)][not (self::Stranka)]" mode="transliteration"/>
					</sense>
				</xsl:if>
			</xsl:for-each-group>
			
			<xsl:for-each-group select="node()" group-starting-with="Cislo_Vyznamu">
				<xsl:if test="position() > 1">
					<sense>
						<xsl:apply-templates select="current-group()" mode="transliteration"/>
					</sense>
				</xsl:if>
			</xsl:for-each-group>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Nemcina" mode="transliteration">
		<reg xml:lang="de-x-translit">
			<xsl:apply-templates />
		</reg>
	</xsl:template>
	
	<!--
	<xsl:template match="Heslovy_Odstavec/Nemcina">
		<sense>
			<xsl:apply-templates select="following-sibling::Nemcina[1]/preceding-sibling::Cislo_Vyznamu except self::* except preceding-sibling::*" mode="sense_num" />
			<xsl:element name="form">
				<orth xml:lang="de" >
					<choice>
						<reg xml:lang="cs-x-transcr"><xsl:apply-templates select="text()" /></reg>
						<xsl:apply-templates select="following-sibling::Nemcina[1]/preceding-sibling::* except self::* except preceding-sibling::*" mode="transliteration" />
					</choice>
				</orth>
			</xsl:element>
		</sense>
	</xsl:template>
	-->
	
	<xsl:template match="Cislo_Vyznamu" mode="transliteration">
		<num><xsl:apply-templates /></num>
	</xsl:template>
	<xsl:template match="Kvalifikator" mode="transliteration">
		<iType>
			<xsl:apply-templates />
		</iType>
	</xsl:template>
	<xsl:template match="Cestina" mode="transliteration">
		<orig xml:lang="cs-x-translit"><xsl:apply-templates /></orig>
	</xsl:template>
	<xsl:template match="Latina" mode="transliteration">
		<orig xml:lang="lat-x-translit"><xsl:apply-templates /></orig>
	</xsl:template>
</xsl:stylesheet>
