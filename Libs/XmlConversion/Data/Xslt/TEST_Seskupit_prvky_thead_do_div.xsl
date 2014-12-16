<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl" exclude-result-prefixes="xd" version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p/>
		</xd:desc>
	</xd:doc>
	<xsl:output indent="yes"/>

	<xsl:strip-space elements="*"/>

<xsl:include href="Kopirovani_prvku.xsl" />

<xd:doc>
	<xd:desc>
		<xd:p>Pokud tělo obsahuje alespoň jeden element <xd:b>head</xd:b> nebo <xd:b>head1</xd:b> (začínající na "head"), vytvoří se pro takto ohraničené oblasti element <xd:b>div</xd:b>.</xd:p>
		<xd:p>Bylo by vhodné elementy, které předcházejí, rovněž seskupit do <xd:b>div</xd:b>, pokud tyto elementy netvoří <xd:b>div</xd:b> nebo <xd:b>pb</xd:b>.</xd:p>
		<xd:p>Pokud tělo žádný element <xd:b>head</xd:b> nebo <xd:b>head1</xd:b> neobsahuje, elementy se zkopírují. (Nebo by se měla vytvořit alespon jedna úroveň <xd:b>div</xd:b>?)</xd:p>
	</xd:desc>
</xd:doc>
	
	<xsl:template match="/">
		<xsl:comment> Seskupit_prvky_thead_do_div </xsl:comment>
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="body">
		<xsl:copy>
			<xsl:choose>
				<xsl:when test="count(/body/child::*[(starts-with(name(), 'thead'))]) &gt; 0">
					<xsl:apply-templates select="/body/*[position() &lt; count(/body/child::*[(starts-with(name(), 'thead'))][1]/preceding-sibling::*)+1]" />
					<xsl:apply-templates select="thead" mode="walker"/>
					<xsl:apply-templates select="/body/*[position() &gt; count(/body/child::*[(starts-with(name(), 'thead'))][1]/preceding-sibling::*) + 1 + count(/body/child::*[(starts-with(name(), 'thead'))][1]/following-sibling::*)+2]"></xsl:apply-templates>
				</xsl:when>
				<xsl:otherwise>
					<xsl:apply-templates />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:copy>
	</xsl:template>

<!--<xsl:template match="head">
	<xsl:apply-templates select="."/>
	<xsl:apply-templates select="head" mode="walker"/>
</xsl:template>
-->
	
	<!-- /body/child::*/following-sibling::*[name() != 'head1'] -->
<!--	<xsl:template match="preceding-sibling::*/head1[1]" mode="zacatek">
		
		</xsl:template>-->
	

	
	<xsl:template match="thead" mode="walker">
		<xsl:element name="div">
			<xsl:attribute name="id"><xsl:value-of select="generate-id(.)"/></xsl:attribute>
			<xsl:apply-templates select="."/>
			<xsl:apply-templates select="following-sibling::*[1]" mode="walker"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="*" mode="walker">
		<xsl:apply-templates select="."/>
		<xsl:if test="not(following-sibling::*[1]/self::thead)">
			<!--<xsl:if test="not(following-sibling::*[1]/self::div[not(@type='editorial' or @type = 'incipit' or @type='explicit')])">-->
				<xsl:apply-templates select="following-sibling::*[1]" mode="walker"/>
			</xsl:if>
		<!--</xsl:if>-->
	</xsl:template>
	

<xsl:template match="div[@type = 'incipit' or @type='explicit']">
	<xsl:copy-of select="."/>
	<!--<xsl:apply-templates />-->
</xsl:template>


	<xsl:template match="@*" priority="2">
		<xsl:variable name="nazev">
			<xsl:choose>
				<xsl:when test="local-name() = 'id'"><xsl:text>xml:id</xsl:text></xsl:when>
				<xsl:when test="local-name() = 'lang'"><xsl:text>xml:lang</xsl:text></xsl:when>
				<xsl:otherwise><xsl:value-of select="local-name()"/></xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:attribute name="{$nazev}">
			<xsl:value-of select="."/>
		</xsl:attribute>
	</xsl:template>	
	
	<xsl:template match="@xml:id" priority="3">
		<xsl:if test="name() = 'xml:id'">
			<xsl:attribute name="xml:id">
				<xsl:value-of select="."/>
			</xsl:attribute>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
