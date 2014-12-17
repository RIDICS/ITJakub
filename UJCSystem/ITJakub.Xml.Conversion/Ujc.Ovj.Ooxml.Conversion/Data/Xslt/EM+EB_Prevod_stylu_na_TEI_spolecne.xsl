<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
	xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
	xmlns:xml="http://www.w3.org/XML/1998/namespace"
	exclude-result-prefixes="xd" version="1.0"
	>
	<xsl:template match="Edicni_komentar">
		<xsl:element name="div">
			<xsl:attribute name="type">
				<xsl:text>editorial</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>comment</xsl:text>
			</xsl:attribute>
			<xsl:element name="p">
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Edicni_komentar_Nadpis">
<!--		<xsl:element name="div">-->
<!--			<xsl:attribute name="type">
				<xsl:text>editorial</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>comment</xsl:text>
			</xsl:attribute>
-->
		<xsl:element name="thead">
				<xsl:attribute name="type">
					<xsl:text>editorial</xsl:text>
				</xsl:attribute>
				<xsl:attribute name="subtype">
					<xsl:text>comment</xsl:text>
				</xsl:attribute>
				<xsl:apply-templates/>
			</xsl:element>
<!--		</xsl:element>-->
	</xsl:template>
	
	<xsl:template match="Edicni_komentar_Podnadpis">
<!--		<xsl:element name="div">-->
<!--			<xsl:attribute name="type">
				<xsl:text>editorial</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>comment</xsl:text>
			</xsl:attribute>
-->
		<xsl:element name="head">
			<xsl:attribute name="type">
				<xsl:text>editorial</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>comment</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
<!--		</xsl:element>-->
	</xsl:template>

	<xsl:template match="torzo">
		<xsl:choose>
			<xsl:when test="starts-with(., '…')">
				<xsl:element name="gap"/>
				<xsl:value-of select="translate(.,'…', '')"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="translate(.,'…', '')"/>
				<xsl:element name="gap"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>


	<xsl:template match="Incipit">
		<xsl:element name="div">
			<xsl:attribute name="type">
				<xsl:text>incipit</xsl:text>
			</xsl:attribute>
			<xsl:element name="p">
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>


	<xsl:template match="Explicit">
		<xsl:element name="div">
			<xsl:attribute name="type">
				<xsl:text>explicit</xsl:text>
			</xsl:attribute>
			<xsl:element name="p">
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	
	<xsl:template match="Grantova_podpora">
		<xsl:element name="div">
			<xsl:attribute name="type">
				<xsl:text>editorial</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>grant</xsl:text>
			</xsl:attribute>
			<xsl:element name="p">
				<xsl:attribute name="rend">
					<xsl:text>grant</xsl:text>
				</xsl:attribute>
				<xsl:apply-templates/>
			</xsl:element>
		</xsl:element>
	</xsl:template>
	

	<xsl:template match="iniciala">
		<xsl:element name="c">
			<xsl:attribute name="function">
				<xsl:text>initial</xsl:text>
			</xsl:attribute>
			<xsl:apply-templates/>
		</xsl:element>
	</xsl:template>


	<xsl:template match="bible_zkratka_knihy">
		<xsl:element name="anchor">
			<xsl:attribute name="type">
				<xsl:text>bible</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>book</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="xml:id">
				<xsl:text>b.</xsl:text>
				<xsl:value-of select="generate-id()"/>
				<xsl:text>.</xsl:text>
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:attribute>
			<xsl:attribute name="n">
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

	<xsl:template match="bible_cislo_kapitoly">
		<xsl:element name="anchor">
			<xsl:attribute name="type">
				<xsl:text>bible</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>chapter</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="xml:id">
				<xsl:text>c.</xsl:text>
				<xsl:value-of select="generate-id()"/>
				<xsl:text>.</xsl:text>
				<xsl:value-of select="normalize-space(translate(., ',', ''))"/>
			</xsl:attribute>
			<xsl:attribute name="n">
				<xsl:value-of select="normalize-space(translate(., ',', ''))"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

	<xsl:template match="bible_cislo_verse">
		<xsl:element name="anchor">
			<xsl:attribute name="type">
				<xsl:text>bible</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="subtype">
				<xsl:text>verse</xsl:text>
			</xsl:attribute>
			<xsl:attribute name="xml:id">
				<xsl:text>v.</xsl:text>
				<xsl:value-of select="generate-id()"/>
				<xsl:text>.</xsl:text>
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:attribute>
			<xsl:attribute name="n">
				<xsl:value-of select="normalize-space(.)"/>
			</xsl:attribute>
		</xsl:element>
	</xsl:template>

	<xsl:template match="textovy_orientator">
		<xsl:element name="add">
			<xsl:attribute name="place"><xsl:text>margin</xsl:text></xsl:attribute>
			<xsl:attribute name="type"><xsl:text>orientating</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="cizi_jazyk_doplneny_text">
		<xsl:element name="foreign">
			<xsl:element name="supplied">
				<xsl:apply-templates />
			</xsl:element>
		</xsl:element>
	</xsl:template>

	<xsl:template match="cizi_jazyk_horni_index">
		<xsl:element name="foreign">
			<xsl:element name="hi">
				<xsl:attribute name="rend"><xsl:text>sup</xsl:text></xsl:attribute>
				<xsl:apply-templates />
			</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Polozka_rejstriku">
		<item>
			<xsl:apply-templates/>
		</item>
	</xsl:template>

	<xsl:template match="kurziva">
		<xsl:element name="hi">
			<xsl:attribute name="rend"><xsl:text>italic</xsl:text></xsl:attribute>
			<xsl:apply-templates />
		</xsl:element>
	</xsl:template>

	<xd:doc>
		<xd:desc>
			<xd:p>Ignorované elementy</xd:p>
		</xd:desc>
	</xd:doc>
	<xsl:template match="transliterace"/>
	<xsl:template match="transliterace_rozepsani_zkratky"/>
	<xsl:template match="transliterace_horni_index" />
	<xsl:template match="transliterace_cizi_jazyk" />
	
</xsl:stylesheet>

