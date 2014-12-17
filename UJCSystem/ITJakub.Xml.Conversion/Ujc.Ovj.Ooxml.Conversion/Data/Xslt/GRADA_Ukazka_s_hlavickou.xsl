<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns="http://www.tei-c.org/ns/1.0"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Dec 4, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> boris</xd:p>
			<xd:p></xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:output method="xml" omit-xml-declaration="no" indent="yes"/>
	<xsl:strip-space elements="*"/>
<!--	
	<xsl:variable name="titul">Pojištění úvěrových rizik v mezinárodním obchodě</xsl:variable>
	<xsl:variable name="autor">doc. Ing. Arnošt Bohm, CSc., Ing. František Janatka, CSc.</xsl:variable>
	<xsl:variable name="rok">2004</xsl:variable>
	<xsl:variable name="ISBN">80-247-0816-7</xsl:variable>
	-->
	
	<!--
	<xsl:variable name="titul">Marketing a management muzeí a památek</xsl:variable>
	<xsl:variable name="autor">PhDr. Ladislav Kesner, Ph.D.</xsl:variable>
	<xsl:variable name="rok">2005</xsl:variable>
	<xsl:variable name="ISBN">80-247-1104-4</xsl:variable>
	-->

<!--
	<xsl:variable name="titul">NÁBOŽENSKÉ SYSTÉMY – Člověk náboženský a jak mu porozumět</xsl:variable>
	<xsl:variable name="autor">prof. PhDr. Josef Kandert, CSc.</xsl:variable>
	<xsl:variable name="rok">2010</xsl:variable>
	<xsl:variable name="ISBN">978-80-247-3166-7</xsl:variable>
-->

	<xsl:variable name="titul">Sebedůvěra – Umění získat cokoliv chcete</xsl:variable>
	<xsl:variable name="autor">Rob Yeung</xsl:variable>
	<xsl:variable name="rok">2009</xsl:variable>
	<xsl:variable name="ISBN">978-80-247-3127-8</xsl:variable>



<!--	<xsl:variable name="titul">Fotografujeme digitálně I.</xsl:variable>
	<xsl:variable name="autor">Roman Soukup</xsl:variable>
	<xsl:variable name="rok">2004</xsl:variable>
	<xsl:variable name="ISBN">80-247-1087-0</xsl:variable>
-->	
	
	
	<xsl:template match="/">
		<TEI xmlns="http://www.tei-c.org/ns/1.0" xml:lang="cs">
			<teiHeader>
				<fileDesc>
					<titleStmt>
						<title><xsl:value-of select="$titul" /></title>
						<author><xsl:value-of select="$autor"/></author>
					</titleStmt>
					<publicationStmt>
						<publisher>Grada Publishing, a.s.</publisher>
						<pubPlace>Praha</pubPlace>
						<date><xsl:value-of select="$rok"/></date>
						<availability status="restricted">
							<p>Tato edice je autorské dílo chráněné ve smyslu zákona č. 121/2000 Sb., o právu autorském.</p>
						</availability>
						<idno type="ISBN_E"><xsl:value-of select="$ISBN"/></idno>
						<idno type="ISBN_P"><xsl:value-of select="$ISBN"/></idno>
					</publicationStmt>
					<sourceDesc>
						<msDesc xml:id="ujcav_10" xml:lang="cs">
							<msIdentifier>
								<country key="xs">Česko</country>
								<settlement>Praha</settlement>
								<repository>Archiv Pražského hradu</repository>
								<idno>XUJC</idno>
							</msIdentifier>
							<msContents>
								<msItem>
									<title><xsl:value-of select="$titul" /></title>
									<author><xsl:value-of select="$autor" /></author>
								</msItem>
							</msContents>
							<history>
								<origin>
									<origDate notBefore="2010" notAfter="2010">2010</origDate>
								</origin>
							</history>
						</msDesc>
					</sourceDesc>
				</fileDesc>
				<revisionDesc>
					<change when="2009-12-13">Převod do elektronické podoby</change>
				</revisionDesc>
			</teiHeader>
			<text>
				<front>
					<docTitle>
						<titlePart><xsl:value-of select="$titul" /></titlePart>
					</docTitle>
					<figure>
						<graphic url="top-logo-grada.gif"/>
					</figure>
				</front>
					<xsl:apply-templates mode="jmennyProstor" />
			</text>
			</TEI>
	</xsl:template>
	

	
	<xsl:template match="*" mode="jmennyProstor">
		<xsl:element name="{local-name()}" namespace="http://www.tei-c.org/ns/1.0">
			<xsl:apply-templates select="@*|*|text()" mode="jmennyProstor"/>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="@*" mode="jmennyProstor">
		<xsl:variable name="nazev">
		<xsl:choose>
			<xsl:when test="local-name() = 'id'"><xsl:text>xml:id</xsl:text></xsl:when>
			<xsl:otherwise><xsl:value-of select="local-name()"/></xsl:otherwise>
		</xsl:choose>
		</xsl:variable>
		<xsl:attribute name="{$nazev}">
			<xsl:value-of select="."/>
		</xsl:attribute>
	</xsl:template>
	
</xsl:stylesheet>