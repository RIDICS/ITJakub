<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	exclude-result-prefixes="xd"
	version="1.0">
	<xd:doc scope="stylesheet">
		<xd:desc>
			<xd:p><xd:b>Created on:</xd:b> Oct 22, 2010</xd:p>
			<xd:p><xd:b>Author:</xd:b> Boris Lehečka</xd:p>
			<xd:p>Přejmenovává odstavcové prvky tak, aby odpovídaly nýzvům prvků používaných ve vertikále.</xd:p>
			<xd:p>Odstraní volný řádek, který v dokumentu slouží pouze pro formátování.</xd:p>
			<xd:p>Odstraní z výstupu ediční komentář.</xd:p>
		</xd:desc>
	</xd:doc>
	
	
	<xsl:output omit-xml-declaration="no"/>
	
	
	<xsl:template match="node()|@*">
		<xsl:copy>
			<xsl:apply-templates select="node()|@*"/>
		</xsl:copy>
	</xsl:template>
	
	
	<xd:doc scope="component">
		<xd:desc>
			<xd:p>Přejmenovává odstavcové prvky.</xd:p>
		</xd:desc>
	</xd:doc>
	
	<xsl:template match="body | Titul | Nadpis | Podnadpis | Normalni | Polozka_rejstriku | Vers | Incipit | Explicit">
		<xsl:variable name="nazev">
			<xsl:choose>
				<xsl:when test="name() = 'body'">doc</xsl:when>
				<xsl:when test="name() = 'Titul'">titul</xsl:when>
				<xsl:when test="name() = 'Nadpis'">nadpis</xsl:when>
				<xsl:when test="name() = 'Podnadpis'">podnadpis</xsl:when>
				<xsl:when test="name() = 'Normalni'">odstavec</xsl:when>
				<xsl:when test="name() = 'Polozka_rejstriku'">rejstrik</xsl:when>
				<xsl:when test="name() = 'Vers'">vers</xsl:when>
				<xsl:when test="name() = 'Incipit'">incipit</xsl:when> <!-- lze to použít? -->
				<xsl:when test="name() = 'Explicit'">explicit</xsl:when> <!-- lze to použít? -->
				<xsl:otherwise><xsl:value-of select="name()"/> </xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		
<!--		<xsl:choose>
			<xsl:when test="name(child::*[1]) = 'foliace'">
				<xsl:copy-of select="child::*[1]"/>
				<xsl:element name="{$nazev}">
					<xsl:apply-templates />
				</xsl:element>
				
			</xsl:when>
			<xsl:otherwise>
				<xsl:element name="{$nazev}">
					<xsl:apply-templates/>
				</xsl:element>
			</xsl:otherwise>
		</xsl:choose>
-->		
		
<!--		<xsl:if test="name(ancestor::node()[1]) = 'foliace'">
			<xsl:copy-of select="ancestor::node()[1]"/>
			<xsl:element name="{$nazev}">
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:if>
		
-->	
		<xsl:element name="{$nazev}">
			<xsl:apply-templates/>
		</xsl:element>
		
	</xsl:template>

	<!--<xsl:template match="foliace[*[1]/self::Normalni]" />-->
	

	<xd:doc>
		<xd:desc>
			<xd:p>Odstraňuje z výstupu volný řádek a ediční komentář.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="Volny_radek | Edicni_komentar" />

	<xd:doc>
		<xd:desc>
			<xd:p>Odstraňuje z výstupu prvky, které se tam dostaly omylem.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="annotation_reference | footnote_reference" />
	
	<xd:doc>
		<xd:desc>
			<xd:p>Odstraňuje z výstupu interní prvky.</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="interni_poznamka | interni_poznamka_kurziva | poznamka_pod_carou" />
	
	
</xsl:stylesheet>